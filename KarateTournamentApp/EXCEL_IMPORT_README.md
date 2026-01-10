# Excel Participant Import - Instructions

## Excel File Format

The Excel file should have the following column structure:

| Column | Header | Required | Description |
|--------|--------|----------|-------------|
| A | FirstName | Yes | Participant's first name |
| B | LastName | Yes | Participant's last name |
| C | Age | Yes | Age (integer > 0) |
| D | Club | No | Club name (optional) |
| E | Belt | Yes | Belt rank |
| F | Sex | Yes | Gender |
| G+ | Category1, Category2, etc. | Yes (min 1) | Categories (one or more columns) |

### Example:

| FirstName | LastName | Age | Club | Belt | Sex | Category1 | Category2 | Category3 |
|-----------|----------|-----|------|------|-----|-----------|-----------|-----------|
| John | Smith | 25 | ABC Club | Kyu5 | Male | Kata | Kumite | |
| Anna | Johnson | 22 | | Kyu3 | Female | Kata | Kihon | |
| Peter | Brown | 18 | XYZ Dojo | Kyu7 | Male | Kumite | Grappling | |

## Important Notes

1. **First row** must contain headers (will be skipped during import)
2. **Required fields**: FirstName, LastName, Age, Belt, Sex, and at least one Category
3. **Optional fields**: Club (can be empty)
4. **Categories**: You can have multiple category columns. The import stops reading categories when it encounters an empty cell.

## Possible Values

### Belt (Rank):
- `Kyu10`, `Kyu9`, `Kyu8`, `Kyu7`, `Kyu6`, `Kyu5`, `Kyu4`, `Kyu3`, `Kyu2`, `Kyu1`, `Dan`

### Sex (Gender):
- `Male` - Male
- `Female` - Female
- `Unisex` - Mixed

### Category:
- `Kata` - Kata
- `Kumite` - Kumite
- `Kihon` - Kihon
- `KobudoShort` - Kobudo (short weapon)
- `KobudoLong` - Kobudo (long weapon)
- `Grappling` - Grappling

## Application Usage

1. **Generate template**: Click "Create Excel Template" button to generate a sample Excel file
2. **Fill in data**: Edit the generated file by adding participants (keep the header row)
3. **Import**: Click "Import Excel" button to import participants
4. **Note**: Before importing, make sure that "Divide by age" or "Divide by belt" options are selected

## Validation

The system will automatically skip rows that:
- Have incomplete required fields
- Have invalid values (e.g., incorrect age, non-existent belt)
- Are duplicates (same first and last name)
- Don't have any category

After import, you will see a message with the number of successfully imported participants and the number of skipped entries.

## Tips

- The generated template includes an "Instructions" sheet with quick reference information
- You can add as many category columns as needed
- Leave cells empty for optional fields (Club) or unused category columns
- Use Excel's data validation features to prevent input errors
