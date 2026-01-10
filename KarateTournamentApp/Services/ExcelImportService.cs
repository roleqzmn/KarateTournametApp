using KarateTournamentApp.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KarateTournamentApp.Services
{
    public class ExcelImportService
    {
        public ExcelImportService()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        /// <summary>
        /// Imports participants from an Excel file (.xlsx, .xls)
        /// Excel format:
        /// Column A: FirstName
        /// Column B: LastName
        /// Column C: Age
        /// Column D: Club (optional)
        /// Column E: Belt
        /// Column F: Sex
        /// Column G+: Categories (one or more columns)
        /// 
        /// First row should contain headers (will be skipped)
        /// </summary>
        public List<Participant> ImportParticipantsFromExcel(string filePath)
        {
            var participants = new List<Participant>();

            try
            {
                FileInfo fileInfo = new FileInfo(filePath);

                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension?.Rows ?? 0;

                    if (rowCount < 2)
                    {
                        return participants;
                    }

                    // Start from row 2 (skipping headers)
                    for (int row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            var firstName = worksheet.Cells[row, 1].Text?.Trim();
                            var lastName = worksheet.Cells[row, 2].Text?.Trim();
                            var ageStr = worksheet.Cells[row, 3].Text?.Trim();
                            var club = worksheet.Cells[row, 4].Text?.Trim(); // Optional
                            var beltStr = worksheet.Cells[row, 5].Text?.Trim();
                            var sexStr = worksheet.Cells[row, 6].Text?.Trim();

                            // Validate required fields
                            if (string.IsNullOrWhiteSpace(firstName) ||
                                string.IsNullOrWhiteSpace(lastName) ||
                                string.IsNullOrWhiteSpace(ageStr) ||
                                string.IsNullOrWhiteSpace(beltStr) ||
                                string.IsNullOrWhiteSpace(sexStr))
                            {
                                continue; // Skip incomplete entries
                            }

                            if (!int.TryParse(ageStr, out int age) || age <= 0)
                            {
                                continue;
                            }

                            if (!Enum.TryParse<Belts>(beltStr, true, out Belts belt))
                            {
                                continue;
                            }

                            if (!Enum.TryParse<Sex>(sexStr, true, out Sex sex))
                            {
                                continue;
                            }

                            // Parse categories (columns from G onwards)
                            var categories = new List<CategoryType>();
                            int categoryColumn = 7; 

                            while (true)
                            {
                                var categoryStr = worksheet.Cells[row, categoryColumn].Text?.Trim();
                                
                                if (string.IsNullOrWhiteSpace(categoryStr))
                                {
                                    break; // End of categories for this participant
                                }

                                if (Enum.TryParse<CategoryType>(categoryStr, true, out CategoryType categoryType))
                                {
                                    categories.Add(categoryType);
                                }

                                categoryColumn++;

                                // Protection against infinite loop
                                if (categoryColumn > 20)
                                {
                                    break;
                                }
                            }

                            // If no categories, skip participant
                            if (!categories.Any())
                            {
                                continue;
                            }

                            var participant = new Participant(
                                firstName,
                                lastName,
                                age,
                                belt,
                                sex,
                                categories,
                                club
                            );

                            participants.Add(participant);
                        }
                        catch (Exception)
                        {
                            // Skip invalid row and continue
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error parsing Excel file: {ex.Message}", ex);
            }

            return participants;
        }

        /// <summary>
        /// Creates a sample Excel file with template
        /// </summary>
        public void CreateSampleExcelFile(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Participants");

                // Headers
                worksheet.Cells[1, 1].Value = "FirstName";
                worksheet.Cells[1, 2].Value = "LastName";
                worksheet.Cells[1, 3].Value = "Age";
                worksheet.Cells[1, 4].Value = "Club";
                worksheet.Cells[1, 5].Value = "Belt";
                worksheet.Cells[1, 6].Value = "Sex";
                worksheet.Cells[1, 7].Value = "Category1";
                worksheet.Cells[1, 8].Value = "Category2";
                worksheet.Cells[1, 9].Value = "Category3";

                // Style headers
                using (var range = worksheet.Cells[1, 1, 1, 9])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                }

                // Add instructions sheet
                ExcelWorksheet instructionsSheet = package.Workbook.Worksheets.Add("Instructions");
                instructionsSheet.Cells[1, 1].Value = "EXCEL IMPORT FORMAT - INSTRUCTIONS";
                instructionsSheet.Cells[1, 1].Style.Font.Bold = true;
                instructionsSheet.Cells[1, 1].Style.Font.Size = 14;

                int row = 3;
                instructionsSheet.Cells[row++, 1].Value = "Column A: FirstName (required)";
                instructionsSheet.Cells[row++, 1].Value = "Column B: LastName (required)";
                instructionsSheet.Cells[row++, 1].Value = "Column C: Age (required, integer > 0)";
                instructionsSheet.Cells[row++, 1].Value = "Column D: Club (optional)";
                instructionsSheet.Cells[row++, 1].Value = "Column E: Belt (required)";
                instructionsSheet.Cells[row++, 1].Value = "Column F: Sex (required)";
                instructionsSheet.Cells[row++, 1].Value = "Column G+: Categories (at least one required)";

                row += 2;
                instructionsSheet.Cells[row++, 1].Value = "Belt values: Kyu10, Kyu9, Kyu8, Kyu7, Kyu6, Kyu5, Kyu4, Kyu3, Kyu2, Kyu1, Dan";
                instructionsSheet.Cells[row++, 1].Value = "Sex values: Male, Female, Unisex";
                instructionsSheet.Cells[row++, 1].Value = "Category values: Kata, Kumite, Kihon, KobudoShort, KobudoLong, Grappling";

                // Auto-fit column widths
                worksheet.Cells.AutoFitColumns();
                instructionsSheet.Cells.AutoFitColumns();

                package.Save();
            }
        }
    }
}
