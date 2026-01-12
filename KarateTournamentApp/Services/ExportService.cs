using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KarateTournamentApp.Services
{
    public class ExportService
    {
        private readonly JsonService _jsonService;

        public ExportService()
        {
            _jsonService = new JsonService();
        }

        public async Task ExportDataAsync(CategoryManager _categoryManager)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                Title = "Exportuj dane turnieju",
                FileName = $"tournament_data_{DateTime.Now:yyyy-MM-dd_HH-mm}.json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    await _jsonService.SaveTournamentDataAsync(saveFileDialog.FileName, _categoryManager.DefinedCategories);
                    MessageBox.Show($"Pomyślnie wyeksportowano {_categoryManager.DefinedCategories.Count} kategorii!",
                        "Export zakończony", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas exportu danych:\n{ex.Message}",
                        "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
