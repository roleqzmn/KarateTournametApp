
namespace KarateTournamentApp.Models
{
    public class Match
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        public Guid? Aka { get; set; }
        public Guid? Shiro { get; set; }
        public Guid? WinnerId { get; set; } = null;
        public short AkaScore { get; set; } = 0;
        public short ShiroScore { get; set; } = 0;
        public bool IsSolo { get; set; } = false;

        public Match(Guid? aka, Guid? shiro)
        {
            if (aka == null)
            {
                Shiro = shiro;
                WinnerId = shiro;
                IsSolo = true;
            }
            if (shiro == null)
            {
                Aka = aka;
                WinnerId = aka;
                IsSolo = true;
            }
        }
    }
}
