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
                IsParticipantFinished = false;
                OnPropertyChanged(nameof(FinalScore));
            }
        }

        public int CurrentParticipantNumber => _currentParticipantIndex + 1;
        public int TotalParticipants => _category.Participants.Count;

        // Collection to store scores from multiple judges
        public ObservableCollection<decimal> JudgeScores { get; set; }

        private bool _isParticipantFinished;
        public bool IsParticipantFinished
        {
            get => _isParticipantFinished;
            set
            {
                _isParticipantFinished = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FinalScore));
            }
        }

        // Calculate final score (remove highest and lowest, then sum)
        public decimal FinalScore
        {
            get
            {
                if (!IsParticipantFinished || JudgeScores.Count < 3) return 0;
                
                var sortedScores = JudgeScores.OrderBy(s => s).ToList();
                // Remove lowest and highest
                sortedScores.RemoveAt(0);
                sortedScores.RemoveAt(sortedScores.Count - 1);
                
                return sortedScores.Sum();
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
                IsParticipantFinished = true;
                
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

        private ParticipantResult ResolveDraw(ParticipantResult participant1, ParticipantResult participant2)
        {
            // Open 1v1 draw resolver window
            var drawResolver = new DrawResolverViewModel(participant1.Participant, participant2.Participant);
            var drawWindow = new System.Windows.Window
            {
                Title = "Rozstrzyganie Remisu",
                Width = 800,
                Height = 600,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                ResizeMode = System.Windows.ResizeMode.NoResize,
                Content = new Views.DrawResolverView
                {
                    DataContext = drawResolver
                }
            };

            Participant winner = null;
            drawResolver.WinnerConfirmed += (sender, selectedWinner) =>
            {
                winner = selectedWinner;
                drawWindow.Close();
            };

            drawWindow.ShowDialog();

            // Return the winner's result
            return winner == participant1.Participant ? participant1 : participant2;
        }

        public ObservableCollection<ParticipantResult> GetFinalRankings()
        {
            var results = new ObservableCollection<ParticipantResult>(Results.OrderByDescending(r => r.Score));
            
            if (results.Count < 2) return results;

            // Resolve draw between 1st and 2nd place
            if (results[0].Score == results[1].Score)
            {
                var sortedScores1 = results[0].JudgeScores.OrderBy(s => s).ToList();
                var sortedScores2 = results[1].JudgeScores.OrderBy(s => s).ToList();
                
                // Compare highest score (after removing lowest and highest)
                if (sortedScores1[sortedScores1.Count - 1] < sortedScores2[sortedScores2.Count - 1])
                {
                    SwapResults(results, 0, 1);
                }
                // Compare lowest score  
                else if (sortedScores1[0] < sortedScores2[0])
                {
                    SwapResults(results, 0, 1);
                }
                // If still tied, resolve with 1v1 match
                else
                {
                    var winner = ResolveDraw(results[0], results[1]);
                    if (winner == results[1])
                    {
                        SwapResults(results, 0, 1);
                    }
                }
            }

            if (results.Count < 3) return results;

            // Resolve draw between 2nd and 3rd place
            if (results[1].Score == results[2].Score)
            {
                var sortedScores1 = results[1].JudgeScores.OrderBy(s => s).ToList();
                var sortedScores2 = results[2].JudgeScores.OrderBy(s => s).ToList();
                
                if (sortedScores1[sortedScores1.Count - 1] < sortedScores2[sortedScores2.Count - 1])
                {
                    SwapResults(results, 1, 2);
                }
                else if (sortedScores1[0] < sortedScores2[0])
                {
                    SwapResults(results, 1, 2);
                }
                else
                {
                    var winner = ResolveDraw(results[1], results[2]);
                    if (winner == results[2])
                    {
                        SwapResults(results, 1, 2);
                    }
                }
            }
            
            return results;
        }

        private void SwapResults(ObservableCollection<ParticipantResult> results, int index1, int index2)
        {
            var temp = results[index1];
            results[index1] = results[index2];
            results[index2] = temp;
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
