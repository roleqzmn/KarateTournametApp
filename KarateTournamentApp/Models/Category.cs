using System.Collections.Generic;

namespace KarateTournamentApp.Models
{
    public class Category
    {
        public string Name { get; set; }
        public List<Participant> Participants { get; set; } = new List<Participant>();
        public object BracketRoot { get; set; }

        public Category(string name)
        {
            Name = name;
        }
    }
}