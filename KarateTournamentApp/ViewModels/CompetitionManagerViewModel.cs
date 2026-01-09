using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using KarateTournamentApp.Models;

namespace KarateTournamentApp.ViewModels
{
    /// <summary>
    /// Manages the state of an ongoing competition/category
    /// </summary>
    public class CompetitionManagerViewModel : ViewModelBase
    {
        private readonly Category _category;
        private int _currentMatchIndex;
        
        public Category Category => _category;
        
        private Match _currentMatch;
        public Match CurrentMatch
        {
            get => _currentMatch;
            set
            {
                _currentMatch = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AkaParticipant));
                OnPropertyChanged(nameof(ShiroParticipant));
                OnPropertyChanged(nameof(IsShobuSanbon));
            }
        }

        public bool IsShobuSanbon => CurrentMatch is ShobuSanbonMatch;

        public Participant AkaParticipant => CurrentMatch?.Aka.HasValue == true 
            ? _category.Participants.FirstOrDefault(p => p.Id == CurrentMatch.Aka.Value)
            : null;

        public Participant ShiroParticipant => CurrentMatch?.Shiro.HasValue == true
            ? _category.Participants.FirstOrDefault(p => p.Id == CurrentMatch.Shiro.Value)
            : null;

        public ICommand NextMatchCommand { get; }
        public ICommand FinishMatchCommand { get; }

        public CompetitionManagerViewModel(Category category)
        {
            _category = category;
            
            // Initialize bracket if not already done~~
            if (!_category.BracketMatches.Any())
            {
                _category.InitializeBracket();
            }

            // Find first unfinished match
            _currentMatchIndex = FindNextUnfinishedMatch();
            if (_currentMatchIndex >= 0)
            {
                CurrentMatch = _category.BracketMatches[_currentMatchIndex];
            }

            NextMatchCommand = new RelayCommand(o => MoveToNextMatch(), o => CanMoveToNextMatch());
            FinishMatchCommand = new RelayCommand(o => FinishCurrentMatch(), o => CurrentMatch != null && !CurrentMatch.IsFinished);
        }

        private int FindNextUnfinishedMatch()
        {
            for (int i = _category.BracketMatches.Count - 1; i >= 0; i--)
            {
                var match = _category.BracketMatches[i];
                if (!match.IsFinished && match.Aka.HasValue && match.Shiro.HasValue)
                {
                    return i;
                }
            }
            return -1;
        }

        private bool CanMoveToNextMatch()
        {
            return CurrentMatch?.IsFinished == true;
        }

        private void MoveToNextMatch()
        {
            if (CurrentMatch.IsFinished)
            {
                _category.PromoteWinner(_currentMatchIndex);
                _currentMatchIndex = FindNextUnfinishedMatch();
                
                if (_currentMatchIndex >= 0)
                {
                    CurrentMatch = _category.BracketMatches[_currentMatchIndex];
                }
                else
                {
                    CurrentMatch = null;
                    _category.IsFinished = true;
                }
            }
        }

        private void FinishCurrentMatch()
        {
            if (CurrentMatch != null && !CurrentMatch.IsFinished)
            {
                // Determine winner based on score
                if (CurrentMatch.AkaScore > CurrentMatch.ShiroScore)
                {
                    CurrentMatch.WinnerId = CurrentMatch.Aka;
                }
                else if (CurrentMatch.ShiroScore > CurrentMatch.AkaScore)
                {
                    CurrentMatch.WinnerId = CurrentMatch.Shiro;
                }
                // TODO: Senshu/+1min if draw
                
                CurrentMatch.IsFinished = true;
                OnPropertyChanged(nameof(CurrentMatch));
            }
        }

        public void UpdateMatchScore(bool isAka, int points)
        {
            if (CurrentMatch != null && !CurrentMatch.IsFinished)
            {
                if (isAka)
                {
                    CurrentMatch.AkaScore += (short)points;
                }
                else
                {
                    CurrentMatch.ShiroScore += (short)points;
                }
                OnPropertyChanged(nameof(CurrentMatch));
            }
        }

        public void UpdatePenalty(bool isAka, int change)
        {
            if (CurrentMatch is ShobuSanbonMatch shobuMatch && !CurrentMatch.IsFinished)
            {
                if (isAka)
                {
                    shobuMatch.PenaltyAka = Math.Max(0, shobuMatch.PenaltyAka + change);
                }
                else
                {
                    shobuMatch.PenaltyShiro = Math.Max(0, shobuMatch.PenaltyShiro + change);
                }
                OnPropertyChanged(nameof(CurrentMatch));
            }
        }
    }
}
