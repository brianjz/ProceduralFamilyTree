﻿using System.Text;

namespace ProceduralFamilyTree
{
    public class Person
    {
        private string firstName = string.Empty;
        private string middleName = string.Empty;
        private string lastName = string.Empty;
        private string suffix = string.Empty;
        private DateTime birthDate = DateTime.MinValue;
        private DateTime deathDate = DateTime.MinValue;
        private char gender;
        private Family? birthFamily = null;
        private Family? marriedFamily = null; 
        private string personNumber = string.Empty;

        public string FirstName { get => firstName; set => firstName = value; }
        public string MiddleName { get => middleName; set => middleName = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public string Suffix { get => suffix; set => suffix = value; }
        public DateTime BirthDate { get => birthDate; set => birthDate = value; }
        public DateTime DeathDate { get => deathDate; set => deathDate = value; }
        public char Gender { get => gender; set => gender = value; }
        public Family? Family { get => marriedFamily; set => marriedFamily = value; }
        public Family? BirthFamily { get => birthFamily; set => birthFamily = value; }
        public string PersonNumber { get => personNumber; set => personNumber = value; }
        public bool HasOwnFamily => Family != null;
        public int Age { get => GetAge(); }
        private List<Person> MaleAncestors { get => FindAncestorsByGender('m'); }

        /// <summary>
        /// Constructor to use to initiate all major attributes of a Person
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="birthDate"></param>
        /// <param name="gender"></param>
        public Person(string firstName, string lastName, DateTime birthDate, char gender)
        {
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
            Gender = gender;
            if (DateTime.Now.Subtract(BirthDate).TotalDays / 365 > Utilities.MaxAge)
            {
                DeathDate = new Utilities.RandomDateTime(BirthDate.AddYears(Utilities.RandomNumber(Utilities.MaxAge, 0)).Year).Next();
            }
        }
        /// <summary>
        /// Constructor to use to initiate all major attributes of a Person with random first name and gender
        /// </summary>
        /// <param name="lastName"></param>
        /// <param name="birthDate"></param>
        /// <param name="family"></param>
        public Person(string lastName, DateTime birthDate, Family family)
        {
            Gender = ChooseGender();
            BirthFamily = family;
            FirstName = Names.RandomName(Gender, "first", family, birthDate);
            MiddleName = Names.RandomName(Gender, "middle", family, birthDate);
            if(MiddleName == FirstName) {
                MiddleName = Names.RandomName(Gender, "middle", family, birthDate);
            }
            Suffix = FirstName == family.Husband.FirstName ? "Jr" : "";
            LastName = lastName;
            BirthDate = birthDate;
            DeathDate = DetermineDeathDate();
        }

        /// <summary>
        /// Constructor to use to initiate randomized Person with set birthday and gender
        /// </summary>
        /// <param name="birthDate"></param>
        /// <param name="gender"></param>
        public Person(DateTime birthDate, char gender, Person? spouse = null)
        {
            if (gender == 'f' || gender == 'm')
            {
                Gender = gender;
            }
            else if (spouse != null)
            {
                Gender = spouse.Gender == 'm' ? 'f' : 'm';
            }
            else if (gender == '?')
            {
                Gender = Utilities.RandomNumber(2) == 0 ? 'm' : 'f';
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(gender), "Gender must be M or F or ?, unless spouse is set.");
            }
            FirstName = Names.RandomName(Gender, "first", Family, birthDate);
            MiddleName = Names.RandomName(Gender, "middle", Family, birthDate);
            if(MiddleName == FirstName) {
                MiddleName = Names.RandomName(Gender, "middle", Family, birthDate);
            }
            Suffix = FirstName == Family?.Husband.FirstName ? "Jr" : "";
            LastName = Names.RandomSurname();
            BirthDate = birthDate;
            int minAge = 0;
            if (spouse != null)
            {
                // minAge = Utilities.RandomNumber(spouse.Age + 5, spouse.Age - 5);
            }
            DeathDate = DetermineDeathDate(minAge);
        }

        public Person(Person spouse)
        {
            Gender = spouse.Gender == 'm' ? 'f' : 'm';
            LastName = Names.RandomSurname();
            BirthDate = new Utilities.RandomDateTime(spouse.BirthDate.Year, 5).Next();
            FirstName = Names.RandomName(Gender, "first", null, BirthDate);
            MiddleName = Names.RandomName(Gender, "middle", null, BirthDate);
            if(MiddleName == FirstName) {
                MiddleName = Names.RandomName(Gender, "middle", null, BirthDate);
            }
            Suffix = FirstName == Family?.Husband.FirstName ? "Jr" : "";
            int minAge = Utilities.RandomNumber(spouse.Age + 5, spouse.Age - 5);
            DeathDate = DetermineDeathDate(minAge);
        }

