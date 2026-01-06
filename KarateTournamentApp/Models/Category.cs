using System.Collections.Generic;
using System.Security.Policy;

namespace KarateTournamentApp.Models
{
    public class Category
    {
        public string Name { get; set; }
        public List<Participant> Participants { get; set; } = new List<Participant>();
        public List<Belts> AllowedBelts { get; set; } = new List<Belts> { };
        public List<Match> BracketMatches { get; set; } = new List<Match>();
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public bool IsFinished { get; set; } = false;

        public Category(string name, Belts belt, int? age=null)
        {
            Name = name;
            MinAge = age;
            MaxAge = age;
            AllowedBelts.Add(belt);
        }

        /// <summary>
        /// Combines the passed category with the current one.
        /// </summary>
        public void MergeWith(Category otherCategory)
        {
            if (otherCategory == null) return;
            foreach (var participant in otherCategory.Participants)
            {
                if (!Participants.Any(p => p.Id == participant.Id))
                {
                    Participants.Add(participant);
                }
            }

            if (otherCategory.MinAge < this.MinAge) this.MinAge = otherCategory.MinAge;
            if (otherCategory.MaxAge > this.MaxAge) this.MaxAge = otherCategory.MaxAge;

            // 3. Połącz listy dozwolonych pasów (bez duplikatów)
            foreach (var belt in otherCategory.AllowedBelts)
            {
                if (!AllowedBelts.Contains(belt))
                {
                    AllowedBelts.Add(belt);
                }
            }

            // 4. Zmień nazwę, aby było widać, że to połączenie
            this.Name = $"{this.Name} + {otherCategory.Name}";
        }
    }
}