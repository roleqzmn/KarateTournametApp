using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;

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

    [JsonDerivedType(typeof(Category), typeDiscriminator: "base")]
    [JsonDerivedType(typeof(ShobuSanbonCategory), typeDiscriminator: "shobu")]
    public class Category
    {
        public string Name { get; set; } = "";
        public ObservableCollection<Participant> Participants { get; set; } = new ObservableCollection<Participant>();
        public List<Belts> AllowedBelts { get; set; } = new List<Belts> { };
        public List<Match> BracketMatches { get; set; } = new List<Match>();
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public bool IsFinished { get; set; } = false;
        public Sex Sex { get; set; }
        public CategoryType CategoryType { get; set; }

        public Category(Belts belt, CategoryType type, Sex sex = Sex.Unisex, int? minAge = null, int? maxAge = null)
        {
            MinAge = minAge;
            MaxAge = maxAge;
            AllowedBelts.Add(belt);
            Sex = sex;
            CategoryType = type;
            GenerateName();
        }

        public Category(List<Belts> belts, CategoryType type, Sex sex = Sex.Unisex, int? minAge = null, int? maxAge = null)
        {
            MinAge = minAge;
            MaxAge = maxAge;
            AllowedBelts = belts ?? new List<Belts>();
            Sex = sex;
            CategoryType = type;
            GenerateName();
        }

        public Category()
        {
        }

        private void GenerateName()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{MinAge}-{MaxAge}");
            foreach(var belt in AllowedBelts)
            {
                sb.Append($", {belt}");
            }
            Name = sb.ToString();
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
            if (Sex != otherCategory.Sex) Sex = Sex.Unisex;
            if (otherCategory.MinAge < this.MinAge) this.MinAge = otherCategory.MinAge;
            if (otherCategory.MaxAge > this.MaxAge) this.MaxAge = otherCategory.MaxAge;

            foreach (var belt in otherCategory.AllowedBelts)
            {
                if (!AllowedBelts.Contains(belt))
                {
                    AllowedBelts.Add(belt);
                }
            }
            if (MinAge >= 18)
            {
                Name = "Senior";
            }
            else
            {
                GenerateName();
            }
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
        public ShobuSanbonCategory(Belts belt, CategoryType type,  Sex sex = Sex.Unisex, int? minAge = null, int? maxAge = null) : base(belt, type, sex, minAge, maxAge)
        {  
        }

        public ShobuSanbonCategory(List<Belts> belts, CategoryType type, Sex sex = Sex.Unisex, int? minAge = null, int? maxAge = null) : base(belts, type, sex, minAge, maxAge)
        {
        }

        public ShobuSanbonCategory() : base()
        {
        }
        /// <summary>
        /// Initializes a bracket, semi-randomly
        /// </summary>
        public new void InitializeBracket()
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