

using System.Text.Json.Serialization;

namespace KarateTournamentApp.Models
{
    [JsonDerivedType(typeof(Match), typeDiscriminator: "match")]
    [JsonDerivedType(typeof(ShobuSanbonMatch), typeDiscriminator: "shobuSanbon")]
    public class Match
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        public Guid? Aka { get; set; }
        public Guid? Shiro { get; set; }
        public Guid? WinnerId { get; set; } = null;
        public short AkaScore { get; set; } = 0;
        public short ShiroScore { get; set; } = 0;
        public bool IsFinished { get; set; } = false;
        public bool IsDisqualification { get; set; } = false;

        public Match()
        {
        }

        public Match(Guid? aka, Guid? shiro)
        {
            Aka = aka;
            Shiro = shiro;
            if (Aka != null && Shiro == null)
            {
                WinnerId = Aka;
                IsFinished = true;
            }
            else if (Shiro != null && Aka == null)
            {
                WinnerId = Shiro;
                IsFinished = true;
            }
        }
    }

    public class ShobuSanbonMatch : Match
    {
        double time = 3;
        int penaltyAka = 0;
        int penaltyShiro = 0;

        public ShobuSanbonMatch() : base()
        {
        }

        public ShobuSanbonMatch(Guid? aka, Guid? shiro) : base(aka, shiro)
        {
        }
    }
}
