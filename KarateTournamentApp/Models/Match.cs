
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
        public bool IsFinished { get; set; } = false;

        public Match(Guid? aka=null, Guid? shiro=null)
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
}