        public DateTime DetermineDeathDate(int minAge = 0) {
            DateTime deathDate = DateTime.MinValue;
            if (DateTime.Now.Subtract(BirthDate).TotalDays / 365 > Utilities.MaxAge)
            {
                deathDate = new Utilities.RandomDateTime(BirthDate.AddYears(Utilities.WeightedRandomNumber(0.8, 0.2, Utilities.MaxAge, minAge)).Year).Next();
            }
            else
            {
                bool isDead = false;
                int curAge = minAge;
                int yearsFromNow = DateTime.Now.Year - BirthDate.Year - 1;
                do {
                    var chance = Utilities.RandomDecimalNumber(100, 0);
                    var dChance = Utilities.MortalityRate(curAge);
                    if(chance <= dChance || curAge == Utilities.MaxAge) {
                        deathDate = new Utilities.RandomDateTime(BirthDate.AddYears(curAge).Year).Next();
                        isDead = true;
                    }
                    curAge++;
                } while (!isDead && curAge < yearsFromNow);
            }
            return deathDate;
        }

        public bool IsAlive()
        {
            return DeathDate == DateTime.MinValue;
        }

        public static char ChooseGender()
        {
            var genders = new List<char> { 'm', 'f' };
            int index = Utilities.RandomNumber(genders.Count);
            return genders[index];
        }

        public string FullName(bool lastNameFirst = false)
        {
            if (lastNameFirst)
                return LastName + ", " + FirstName;

            return FirstName + " " + LastName;
        }

        public int GetAge(int year = 0)
        {
            double ageInDays;
            if (year > 0)
            {
                ageInDays = new DateTime(year, 1, 1).Subtract(BirthDate).TotalDays / 365;
            }
            else
            {
                if (DeathDate != DateTime.MinValue)
                {
                    ageInDays = DeathDate.Subtract(BirthDate).TotalDays / 365;
                }
                else
                {
                    ageInDays = DateTime.Now.Subtract(BirthDate).TotalDays / 365;
                }
            }
            return Convert.ToInt16(Math.Floor(ageInDays));
        }

        public bool WasAlive(int year)
        {
            if(DeathDate != DateTime.MinValue)
            {
                if(year < DeathDate.Year)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        // Iterator method to loop through generations using BFS traversal
        public IEnumerable<Person> GetNestedChildren()
        {
            Queue<Person> queue = new();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                yield return current;

                if (current.HasOwnFamily && current.Family.Children.Count > 0)
                {
                    foreach (var child in current.Family.Children)
                    {
                        queue.Enqueue(child);
                    }
                }
            }
        }
        public Person? FindRandomAncestor(Person? person, char gender)
        {
            person ??= this;
            Person? lastAncestor = null;
            if (person.BirthFamily != null)
            {
                lastAncestor = person;
                Person parent = gender == 'm' ? person.BirthFamily.Husband : person.BirthFamily.Wife;

                Person? grandparent = null;
                if(Utilities.RandomNumber(100) < 75) {
                    grandparent = FindRandomAncestor(parent, gender);
                }
                lastAncestor = grandparent ?? parent;
            }

            return lastAncestor;
        }

        public List<Person> FindAncestorsByGender(char gender)
        {
            List<Person> ancestorsWithProperty = new();

            if (Gender == gender)
            {
                ancestorsWithProperty.Add(this);
            }

            if (BirthFamily?.Husband != null || BirthFamily?.Wife != null)
            {
                if (BirthFamily?.Husband != null)
                {
                    ancestorsWithProperty.AddRange(BirthFamily.Husband.FindAncestorsByGender(gender));
                }

                if (BirthFamily?.Wife != null)
                {
                    ancestorsWithProperty.AddRange(BirthFamily.Wife.FindAncestorsByGender(gender));
                }
            }

            return ancestorsWithProperty;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("#" + PersonNumber);
            sb.Append(" [" + Gender.ToString().ToUpper() + "] ");
            sb.Append(FullName());
            sb.Append(" (" + BirthDate.ToString("d") + " - ");
            string ageText = "Age";
            if (DeathDate == DateTime.MinValue)
            {
                sb.Append("Living");
                ageText = "Current Age";
            }
            else
            {
                sb.Append(DeathDate.ToString("d"));
            }
            sb.Append(')');
            sb.Append(" - " + ageText + ": " + Age + " years");
            return sb.ToString();
        }
    }

}
