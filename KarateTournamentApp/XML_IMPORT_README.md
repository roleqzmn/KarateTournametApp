# Participant Import - Instructions

## Supported Formats

The application supports two import formats:
1. **XML** - `.xml` files
2. **Excel** - `.xlsx` and `.xls` files

Choose the format that best suits your needs. Both formats support the same data structure.

---

## XML Import Format

## XML File Format

The XML file must have the following structure:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Participants>
  <Participant>
    <FirstName>John</FirstName>
    <LastName>Smith</LastName>
    <Age>25</Age>
    <Club>ABC Karate Club</Club>
    <Belt>Kyu5</Belt>
    <Sex>Male</Sex>
    <Categories>
      <Category>Kata</Category>
      <Category>Kumite</Category>
    </Categories>
  </Participant>
  <Participant>
    <FirstName>Anna</FirstName>
    <LastName>Johnson</LastName>
    <Age>22</Age>
    <Club></Club>
    <Belt>Kyu3</Belt>
    <Sex>Female</Sex>
    <Categories>
      <Category>Kata</Category>
      <Category>Kihon</Category>
    </Categories>
  </Participant>
</Participants>
```

## Field Description

### Required fields:
- **FirstName** - Participant's first name
- **LastName** - Participant's last name
- **Age** - Age (integer > 0)
- **Belt** - Belt rank (see possible values below)
- **Sex** - Gender (see possible values below)
- **Categories** - List of categories (minimum one)

### Optional fields:
- **Club** - Club name (can be empty)

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

1. **Generate template**: Use the "Create XML Template" button to generate a sample XML file
2. **Fill in data**: Edit the generated file by adding participants
3. **Import**: Use the "Import XML" button to import participants
4. **Note**: Before importing, make sure that "Divide by age" or "Divide by belt" options are selected

## Validation

The system will automatically skip entries that:
- Have incomplete required fields
- Have invalid values (e.g., incorrect age, non-existent belt)
- Are duplicates (same first and last name)
- Don't have any category

After import, you will see a message with the number of successfully imported participants and the number of skipped entries.

---

## Excel Import Format

For detailed Excel import instructions, see [EXCEL_IMPORT_README.md](EXCEL_IMPORT_README.md)

### Quick Overview - Excel Format:

The Excel file should have columns:
- **A**: FirstName
- **B**: LastName
- **C**: Age
- **D**: Club (optional)
- **E**: Belt
- **F**: Sex
- **G+**: Categories (one or more columns)

First row must contain headers.

### Application Usage - Excel:

1. **Generate template**: Use the "Create Excel Template" button
2. **Fill in data**: Edit the file, keeping the header row
3. **Import**: Use the "Import Excel" button
4. **Note**: Ensure "Divide by age" or "Divide by belt" is selected before import

---

## Choosing Between XML and Excel

- **Use XML** if you prefer text-based formats or need to generate files programmatically
- **Use Excel** if you prefer spreadsheet editing with familiar tools like Microsoft Excel, LibreOffice, or Google Sheets

Both formats support the same validation rules and produce identical results.
