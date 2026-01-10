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

        public void ImportParticipantsFromXml(CategoryManager categoryManager, ObservableCollection<Participant> allParticipants, bool divideByBelt, bool divideByAge)
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
                    var importedParticipants = _xmlImportService.ImportParticipantsFromXml(openFileDialog.FileName);

                    if (importedParticipants != null && importedParticipants.Any())
                    {
                        int addedCount = 0;
                        foreach (var participant in importedParticipants)
                        {
                            // Check if participant already exists (by first and last name)
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

        public void ImportParticipantsFromExcel(CategoryManager categoryManager, ObservableCollection<Participant> allParticipants, bool divideByBelt, bool divideByAge)
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
                    var importedParticipants = _excelImportService.ImportParticipantsFromExcel(openFileDialog.FileName);

                    if (importedParticipants != null && importedParticipants.Any())
                    {
                        int addedCount = 0;
                        foreach (var participant in importedParticipants)
                        {
                            // Check if participant already exists (by first and last name)
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

        public void CreateSampleXmlFile()
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
                    _xmlImportService.CreateSampleXmlFile(saveFileDialog.FileName);
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

        public void CreateSampleExcelFile()
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
                    _excelImportService.CreateSampleExcelFile(saveFileDialog.FileName);
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

        public void ImportData(CategoryManager _categoryManager, ObservableCollection<Participant> AllParticipants)
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


                        MessageBox.Show($"Successfully imported {importedCategories.Count} categories and {AllParticipants.Count} participants!",
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
