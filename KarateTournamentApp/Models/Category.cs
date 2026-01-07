using System.Collections.Generic;
using System.Security.Policy;

namespace KarateTournamentApp.Models
{
    public enum CategoryType
    {
        Kata,
        Kumite,
        Kihon,
        KobudoShort,
        KobudoLong,
        Grappling
    }
    public class Category
    {
        public string Name { get; set; }
        public List<Participant> Participants { get; set; } = new List<Participant>();
        public List<Belts> AllowedBelts { get; set; } = new List<Belts> { };
        public List<Match> BracketMatches { get; set; } = new List<Match>();
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public bool IsFinished { get; set; } = false;
        public Sex Sex { get; set; }
        public CategoryType CategoryType { get; set; }

        public Category(string name, Belts belt, CategoryType type, Sex sex = Sex.Unisex, int? age=null)
        {
            Name = name;
            MinAge = age;
            MaxAge = age;
            AllowedBelts.Add(belt);
            Sex = sex;
            CategoryType = type;
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

            foreach (var belt in otherCategory.AllowedBelts)
            {
                if (!AllowedBelts.Contains(belt))
                {
                    AllowedBelts.Add(belt);
                }
            }
            this.Name = $"{this.Name} + {otherCategory.Name}";
        }

        public void PromoteWinner(int currentMatchIndex)
        {
            if (currentMatchIndex == 0) { IsFinished = true; return; }

            int parentIndex = (currentMatchIndex - 1) / 2;
            var winnerId = BracketMatches[currentMatchIndex].WinnerId;
            if (currentMatchIndex % 2 != 0) 
                BracketMatches[parentIndex].Aka = winnerId;
            else
                BracketMatches[parentIndex].Shiro = winnerId;
        }
    }
}