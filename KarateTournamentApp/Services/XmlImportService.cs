using KarateTournamentApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace KarateTournamentApp.Services
{
    public class XmlImportService
    {
        /// <summary>
        /// Imports from XML.
        /// XML format: 
        /// <Participants>
        ///   <Participant>
        ///     <FirstName>Jan</FirstName>
        ///     <LastName>Kowalski</LastName>
        ///     <Age>25</Age>
        ///     <Club>Klub ABC</Club> (optional)
        ///     <Belt>Kyu5</Belt>
        ///     <Sex>Male</Sex>
        ///     <Categories>
        ///       <Category>Kata</Category>
        ///       <Category>Kumite</Category>
        ///     </Categories>
        ///   </Participant>
        /// </Participants>
        /// </summary>
        public List<Participant> ImportParticipantsFromXml(string filePath)
        {
            var participants = new List<Participant>();

            try
            {
                XDocument doc = XDocument.Load(filePath);
                var participantElements = doc.Descendants("Participant");

                foreach (var element in participantElements)
                {
                    try
                    {
                        var firstName = element.Element("FirstName")?.Value;
                        var lastName = element.Element("LastName")?.Value;
                        var ageStr = element.Element("Age")?.Value;
                        var club = element.Element("Club")?.Value; 
                        var beltStr = element.Element("Belt")?.Value;
                        var sexStr = element.Element("Sex")?.Value;

                        if (string.IsNullOrWhiteSpace(firstName) ||
                            string.IsNullOrWhiteSpace(lastName) ||
                            string.IsNullOrWhiteSpace(ageStr) ||
                            string.IsNullOrWhiteSpace(beltStr) ||
                            string.IsNullOrWhiteSpace(sexStr))
                        {
                            continue;
                        }

                        if (!int.TryParse(ageStr, out int age) || age <= 0)
                        {
                            continue;
                        }

                        if (!Enum.TryParse<Belts>(beltStr, true, out Belts belt))
                        {
                            continue;
                        }

                        if (!Enum.TryParse<Sex>(sexStr, true, out Sex sex))
                        {
                            continue;
                        }

                        var categoriesElement = element.Element("Categories");
                        var categories = new List<CategoryType>();

                        if (categoriesElement != null)
                        {
                            foreach (var categoryElement in categoriesElement.Elements("Category"))
                            {
                                var categoryStr = categoryElement.Value;
                                if (Enum.TryParse<CategoryType>(categoryStr, true, out CategoryType categoryType))
                                {
                                    categories.Add(categoryType);
                                }
                            }
                        }

                        if (!categories.Any())
                        {
                            continue;
                        }

                        var participant = new Participant(
                            firstName,
                            lastName,
                            age,
                            belt,
                            sex,
                            categories,
                            club
                        );

                        participants.Add(participant);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error parsing XML file: {ex.Message}", ex);
            }

            return participants;
        }

        /// <summary>
        /// Creates sample XML file
        /// </summary>
        public void CreateSampleXmlFile(string filePath)
        {
            var xml = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("Participants",
                    new XElement("Participant",
                        new XElement("FirstName", "Jan"),
                        new XElement("LastName", "Kowalski"),
                        new XElement("Age", "25"),
                        new XElement("Club", "Klub ABC"),
                        new XElement("Belt", "Kyu5"),
                        new XElement("Sex", "Male"),
                        new XElement("Categories",
                            new XElement("Category", "Kata"),
                            new XElement("Category", "Kumite")
                        )
                    ),
                    new XElement("Participant",
                        new XElement("FirstName", "Anna"),
                        new XElement("LastName", "Nowak"),
                        new XElement("Age", "22"),
                        new XElement("Club", ""),
                        new XElement("Belt", "Kyu3"),
                        new XElement("Sex", "Female"),
                        new XElement("Categories",
                            new XElement("Category", "Kata"),
                            new XElement("Category", "Kihon")
                        )
                    )
                )
            );

            xml.Save(filePath);
        }
    }
}
