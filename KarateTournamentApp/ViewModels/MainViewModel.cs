using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using KarateTournamentApp.Models;
using KarateTournamentApp.Services;

namespace KarateTournamentApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly CategoryManager _categoryManager;

        // Form data
        private string _firstName;
        public string FirstName { get => _firstName; set { _firstName = value; OnPropertyChanged(); } }

        private string _lastName;
        public string LastName { get => _lastName; set { _lastName = value; OnPropertyChanged(); } }

        private int? _age; 
        public int? Age { get => _age; set { _age = value; OnPropertyChanged(); } }

        private Sex _selectedSex;
        public Sex SelectedSex { get => _selectedSex; set { _selectedSex = value; OnPropertyChanged(); } }

        private Belts _selectedBelt;
        public Belts SelectedBelt { get => _selectedBelt; set { _selectedBelt = value; OnPropertyChanged(); } }

        private string _club;
        public string Club { get => _club; set { _club = value; OnPropertyChanged(); } }

        public ObservableCollection<CategorySelectionItem> CategorySelections { get; set; }

        public ObservableCollection<Participant> AllParticipants { get; set; }

        public ICommand AddParticipantCommand { get; }

        public MainViewModel(CategoryManager categoryManager)
        {
            _categoryManager = categoryManager;

            CategorySelections = new ObservableCollection<CategorySelectionItem>
            {
                new CategorySelectionItem(CategoryType.Kata, "Kata"),
                new CategorySelectionItem(CategoryType.Kumite, "Kumite"),
                new CategorySelectionItem(CategoryType.Kihon, "Kihon"),
                new CategorySelectionItem(CategoryType.KobudoShort, "Kobudo (Short)"),
                new CategorySelectionItem(CategoryType.KobudoLong, "Kobudo (Long)"),
                new CategorySelectionItem(CategoryType.Grappling, "Grappling")
            };

            AllParticipants = new ObservableCollection<Participant>();

            AddParticipantCommand = new RelayCommand(o => CreateParticipant(), o => CanCreateParticipant());
        }

        private bool CanCreateParticipant()
        {
            return !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   Age.HasValue && Age > 0 &&
                   CategorySelections.Any(c => c.IsSelected);
        }

        private void CreateParticipant()
        {
            var selectedCategories = CategorySelections
                .Where(c => c.IsSelected)
                .Select(c => c.CategoryType)
                .ToList();

            var newParticipant = new Participant(
                FirstName,
                LastName,
                Age.Value,
                SelectedBelt,
                SelectedSex,
                selectedCategories,
                Club
            );

            _categoryManager.AssignParticipant(newParticipant);
            AllParticipants.Add(newParticipant);

            ResetForm();
        }

        private void ResetForm()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Age = null;
            Club = string.Empty;
            
            foreach (var category in CategorySelections)
            {
                category.IsSelected = false;
            }
        }
    }
}