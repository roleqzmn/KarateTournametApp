using System.Windows.Input;
using KarateTournamentApp.Models;

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

        private void RequestMerge()
        {
            _mergeRequestCallback?.Invoke(this);
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
    }
}
