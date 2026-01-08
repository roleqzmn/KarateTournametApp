using KarateTournamentApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace KarateTournamentApp.Services
{
    public class CategoryManager
    {
        public List<Category> DefinedCategories { get; set; } = new List<Category>();

        public void AssignParticipant(Participant p)
        {
            foreach (var categoryType in p.Categories)
            {
                var targetCategory = DefinedCategories.FirstOrDefault(c =>
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
                    if(p.Age>=18)
                    {
                        var cat = new Category("Senior", p.Belt, categoryType, p.Sex, 18);
                        if (categoryType == CategoryType.Kumite)
                        {
                            cat = new ShobuSanbonCategory("Senior", p.Belt, categoryType, p.Sex, 18);
                        }
                        cat.MaxAge = 99;
                        cat.Participants.Add(p);
                        DefinedCategories.Add(cat);
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(p.Belt.ToString());
                        sb.Append(" ");
                        sb.Append(p.Age.ToString());
                        string name = sb.ToString();
                        var cat = new Category(name, p.Belt, categoryType, p.Sex, p.Age);
                        if (categoryType == CategoryType.Kumite)
                        {
                            cat = new ShobuSanbonCategory(name, p.Belt, categoryType, p.Sex, p.Age);
                        }
                        cat.Participants.Add(p);
                        DefinedCategories.Add(cat);
                    }
                    
                }
            }
        }
    }
}