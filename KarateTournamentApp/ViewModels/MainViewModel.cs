using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows;
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

        private string? _club;
        public string? Club { get => _club; set { _club = value; OnPropertyChanged(); } }

        private bool _divideByAge;
        public bool DivideByAge 
        { 
            get => _divideByAge; 
            set 
            { 
                _divideByAge = value; 
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            } 
        }

        private bool _divideByBelt;
        public bool DivideByBelt 
        { 
            get => _divideByBelt; 
            set 
            { 
                _divideByBelt = value; 
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            } 
        }

        public ObservableCollection<CategorySelectionItem> CategorySelections { get; set; }

        public ObservableCollection<Participant> AllParticipants { get; set; }

        public ObservableCollection<CategoryViewModel> Categories { get; set; }

        private CategoryViewModel _selectedCategoryForMerge;
        public CategoryViewModel SelectedCategoryForMerge
        {
            get => _selectedCategoryForMerge;
            set
            {
                _selectedCategoryForMerge = value;
                OnPropertyChanged();
            }
        }

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
            Categories = new ObservableCollection<CategoryViewModel>();

            AddParticipantCommand = new RelayCommand(o => CreateParticipant(), o => CanCreateParticipant());
        }

        private bool CanCreateParticipant()
        {
            return !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   Age.HasValue && Age > 0 &&
                   CategorySelections.Any(c => c.IsSelected) &&
                   (DivideByAge || DivideByBelt);
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

            _categoryManager.AssignParticipant(newParticipant, DivideByBelt, DivideByAge);
            AllParticipants.Add(newParticipant);

            RefreshCategories();
            ResetForm();
        }

        private void RefreshCategories()
        {
            Categories.Clear();
            foreach (var category in _categoryManager.DefinedCategories)
            {
                Categories.Add(new CategoryViewModel(category, OnMergeRequested));
            }
        }

        private void OnMergeRequested(CategoryViewModel requestingCategory)
        {
            if (SelectedCategoryForMerge == null)
            {
                SelectedCategoryForMerge = requestingCategory;
                MessageBox.Show($"Kategoria '{requestingCategory.Name}' wybrana do połączenia.\n\nKliknij przycisk połączenia w innej kategorii tego samego typu, aby je scalić.",
                    "Wybrano kategorię", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (SelectedCategoryForMerge == requestingCategory)
            {
                SelectedCategoryForMerge = null;
                MessageBox.Show("Anulowano wybór kategorii.", "Anulowano", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (SelectedCategoryForMerge.Category.CategoryType != requestingCategory.Category.CategoryType)
                {
                    MessageBox.Show("Nie można połączyć kategorii różnych typów!\n\n" +
                        $"Wybrana: {SelectedCategoryForMerge.CategoryTypeDisplay}\n" +
                        $"Druga: {requestingCategory.CategoryTypeDisplay}",
                        "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    SelectedCategoryForMerge = null;
                    return;
                }

                var result = MessageBox.Show(
                    $"Czy na pewno chcesz połączyć kategorie:\n\n" +
                    $"'{SelectedCategoryForMerge.Name}'\n" +
                    $"Zawodnicy: {SelectedCategoryForMerge.ParticipantCount}\n\n" +
                    $"z\n\n" +
                    $"'{requestingCategory.Name}'\n" +
                    $"Zawodnicy: {requestingCategory.ParticipantCount}\n\n" +
                    $"Po połączeniu: {SelectedCategoryForMerge.ParticipantCount + requestingCategory.ParticipantCount} zawodników",
                    "Potwierdź połączenie", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    requestingCategory.Category.MergeWith(SelectedCategoryForMerge.Category);
                    _categoryManager.DefinedCategories.Remove(SelectedCategoryForMerge.Category);
                    
                    SelectedCategoryForMerge = null;
                    RefreshCategories();
                    
                    MessageBox.Show("Kategorie zostały pomyślnie połączone!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
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