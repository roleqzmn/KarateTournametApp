using System.Windows.Input;
using KarateTournamentApp.Models;
using KarateTournamentApp.Views;

namespace KarateTournamentApp.ViewModels
{
    public class CategoryViewModel : ViewModelBase
    {
        private readonly Category _category;
        private readonly Action<CategoryViewModel> _mergeRequestCallback;

        public CategoryViewModel(Category category, Action<CategoryViewModel> mergeRequestCallback)
        {
            _category = category;
            _mergeRequestCallback = mergeRequestCallback;
            MergeCommand = new RelayCommand(o => RequestMerge(), o => true);
            RemoveParticipantCommand = new RelayCommand(RemoveParticipant, o => true);
            StartCompetitionCommand = new RelayCommand(o => StartCompetition(), o => CanStartCompetition());
            DeleteCommand = new RelayCommand(o => { }, o => false); // Placeholder for delete functionality
        }

        public Category Category => _category;

        public string Name => _category.Name;
        public int ParticipantCount => _category.Participants.Count;
        public string CategoryTypeDisplay => _category.CategoryType.ToString();
        public string SexDisplay => _category.Sex.ToString();
        public string AgeRangeDisplay
        {
            get
            {
                if (_category.MinAge.HasValue && _category.MaxAge.HasValue)
                {
                    if (_category.MinAge == _category.MaxAge)
                        return $"{_category.MinAge} lat";
                    return $"{_category.MinAge}-{_category.MaxAge} lat";
                }
                return "Brak";
            }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        public ICommand MergeCommand { get; }
        public ICommand RemoveParticipantCommand { get; }
        public ICommand StartCompetitionCommand { get; }
        public ICommand DeleteCommand { get; }

        private void RequestMerge()
        {
            _mergeRequestCallback?.Invoke(this);
        }

        private void RemoveParticipant(object parameter)
        {
            if (parameter is Participant participant)
            {
                _category.Participants.Remove(participant);
                OnPropertyChanged(nameof(ParticipantCount));
                Refresh();
            }
        }

        private void StartCategory()
        {
            _category.ConductCategory();
        }

        public void Refresh()
        {
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(ParticipantCount));
            OnPropertyChanged(nameof(CategoryTypeDisplay));
            OnPropertyChanged(nameof(SexDisplay));
            OnPropertyChanged(nameof(AgeRangeDisplay));
            OnPropertyChanged(nameof(Category));
        }

        private bool CanStartCompetition()
        {
            return _category.Participants.Count >= 2 && !_category.IsFinished;
        }

        private void StartCompetition()
        {
            // Check if this is a Shobu Sanbon category (bracket-style competition)
            if (_category is ShobuSanbonCategory)
            {
                StartBracketCompetition();
            }
            else
            {
                // All other categories are individual (Kata, Kumite, Kihon, Kobudo, Grappling)
                StartIndividualCompetition();
            }
        }

        private void StartBracketCompetition()
        {
            var competitionManager = new CompetitionManagerViewModel(_category);
            var scoreboardViewModel = new ScoreboardViewModel(competitionManager);
            
            // Create and show Scoreboard View (public display)
            var scoreboardWindow = new System.Windows.Window
            {
                Title = $"Tablica wynikow - {_category.Name}",
                Width = 1200,
                Height = 800,
                WindowState = System.Windows.WindowState.Maximized,
                Content = new ScoreboardView
                {
                    DataContext = scoreboardViewModel
                }
            };

            // Create and show Judge Panel
            var judgeWindow = new System.Windows.Window
            {
                Title = $"Panel sedziowski - {_category.Name}",
                Width = 800,
                Height = 600,
                Content = new ScoreboardJudge
                {
                    DataContext = new ScoreboardJudgeViewModel(competitionManager, scoreboardViewModel)
                }
            };

            scoreboardWindow.Show();
            judgeWindow.Show();
        }

        private void StartIndividualCompetition()
        {
            var competitionManager = new IndividualCompetitionManagerViewModel(_category);
            var scoreboardViewModel = new IndividualScoreboardViewModel(competitionManager);
            
            // Create and show Scoreboard View (public display)
            var scoreboardWindow = new System.Windows.Window
            {
                Title = $"Tablica wynikow - {_category.Name}",
                Width = 1200,
                Height = 800,
                WindowState = System.Windows.WindowState.Maximized,
                Content = new IndividualScoreboardView
                {
                    DataContext = scoreboardViewModel
                }
            };

            // Create and show Judge Panel
            var judgeWindow = new System.Windows.Window
            {
                Title = $"Panel sedziowski - {_category.Name}",
                Width = 900,
                Height = 700,
                Content = new IndividualJudgeView
                {
                    DataContext = new IndividualJudgeViewModel(competitionManager)
                }
            };

            scoreboardWindow.Show();
            judgeWindow.Show();
        }
    }
}
