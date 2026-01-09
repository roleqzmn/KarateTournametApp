using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using KarateTournamentApp.Models;

namespace KarateTournamentApp.ViewModels
{
    /// <summary>
    /// Manages individual competitions where participants perform one at a time (Kata, Kumite judging, etc.)
    /// </summary>
    public class IndividualCompetitionManagerViewModel : ViewModelBase
    {
        private readonly Category _category;
        private int _currentParticipantIndex;
        
        public Category Category => _category;
        
        private Participant _currentParticipant;
        public Participant CurrentParticipant
        {
            get => _currentParticipant;
            set
            {
                _currentParticipant = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentParticipantNumber));
                OnPropertyChanged(nameof(TotalParticipants));
                
                // Reset judge scores for new participant
                JudgeScores.Clear();
                OnPropertyChanged(nameof(FinalScore));
            }
        }

        public int CurrentParticipantNumber => _currentParticipantIndex + 1;
        public int TotalParticipants => _category.Participants.Count;

        // Collection to store scores from multiple judges
        public ObservableCollection<decimal> JudgeScores { get; set; }

        // Calculate final score (remove highest and lowest, then average)
        public decimal FinalScore
        {
            get
            {
                if (JudgeScores.Count < 3) return 0;
                
                var sortedScores = JudgeScores.OrderBy(s => s).ToList();
                // Remove lowest and highest
                sortedScores.RemoveAt(0);
                sortedScores.RemoveAt(sortedScores.Count - 1);
                
                return sortedScores.Any() ? sortedScores.Average() : 0;
            }
        }

        // Store all participant results
        public ObservableCollection<ParticipantResult> Results { get; set; }

        public ICommand NextParticipantCommand { get; }
        public ICommand FinishParticipantCommand { get; }
        public ICommand AddJudgeScoreCommand { get; }
        public ICommand RemoveLastJudgeScoreCommand { get; }

        public IndividualCompetitionManagerViewModel(Category category)
        {
            _category = category;
            _currentParticipantIndex = 0;
            
            JudgeScores = new ObservableCollection<decimal>();
            Results = new ObservableCollection<ParticipantResult>();
            
            if (_category.Participants.Any())
            {
                CurrentParticipant = _category.Participants[_currentParticipantIndex];
            }

            NextParticipantCommand = new RelayCommand(o => MoveToNextParticipant(), o => CanMoveToNext());
            FinishParticipantCommand = new RelayCommand(o => FinishCurrentParticipant(), o => CanFinishParticipant());
            AddJudgeScoreCommand = new RelayCommand(AddJudgeScore);
            RemoveLastJudgeScoreCommand = new RelayCommand(o => RemoveLastScore(), o => JudgeScores.Any());
        }

        private bool CanMoveToNext()
        {
            return _currentParticipantIndex < _category.Participants.Count - 1;
        }

        private bool CanFinishParticipant()
        {
            return CurrentParticipant != null && JudgeScores.Count >= 3;
        }

        private void MoveToNextParticipant()
        {
            if (_currentParticipantIndex < _category.Participants.Count - 1)
            {
                _currentParticipantIndex++;
                CurrentParticipant = _category.Participants[_currentParticipantIndex];
            }
            else
            {
                // Competition finished
                _category.IsFinished = true;
                CurrentParticipant = null;
            }
        }

        private void FinishCurrentParticipant()
        {
            if (CurrentParticipant != null && JudgeScores.Count >= 3)
            {
                var result = new ParticipantResult
                {
                    Participant = CurrentParticipant,
                    Score = FinalScore,
                    JudgeScores = new List<decimal>(JudgeScores)
                };
                
                Results.Add(result);
                
                // Move to next participant automatically
                MoveToNextParticipant();
            }
        }

        private void AddJudgeScore(object parameter)
        {
            if (parameter is string scoreText && decimal.TryParse(scoreText, out decimal score))
            {
                if (score >= 0 && score <= 10) // Assuming 0-10 scale
                {
                    JudgeScores.Add(score);
                    OnPropertyChanged(nameof(FinalScore));
                }
            }
        }

        private void RemoveLastScore()
        {
            if (JudgeScores.Any())
            {
                JudgeScores.RemoveAt(JudgeScores.Count - 1);
                OnPropertyChanged(nameof(FinalScore));
            }
        }

        public ObservableCollection<ParticipantResult> GetFinalRankings()
        {
            return new ObservableCollection<ParticipantResult>(Results.OrderByDescending(r => r.Score));
        }
    }

    /// <summary>
    /// Stores the result for a participant in an individual competition
    /// </summary>
    public class ParticipantResult
    {
        public Participant Participant { get; set; }
        public decimal Score { get; set; }
        public List<decimal> JudgeScores { get; set; }
    }
}
