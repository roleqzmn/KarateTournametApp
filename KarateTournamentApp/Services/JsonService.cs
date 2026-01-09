using KarateTournamentApp.Models;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KarateTournamentApp.Services
{
    public class JsonService
    {
         private JsonSerializerOptions GetOptions()
         {
            return new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
         }

        public void SaveTournamentData(string filePath, List<Category> categories)
        {
            string jsonString = JsonSerializer.Serialize(categories, GetOptions());

            File.WriteAllText(filePath, jsonString);
        }

        public List<Category> LoadTournamentData(string filePath)
        {
            if (!File.Exists(filePath)) return new List<Category>();

            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Category>>(jsonString, GetOptions());
        }
    }
}