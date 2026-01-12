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
        private readonly XmlImportService _xmlImportService;
        private readonly ExcelImportService _excelImportService;

        public ImportService()
        {
            _jsonService = new JsonService();
            _xmlImportService = new XmlImportService();
            _excelImportService = new ExcelImportService();
        }

        public async Task ImportParticipantsFromXmlAsync(CategoryManager categoryManager, ObservableCollection<Participant> allParticipants, bool divideByBelt, bool divideByAge)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*",
                Title = "Import Participants from XML"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var importedParticipants = await _xmlImportService.ImportParticipantsFromXmlAsync(openFileDialog.FileName);

                    if (importedParticipants != null && importedParticipants.Any())
                    {
                        int addedCount = 0;
                        foreach (var participant in importedParticipants)
                        {
                            if (!allParticipants.Any(p => p.FirstName == participant.FirstName && p.LastName == participant.LastName))
                            {
                                categoryManager.AssignParticipant(participant, divideByBelt, divideByAge);
                                allParticipants.Add(participant);
                                addedCount++;
                            }
                        }

                        MessageBox.Show($"Successfully imported {addedCount} of {importedParticipants.Count} participants!\n\n" +
                            $"Skipped: {importedParticipants.Count - addedCount} (duplicates or invalid data)",
                            "Import Completed", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("File does not contain any valid participant data.",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error importing data from XML:\n{ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public async Task ImportParticipantsFromExcelAsync(CategoryManager categoryManager, ObservableCollection<Participant> allParticipants, bool divideByBelt, bool divideByAge)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel files (*.xlsx;*.xls)|*.xlsx;*.xls|All files (*.*)|*.*",
                Title = "Import Participants from Excel"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var importedParticipants = await _excelImportService.ImportParticipantsFromExcelAsync(openFileDialog.FileName);

                    if (importedParticipants != null && importedParticipants.Any())
                    {
                        int addedCount = 0;
                        foreach (var participant in importedParticipants)
                        {
                            if (!allParticipants.Any(p => p.FirstName == participant.FirstName && p.LastName == participant.LastName))
                            {
                                categoryManager.AssignParticipant(participant, divideByBelt, divideByAge);
                                allParticipants.Add(participant);
                                addedCount++;
                            }
                        }

                        MessageBox.Show($"Successfully imported {addedCount} of {importedParticipants.Count} participants!\n\n" +
                            $"Skipped: {importedParticipants.Count - addedCount} (duplicates or invalid data)",
                            "Import Completed", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("File does not contain any valid participant data.",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error importing data from Excel:\n{ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public async Task CreateSampleXmlFileAsync()
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "XML files (*.xml)|*.xml",
                Title = "Save XML Template",
                FileName = "participants_template.xml"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    await _xmlImportService.CreateSampleXmlFileAsync(saveFileDialog.FileName);
                    MessageBox.Show($"XML template has been saved!\n\nPath: {saveFileDialog.FileName}",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating template:\n{ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public async Task CreateSampleExcelFileAsync()
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Title = "Save Excel Template",
                FileName = "participants_template.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    await _excelImportService.CreateSampleExcelFileAsync(saveFileDialog.FileName);
                    MessageBox.Show($"Excel template has been saved!\n\nPath: {saveFileDialog.FileName}",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating template:\n{ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public async Task ImportDataAsync(CategoryManager categoryManager, ObservableCollection<Participant> allParticipants)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                Title = "Import Tournament Data"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var importedCategories = await _jsonService.LoadTournamentDataAsync(openFileDialog.FileName);

                    if (importedCategories != null && importedCategories.Any())
                    {
                        categoryManager.DefinedCategories.Clear();
                        categoryManager.DefinedCategories.AddRange(importedCategories);

                        allParticipants.Clear();
                        foreach (var category in importedCategories)
                        {
                            foreach (var participant in category.Participants)
                            {
                                if (!allParticipants.Any(p => p.Id == participant.Id))
                                {
                                    allParticipants.Add(participant);
                                }
                            }
                        }

                        MessageBox.Show($"Successfully imported {importedCategories.Count} categories and {allParticipants.Count} participants!",
                            "Import Completed", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("File does not contain any data.",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error importing data:\n{ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
