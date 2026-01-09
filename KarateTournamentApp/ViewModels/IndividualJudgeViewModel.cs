using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using KarateTournamentApp.Models;

namespace KarateTournamentApp.ViewModels
{
    /// <summary>
    /// ViewModel for judge panel in individual competitions
    /// </summary>
    public class IndividualJudgeViewModel : ViewModelBase
    {
        private readonly IndividualCompetitionManagerViewModel _competitionManager;

        public IndividualCompetitionManagerViewModel CompetitionManager => _competitionManager;

        public string CategoryName => _competitionManager.Category.Name;
        public string CurrentParticipantName => _competitionManager.CurrentParticipant?.FullName ?? "---";
        
        public ObservableCollection<decimal> JudgeScores => _competitionManager.JudgeScores;
        public decimal FinalScore => _competitionManager.FinalScore;

        // Commands
        public ICommand AddScoreCommand { get; }
        public ICommand RemoveLastScoreCommand { get; }
        public ICommand FinishParticipantCommand { get; }
        public ICommand NextParticipantCommand { get; }
        public ICommand ShowResultsCommand { get; }

        // For quick score entry buttons
        public ICommand AddQuickScoreCommand { get; }

        private string _scoreInput;
        public string ScoreInput
        {
            get => _scoreInput;
            set
            {
                _scoreInput = value;
                OnPropertyChanged();
            }
        }

        public IndividualJudgeViewModel(IndividualCompetitionManagerViewModel competitionManager)
        {
            _competitionManager = competitionManager;

            AddScoreCommand = new RelayCommand(o => AddScore());
            RemoveLastScoreCommand = _competitionManager.RemoveLastJudgeScoreCommand;
            FinishParticipantCommand = _competitionManager.FinishParticipantCommand;
            NextParticipantCommand = _competitionManager.NextParticipantCommand;
            ShowResultsCommand = new RelayCommand(o => ShowResults());
            AddQuickScoreCommand = new RelayCommand(AddQuickScore);

            // Subscribe to changes
            _competitionManager.PropertyChanged += (s, e) => RefreshAll();
            _competitionManager.JudgeScores.CollectionChanged += (s, e) => RefreshScores();
        }

        private void AddScore()
        {
            if (!string.IsNullOrWhiteSpace(ScoreInput) && decimal.TryParse(ScoreInput, out decimal score))
            {
                if (score >= 0 && score <= 10)
                {
                    JudgeScores.Add(score);
                    ScoreInput = string.Empty;
                    RefreshScores();
                }
            }
        }

        private void AddQuickScore(object parameter)
        {
            if (parameter is decimal score)
            {
                JudgeScores.Add(score);
                RefreshScores();
            }
            else if (parameter is string scoreText && decimal.TryParse(scoreText, out decimal parsedScore))
            {
                JudgeScores.Add(parsedScore);
                RefreshScores();
            }
        }

        private void ShowResults()
        {
            // This would open a results window showing final rankings
            var resultsWindow = new System.Windows.Window
            {
                Title = $"Wyniki - {CategoryName}",
                Width = 800,
                Height = 600,
                Content = new Views.ResultsView
                {
                    DataContext = new ResultsViewModel(_competitionManager)
                }
            };
            resultsWindow.ShowDialog();
        }

        private void RefreshAll()
        {
            OnPropertyChanged(nameof(CurrentParticipantName));
            OnPropertyChanged(nameof(CategoryName));
            RefreshScores();
        }

        private void RefreshScores()
        {
            OnPropertyChanged(nameof(FinalScore));
        }
    }
}
