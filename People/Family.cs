using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProceduralFamilyTree
{
    public class Family
    {
        private Person husband;
        private Person wife;
        private DateTime marriageDate = DateTime.MinValue;
        private List<Person> children;
        private static int familyIDCounter = 0;
        private int familyID = 0;

        public Person Husband { get => husband; set => husband = value; }
        public Person Wife { get => wife; set => wife = value; }
        public DateTime MarriageDate { get => marriageDate; set => marriageDate = value; }
        public List<Person> Children { get => children; set => children = value; }
        public int FamilyID { get => familyID; set => familyID = value; }

        /// <summary>
        /// Constructor used if you want to set all spouses with marriage date.
        /// </summary>
        /// <param name="spouse1"></param>
        /// <param name="spouse2"></param>
        /// <param name="marriageDate"></param>
        private Family(Person spouse1, Person spouse2, DateTime marriageDate)
        {
            if (marriageDate.Year - spouse1.BirthDate.Year >= Utilities.MinMarriageAge && marriageDate.Year - spouse2.BirthDate.Year >= Utilities.MinMarriageAge)
            {
                Husband = spouse1;
                Wife = spouse2;
                Children = new List<Person>();
                MarriageDate = marriageDate;
                Husband.Family = this;
                Wife.Family = this;
                FamilyID = GetNextFamilyID();
            }
            else
            {
                throw new System.ArgumentException("One spouse not of marriage age", nameof(spouse1));
            }
        }

        public static Family CreateNewRandomFamily(int year = 0)
        {
            year = year == 0 ? Utilities.RandomNumber(1950, 1850) : year - Utilities.MinMarriageAge - 3;
            Person husband = new(new Utilities.RandomDateTime(year).Next(), 'm');
            Person wife = new(new Utilities.RandomDateTime(husband.BirthDate.Year, 5).Next(), 'f', husband);

            Family family = Family.CreateFamily(husband, wife);
            family.Husband.PersonNumber = "1";
            family.CreateChildren();
            family.AssignPersonNumbers(family.Husband);

            return family;
        }

        public static Family? CreateFamily(Person spouse1, Person? spouse2, DateTime? marriageDate = null)
        {

            if (spouse2 != null && spouse1.Gender == spouse2.Gender) // Same genders cannot marry at this point, will implement
            {
                return null;
            }
            var children = new List<Person>();
            int startingYear = spouse1.BirthDate.Year > spouse2.BirthDate.Year ? spouse1.BirthDate.Year : spouse2.BirthDate.Year;
            int marriageAge = Utilities.MinMarriageAge + Utilities.RandomNumber(6, 1);
            if (marriageAge > spouse1.Age() || marriageAge > spouse2.Age())
            {
                marriageAge = spouse1.Age() > spouse2.Age() ? spouse2.Age() : spouse1.Age();
            }
            int marriageYear = startingYear + marriageAge;
            if (spouse1.DeathDate.Year <= marriageYear && spouse1.DeathDate != DateTime.MinValue)
            {
                marriageYear = spouse1.DeathDate.Year - 1;
            }
            else if (spouse2.DeathDate.Year <= marriageYear && spouse2.DeathDate != DateTime.MinValue)
            {
                marriageYear = spouse2.DeathDate.Year - 1;
            }
            DateTime finalMarriageDate = (DateTime)(marriageDate != null ? marriageDate : new Utilities.RandomDateTime(marriageYear).Next());
            //Husband.Family = this;
            //Wife.Family = this;
            if (spouse1.Age(finalMarriageDate.Year) >= Utilities.MinMarriageAge && spouse2.Age(finalMarriageDate.Year) >= Utilities.MinMarriageAge)
            {
                if (spouse1.Gender == 'm')
                {
                    return new Family(spouse1, spouse2, finalMarriageDate);
                }
                else
                {
                    return new Family(spouse2, spouse1, finalMarriageDate);
                }
            }
            return null;
        }

        public void CreateGenerations(int generations = 0)
        {
            if (generations > 0)
            {
                for (int i = 0; i < generations; i++)
                {
                    foreach (Person child in Children)
                    {
                        if (child.Age() > Utilities.MinMarriageAge && child.Age(DateTime.Now.Year) > Utilities.MinMarriageAge)
                        {
                            Person spouse = new Person(child);
                            child.Family = CreateFamily(child, spouse);
                            if (child.Family != null)
                            {
                                child.Family.CreateChildren();
                                child.Family.CreateGenerations(generations - 1);
                            }
                        }
                    }
                }
            }
        }


        public void AddChild(Person child)
        {
            child.BirthFamily = this;
            Children.Add(child);
        }

        /// <summary>
        /// Create a new random Person and add it to the family as a child
        /// </summary>
        public void CreateChild()
        {
            var birthYear = MarriageDate.Year;
            if (Children.Count > 0)
            {
                birthYear = Children.Last().BirthDate.Year;
            }
            birthYear += Utilities.RandomNumber(Utilities.YearsBetweenChildren, 2);
            if (birthYear < DateTime.Now.Year)
            {
                if (birthYear - Wife.BirthDate.Year <= 40 && Wife.WasAlive(birthYear) && Husband.WasAlive(birthYear))
                {
                    var birthDate = new Utilities.RandomDateTime(birthYear).Next();
                    AddChild(new Person(Husband.LastName, birthDate, this));
                }
            }
        }

        public void CreateChildren(int maxNumChildren = 0)
        {
            maxNumChildren = maxNumChildren == 0 ? Utilities.MaxNumberOfKids : maxNumChildren;
            for (var i = 0; i < Utilities.WeightedRandomNumber(0.8, 0.2, maxNumChildren, 0); i++)
            {
                CreateChild();
            }

        }

        public int NumberOfMarriableChildren()
        {
            return Children.Count(child => child.Age() >= Utilities.MinMarriageAge);
        }

        public int NumberOfDescendants(Person? descendant = null)
        {
            int num = 0;

            var children = descendant == null ? Children : descendant.Family.Children;
            foreach (Person child in children)
            {
                num++; // Increment count for each immediate child
                if (child.HasOwnFamily)
                {
                    num += NumberOfDescendants(child); // Recursively count descendants of each child
                }
            }

            return num;
        }

        /// <summary>
        /// Used for the console app to print out the family in an easy to read format
        /// </summary>
        /// <param name="format"></param>
        /// <param name="tabAmount"></param>
        /// <returns></returns>
        public string ListFamily(string format = "sentence", int tabAmount = 0)
        {
            var family = String.Empty;
            format = format.ToLower();
            string tabs = String.Empty;
            for (int x = 0; x < tabAmount; x++)
            {
                tabs += "  ";
            }
            if (format == "sentence")
            {
                string separator = Children.Count > 0 ? ", " : " and ";
                family = Husband.ToString() + separator + Wife.ToString();
                if (Children.Count > 0)
                {
                    family += Children.Count == 1 ? " and " : ", ";
                    family += ListChildren(format);
                }
            }
            else if (format == "list")
            {
                family = tabs + Husband.ToString() + "\r\n" + tabs + Wife.ToString() + "\r\n";
                family += tabs + "Married: " + MarriageDate.ToString("d") + "\r\n";
                family += tabs + "=== " + Children.Count;
                family += Children.Count == 1 ? " Child" : " Children";
                family += ": =====================\r\n";
                if (Children.Count > 0)
                {
                    family += ListChildren(format, tabAmount);
                }
            }
            return family;
        }

        /// <summary>
        /// Used for the console app to show the children in a easy-to-read format
        /// </summary>
        /// <param name="format"></param>
        /// <param name="tabAmount"></param>
        /// <returns></returns>
        public string ListChildren(string format = "sentence", int tabAmount = 0)
        {
            string tabs = String.Empty;
            for (int x = 0; x < tabAmount; x++)
            {
                tabs += "  ";
            }
            string kids = String.Empty;
            format = format.ToLower();
            if (format == "sentence")
            {
                kids = String.Join(", ", Children);
                var lastComma = kids.LastIndexOf(',');
                if (lastComma != -1) kids = kids.Insert(lastComma + 1, " and");
            }
            else if (format == "list")
            {
                kids = tabs + String.Join("\r\n" + tabs, Children);
            }

            return kids;
        }

        public List<string> ChildrensNames()
        {
            var names = new List<string>();

            foreach (var child in Children)
            {
                names.Add(child.FirstName);
            }

            return names;
        }

        public void AssignPersonNumbers(Person person, string parentNumber = "")
        {
            if (string.IsNullOrEmpty(parentNumber))
            {
                person.PersonNumber = "1";
            }
            else
            {
                person.PersonNumber = parentNumber;
            }

            if (person.Family != null && person.HasOwnFamily)
            {
                for (int i = 0; i < person.Family.Children.Count; i++)
                {
                    AssignPersonNumbers(person.Family.Children[i], person.PersonNumber + "." + (i + 1).ToString());
                }
            }
        }

        public static int GetNextFamilyID()
        {
            return ++familyIDCounter;
        }

    }
}