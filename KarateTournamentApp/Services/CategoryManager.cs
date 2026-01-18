using KarateTournamentApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace KarateTournamentApp.Services
{
    public class CategoryManager
    {
        public List<Category> DefinedCategories { get; set; } = new List<Category>();
        private List<Belts> AllBelts { get; } = Enum.GetValues<Belts>().ToList();
        public void AssignParticipant(Participant p, bool DivideByBelt, bool DivideByAge) 
        {
            if (p.Age >= 18) AssignSenior(p);
            if (DivideByAge) AssignByAge(p);
            if (DivideByBelt) AssignByBelt(p);
            if (DivideByBelt && DivideByAge) AssignByBoth(p);
        }
        public void AssignSenior(Participant p)
        {
            foreach(var categoryType in p.Categories)
            {
                var targetCategory = DefinedCategories.FirstOrDefault(c =>
                   c.MaxAge < 18 &&
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
                    string name = sb.ToString();
                    var cat = new Category(name, p.Belt, categoryType, p.Sex, 1, 17);
                    if (categoryType == CategoryType.Kumite)
                    {
                        cat = new ShobuSanbonCategory(name, p.Belt, categoryType, p.Sex, 1, 17);
                    }
                    cat.Participants.Add(p);
                    DefinedCategories.Add(cat);
                }
            }
        }
        public void AssignByBelt(Participant p)
        {
            foreach (var categoryType in p.Categories)
            {
                var cat = new Category("Senior", AllBelts, categoryType, p.Sex, 18, 99);
                if (categoryType == CategoryType.Kumite)
                {
                    cat = new ShobuSanbonCategory("Senior", AllBelts, categoryType, p.Sex, 18, 99);
                }
                cat.Participants.Add(p);
                DefinedCategories.Add(cat);
            }
        }
        public void AssignByAge(Participant p)
        {
            foreach (var categoryType in p.Categories)
            {
                var targetCategory = DefinedCategories.FirstOrDefault(c =>
                    p.Age >= c.MinAge &&
                    p.Age <= c.MaxAge &&
                    (p.Sex == c.Sex || c.Sex == Sex.Unisex) &&
                    categoryType == c.CategoryType);

                if (targetCategory != null)
                {
                    targetCategory.Participants.Add(p);
                }
                else
                { 
                    StringBuilder sb = new StringBuilder();
                    sb.Append(p.Age.ToString());
                    string name = sb.ToString();
                    var cat = new Category(name, AllBelts, categoryType, p.Sex, p.Age, p.Age);
                    if (categoryType == CategoryType.Kumite)
                    {
                        cat = new ShobuSanbonCategory(name, AllBelts, categoryType, p.Sex, p.Age, p.Age);
                    }
                    cat.Participants.Add(p);
                    DefinedCategories.Add(cat);
                }
            }
        }
        public void AssignByBoth(Participant p)
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
                    StringBuilder sb = new StringBuilder();
                    sb.Append(p.Belt.ToString());
                    sb.Append(" ");
                    sb.Append(p.Age.ToString());
                    string name = sb.ToString();
                    var cat = new Category(name, p.Belt, categoryType, p.Sex, p.Age, p.Age);
                    if (categoryType == CategoryType.Kumite)
                    {
                        cat = new ShobuSanbonCategory(name, p.Belt, categoryType, p.Sex, p.Age, p.Age);
                    }
                    cat.Participants.Add(p);
                    DefinedCategories.Add(cat);
                }
            }
        }
        
            
    }
}