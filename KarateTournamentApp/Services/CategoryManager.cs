using System.Collections.Generic;
using System.Linq;
using System.Text;
using KarateTournamentApp.Models;

namespace KarateTournamentApp.Services
{
    public class CategoryManager
    {
        public List<Category> DefinedCategories { get; set; } = new List<Category>();

        public void AssignParticipant(Participant p)
        {
            foreach (var categoryType in p.Categories)
            {
                var targetCategory = DefinedCategories.First(c =>
                    p.Age >= c.MinAge &&
                    p.Age <= c.MaxAge &&
                    c.AllowedBelts.Contains(p.Belt) &&
                    (p.Sex == c.Sex || c.Sex == Sex.Unisex) &&
                    categoryType == c.CategoryType);

                if (targetCategory != null)
                {
                    targetCategory.Participants.Add(p);
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(p.Belt.ToString());
                    sb.Append(p.Age.ToString());
                    string name = sb.ToString();
                    DefinedCategories.Add(new Category(name, p.Belt, categoryType, p.Sex, p.Age));
                }
            }
        }
    }
}