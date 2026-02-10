using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using KarateTournamentApp.Models;
using KarateTournamentApp.Commands;

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
                OnPropertyChanged(nameof(SenshuEnabled));
                OnPropertyChanged(nameof(HasSenshuAka));
                OnPropertyChanged(nameof(HasSenshuShiro));
                OnPropertyChanged(nameof(IsInOvertime));
            }
        }

        public bool IsShobuSanbon => CurrentMatch is ShobuSanbonMatch;

        // Senshu properties
        public bool SenshuEnabled
        {
            get => CurrentMatch is ShobuSanbonMatch match && match.SenshuEnabled;
            set
            {
                if (CurrentMatch is ShobuSanbonMatch match)
                {
                    match.SenshuEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HasSenshuAka => CurrentMatch is ShobuSanbonMatch match && match.HasSenshuAka;
        public bool HasSenshuShiro => CurrentMatch is ShobuSanbonMatch match && match.HasSenshuShiro;
        public bool IsInOvertime => CurrentMatch is ShobuSanbonMatch match && match.IsInOvertime;

        public Participant? AkaParticipant => CurrentMatch?.Aka.HasValue == true 
            ? _category.Participants.FirstOrDefault(p => p.Id == CurrentMatch.Aka.Value)
            : null;

        public Participant? ShiroParticipant => CurrentMatch?.Shiro.HasValue == true
            ? _category.Participants.FirstOrDefault(p => p.Id == CurrentMatch.Shiro.Value)
            : null;

        public ICommand NextMatchCommand { get; }
        public ICommand FinishMatchCommand { get; }
        public ICommand StartOvertimeCommand { get; }
        public ICommand SetTimeCommand { get; }

        public CompetitionManagerViewModel(Category category)
        {
            _category = category;
            
            // Initialize bracket if not already done
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
            StartOvertimeCommand = new RelayCommand(o => StartOvertime(), o => CanStartOvertime());
            SetTimeCommand = new RelayCommand(SetTime, o => CurrentMatch is ShobuSanbonMatch);
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
                // Check for draw
                if (CurrentMatch.AkaScore == CurrentMatch.ShiroScore)
                {
                    if (CurrentMatch is ShobuSanbonMatch shobuMatch)
                    {
                        // Check Senshu if enabled
                        if (shobuMatch.SenshuEnabled && (shobuMatch.HasSenshuAka || shobuMatch.HasSenshuShiro))
                        {
                            // Winner by Senshu
                            CurrentMatch.WinnerId = shobuMatch.HasSenshuAka ? CurrentMatch.Aka : CurrentMatch.Shiro;
                            CurrentMatch.IsFinished = true;
                            
                            System.Windows.MessageBox.Show(
                                $"Remis! Zwyciêzca przez SENSHU: {(shobuMatch.HasSenshuAka ? "AKA" : "SHIRO")}",
                                "Rozstrzygniêcie przez Senshu",
                                System.Windows.MessageBoxButton.OK,
                                System.Windows.MessageBoxImage.Information);
                        }
                        else
                        {
                            // Draw without Senshu - need overtime
                            var result = System.Windows.MessageBox.Show(
                                "Remis! Czy rozpocz¹æ dogrywkê (+60 sekund)?",
                                "Dogrywka",
                                System.Windows.MessageBoxButton.YesNo,
                                System.Windows.MessageBoxImage.Question);

                            if (result == System.Windows.MessageBoxResult.Yes)
                            {
                                StartOvertime();
                                return; // Don't finish the match yet
                            }
                            else
                            {
                                // Manual decision or cancel
                                System.Windows.MessageBox.Show(
                                    "Walka niezakoñczona. U¿yj DrawResolver lub rêcznie wybierz zwyciêzcê.",
                                    "Uwaga",
                                    System.Windows.MessageBoxButton.OK,
                                    System.Windows.MessageBoxImage.Warning);
                                return;
                            }
                        }
                    }
                    else
                    {
                        // Non-Shobu Sanbon draw (shouldn't happen, but handle it)
                        System.Windows.MessageBox.Show("Remis! Rêcznie wybierz zwyciêzcê.", "Remis", 
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                        return;
                    }
                }
                else
                {
                    // Determine winner based on score
                    if (CurrentMatch.AkaScore > CurrentMatch.ShiroScore)
                    {
                        CurrentMatch.WinnerId = CurrentMatch.Aka;
                    }
                    else
                    {
                        CurrentMatch.WinnerId = CurrentMatch.Shiro;
                    }
                    
                    CurrentMatch.IsFinished = true;
                }
                
                OnPropertyChanged(nameof(CurrentMatch));
            }
        }

        private bool CanStartOvertime()
        {
            return CurrentMatch is ShobuSanbonMatch match 
                   && !match.IsFinished 
                   && match.TimeRemaining <= 0
                   && match.AkaScore == match.ShiroScore;
        }

        private void StartOvertime()
        {
            if (CurrentMatch is ShobuSanbonMatch shobuMatch)
            {
                shobuMatch.IsInOvertime = true;
                shobuMatch.OvertimeCount++;
                shobuMatch.TimeRemaining = 60; // Add 60 seconds
                shobuMatch.IsRunning = false; // Stop timer, judge needs to start it manually
                
                // Reset Senshu for overtime (fresh start)
                shobuMatch.HasSenshuAka = false;
                shobuMatch.HasSenshuShiro = false;
                
                OnPropertyChanged(nameof(CurrentMatch));
                OnPropertyChanged(nameof(IsInOvertime));
                
                System.Windows.MessageBox.Show(
                    $"Dogrywka {shobuMatch.OvertimeCount} rozpoczêta!\n+60 sekund dodane.\nSenshu zresetowane.",
                    "Dogrywka",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
        }

        private void SetTime(object parameter)
        {
            if (CurrentMatch is ShobuSanbonMatch shobuMatch)
            {
                if (parameter is string timeText && double.TryParse(timeText, out double seconds))
                {
                    if (seconds >= 0 && seconds <= 600) // Max 10 minutes
                    {
                        shobuMatch.TimeRemaining = seconds;
                        shobuMatch.IsRunning = false; // Stop timer when manually setting time
                        OnPropertyChanged(nameof(CurrentMatch));
                        
                        System.Diagnostics.Debug.WriteLine($"Time set to {seconds} seconds");
                    }
                }
            }
        }

        public void UpdateMatchScore(bool isAka, int points)
        {
            if (CurrentMatch != null && !CurrentMatch.IsFinished)
            {
                // Check if this is the first point scored (for Senshu)
                if (CurrentMatch is ShobuSanbonMatch shobuMatch && shobuMatch.SenshuEnabled)
                {
                    bool isFirstPoint = shobuMatch.AkaScore == 0 && shobuMatch.ShiroScore == 0;
                    
                    if (isFirstPoint && points > 0)
                    {
                        if (isAka)
                        {
                            shobuMatch.HasSenshuAka = true;
                            System.Diagnostics.Debug.WriteLine("SENSHU dla AKA - pierwszy punkt!");
                        }
                        else
                        {
                            shobuMatch.HasSenshuShiro = true;
                            System.Diagnostics.Debug.WriteLine("SENSHU dla SHIRO - pierwszy punkt!");
                        }
                        
                        OnPropertyChanged(nameof(HasSenshuAka));
                        OnPropertyChanged(nameof(HasSenshuShiro));
                    }
                }
                
                // Update score
                if (isAka)
                {
                    CurrentMatch.AkaScore += (short)points;
                    if (CurrentMatch.AkaScore < 0) CurrentMatch.AkaScore = 0;
                }
                else
                {
                    CurrentMatch.ShiroScore += (short)points;
                    if (CurrentMatch.ShiroScore < 0) CurrentMatch.ShiroScore = 0;
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
