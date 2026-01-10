using KarateTournamentApp.Models;
using KarateTournamentApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KarateTournamentApp.Services
{
    public class ImportService
    {
        private readonly JsonService _jsonService;

        public ImportService()
        {
            _jsonService = new JsonService();
        }
        public void ImportData(CategoryManager _categoryManager, ObservableCollection<Participant> AllParticipants)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                Title = "Importuj dane turnieju"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var importedCategories = _jsonService.LoadTournamentData(openFileDialog.FileName);

                    if (importedCategories != null && importedCategories.Any())
                    {
                        _categoryManager.DefinedCategories.Clear();
                        _categoryManager.DefinedCategories.AddRange(importedCategories);

                        AllParticipants.Clear();
                        foreach (var category in importedCategories)
                        {
                            foreach (var participant in category.Participants)
                            {
                                if (!AllParticipants.Any(p => p.Id == participant.Id))
                                {
                                    AllParticipants.Add(participant);
                                }
                            }
                        }


                        MessageBox.Show($"Pomyślnie zaimportowano {importedCategories.Count} kategorii i {AllParticipants.Count} zawodników!",
                            "Import zakończony", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Plik nie zawiera żadnych danych.",
                            "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas importu danych:\n{ex.Message}",
                        "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
