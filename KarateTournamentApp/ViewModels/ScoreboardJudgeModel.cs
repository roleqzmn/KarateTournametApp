using System.Windows.Input;
using KarateTournamentApp.Models;

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
        public ICommand FinishMatchCommand { get; }
        public ICommand NextMatchCommand { get; }

        // Commands for other categories (Kata, Kumite, etc.)
        public ICommand AddJudgeScoreCommand { get; }

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
