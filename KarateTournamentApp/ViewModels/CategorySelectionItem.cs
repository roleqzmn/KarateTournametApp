using KarateTournamentApp.Models;

namespace KarateTournamentApp.ViewModels
{
    public class CategorySelectionItem : ViewModelBase
    {
        public CategoryType CategoryType { get; set; }
        
        public string DisplayName { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public CategorySelectionItem(CategoryType categoryType, string displayName)
        {
            CategoryType = categoryType;
            DisplayName = displayName;
            IsSelected = false;
        }
    }
}
