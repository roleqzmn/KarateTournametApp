using System.Windows.Input;
using KarateTournamentApp.Models;
using KarateTournamentApp.Commands;

namespace KarateTournamentApp.ViewModels
{
    /// <summary>
    /// ViewModel for judge control panel
    /// </summary>
    public class ScoreboardJudgeViewModel : ViewModelBase
    {
        private readonly CompetitionManagerViewModel _competitionManager;
        private readonly ScoreboardViewModel _scoreboardViewModel;

        public CompetitionManagerViewModel CompetitionManager => _competitionManager;

        public string CategoryName => _competitionManager.Category.Name;
        public string AkaName => _competitionManager.AkaParticipant?.FullName ?? "---";
        public string ShiroName => _competitionManager.ShiroParticipant?.FullName ?? "---";
        public bool IsShobuSanbon => _competitionManager.IsShobuSanbon;

        // Commands for Shobu Sanbon
        public ICommand AddAkaPointCommand { get; }
        public ICommand AddShiroPointCommand { get; }
        public ICommand RemoveAkaPointCommand { get; }
        public ICommand RemoveShiroPointCommand { get; }
        public ICommand AddAkaPenaltyCommand { get; }
        public ICommand AddShiroPenaltyCommand { get; }
        public ICommand RemoveAkaPenaltyCommand { get; }
        public ICommand RemoveShiroPenaltyCommand { get; }
        public ICommand StartTimerCommand { get; }
        public ICommand StopTimerCommand { get; }
        public ICommand ResetTimerCommand { get; }
        public ICommand SetTimeCommand { get; }
        public ICommand FinishMatchCommand { get; }
        public ICommand NextMatchCommand { get; }

        // Commands for other categories (Kata, Kumite, etc.)
        public ICommand AddJudgeScoreCommand { get; }

        private string _timeInput;
        public string TimeInput
        {
            get => _timeInput;
            set
            {
                _timeInput = value;
                OnPropertyChanged();
            }
        }

        public ScoreboardJudgeViewModel(CompetitionManagerViewModel competitionManager, ScoreboardViewModel scoreboardViewModel = null)
        {
            _competitionManager = competitionManager;
            _scoreboardViewModel = scoreboardViewModel;

            // Initialize commands
            AddAkaPointCommand = new RelayCommand(o => AddPoint(true, 1));
            AddShiroPointCommand = new RelayCommand(o => AddPoint(false, 1));
            RemoveAkaPointCommand = new RelayCommand(o => AddPoint(true, -1));
            RemoveShiroPointCommand = new RelayCommand(o => AddPoint(false, -1));

            AddAkaPenaltyCommand = new RelayCommand(o => AddPenalty(true, 1));
            AddShiroPenaltyCommand = new RelayCommand(o => AddPenalty(false, 1));
            RemoveAkaPenaltyCommand = new RelayCommand(o => AddPenalty(true, -1));
            RemoveShiroPenaltyCommand = new RelayCommand(o => AddPenalty(false, -1));

            StartTimerCommand = new RelayCommand(o => StartTimer());
            StopTimerCommand = new RelayCommand(o => StopTimer());
            ResetTimerCommand = new RelayCommand(o => ResetTimer());
            SetTimeCommand = new RelayCommand(o => SetTime(o), o => CanSetTime(o));

            FinishMatchCommand = _competitionManager.FinishMatchCommand;
            NextMatchCommand = _competitionManager.NextMatchCommand;

            AddJudgeScoreCommand = new RelayCommand(o => AddJudgeScore(o));

            // Subscribe to changes
            _competitionManager.PropertyChanged += (s, e) => RefreshAll();
        }

        private void AddPoint(bool isAka, int points)
        {
            _competitionManager.UpdateMatchScore(isAka, points);
            RefreshAll();
        }

        private void AddPenalty(bool isAka, int change)
        {
            _competitionManager.UpdatePenalty(isAka, change);
            RefreshAll();
        }

        private void StartTimer()
        {
            _scoreboardViewModel?.StartTimer();
        }

        private void StopTimer()
        {
            _scoreboardViewModel?.StopTimer();
        }

        private void ResetTimer()
        {
            if (_competitionManager.CurrentMatch is ShobuSanbonMatch shobuMatch)
            {
                shobuMatch.TimeRemaining = 180;
                shobuMatch.IsRunning = false;
                _scoreboardViewModel?.StopTimer();
                RefreshAll();
            }
        }

        private bool CanSetTime(object parameter)
        {
            if (_competitionManager.CurrentMatch is not ShobuSanbonMatch)
                return false;
            
            // If parameter is provided (quick preset button)
            if (parameter is string)
                return true;
            
            // If manual input
            return !string.IsNullOrWhiteSpace(TimeInput);
        }

        private void SetTime(object parameter)
        {
            if (_competitionManager.CurrentMatch is ShobuSanbonMatch shobuMatch)
            {
                string timeValue = parameter as string ?? TimeInput;
                
                if (string.IsNullOrWhiteSpace(timeValue))
                    return;
                
                // Try to parse as seconds first
                if (double.TryParse(timeValue, out double seconds))
                {
                    if (seconds >= 0 && seconds <= 600) // Max 10 minutes
                    {
                        shobuMatch.TimeRemaining = seconds;
                        shobuMatch.IsRunning = false;
                        _scoreboardViewModel?.StopTimer();
                        TimeInput = string.Empty;
                        RefreshAll();
                        return;
                    }
                }
                
                // Try to parse as MM:SS format
                var parts = timeValue.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[0], out int minutes) && int.TryParse(parts[1], out int secs))
                {
                    if (minutes >= 0 && minutes <= 10 && secs >= 0 && secs < 60)
                    {
                        double totalSeconds = minutes * 60 + secs;
                        shobuMatch.TimeRemaining = totalSeconds;
                        shobuMatch.IsRunning = false;
                        _scoreboardViewModel?.StopTimer();
                        TimeInput = string.Empty;
                        RefreshAll();
                        return;
                    }
                }
                
                // Invalid input
                System.Windows.MessageBox.Show(
                    "Nieprawidłowy format czasu!\nUżyj sekund (np. 180) lub MM:SS (np. 3:00)",
                    "Błąd",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
            }
        }

        private void AddJudgeScore(object parameter)
        {
            // Logic for adding judge scores in Kata/Kumite categories
        }

        private void RefreshAll()
        {
            OnPropertyChanged(nameof(AkaName));
            OnPropertyChanged(nameof(ShiroName));
            OnPropertyChanged(nameof(CategoryName));
        }
    }
}
