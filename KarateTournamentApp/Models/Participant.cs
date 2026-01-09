
using System.Text.Json.Serialization;

namespace KarateTournamentApp.Models
{
    public enum Belts
    {
        Kyu10 = 10,
        Kyu9 = 9,
        Kyu8 = 8,
        Kyu7 = 7,
        Kyu6 = 6,
        Kyu5 = 5,
        Kyu4 = 4,
        Kyu3 = 3,
        Kyu2 = 2,
        Kyu1 = 1,
        Dan=0
    }
    public enum Sex
    {
        Male = 1,
        Female = 2,
        Unisex = 3
    }
    public class Participant
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FirstName { get ; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string? Club { get; set; }
        public Belts Belt { get; set; }
        public List<Guid> MatchHistory { get; set; } = new List<Guid>();
        public string FullName => $"{FirstName} {LastName}";
        public Sex Sex { get ; set; }
        public List<CategoryType> Categories { get; set; }

        public Participant(string firstName, string lastName, int age, Belts belt, Sex sex, List<CategoryType> categories, string? club = null)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            Belt = belt;
            Club = club;
            Sex = sex;
            Categories = categories;
        }

        public Participant()
        {
        }
    }
}
