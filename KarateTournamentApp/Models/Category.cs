using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
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

        public Category(string name, Belts belt, CategoryType type, Sex sex = Sex.Unisex, int? age = null)
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

        /// <summary>
        /// Initializes a bracket, semi-randomly
        /// </summary>
        public void InitializeBracket()
        {
            BracketMatches = new List<Match>();
            var size = Participants.Count;
            int leafs = 1;
            while (leafs < size) leafs = 2 * leafs;

            int totalMatches = 2 * leafs - 1;
            for (int j = 0; j < totalMatches; j++)
            {
                BracketMatches.Add(new Match());
            }

            var shuffledParticipants = Participants.OrderBy(a => Guid.NewGuid()).ToList(); //pseudo shuffling by guid

            for (int i = 0; i < size; i++)
            {
                int bracketIndex = (leafs - 1) + i;
                BracketMatches[bracketIndex].Aka = shuffledParticipants[i].Id;

                if (BracketMatches[bracketIndex].IsFinished)
                {
                    PromoteWinner(bracketIndex);
                }
            }
        }
    }

    public class ShobuSanbonCategory : Category
    {
        public ShobuSanbonCategory(string name, Belts belt, CategoryType type,  Sex sex = Sex.Unisex, int? age = null) : base(name, belt, type, sex, age)
        {  
        }
        /// <summary>
        /// Initializes a bracket, semi-randomly
        /// </summary>
        public void InitializeBracket()
        {
            BracketMatches = new List<Match>();
            var size = Participants.Count;
            int leafs = 1;
            while (leafs < size) leafs = 2 * leafs;

            int totalMatches = 2 * leafs - 1;
            for (int j = 0; j < totalMatches; j++)
            {
                BracketMatches.Add(new ShobuSanbonMatch());
            }

            var shuffledParticipants = Participants.OrderBy(a => Guid.NewGuid()).ToList(); //pseudo shuffling by guid

            for (int i = 0; i < size; i++)
            {
                int bracketIndex = (leafs - 1) + i;
                BracketMatches[bracketIndex].Aka = shuffledParticipants[i].Id;

                if (BracketMatches[bracketIndex].IsFinished)
                {
                    PromoteWinner(bracketIndex);
                }
            }
        }
    }
}