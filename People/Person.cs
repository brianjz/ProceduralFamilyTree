using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralFamilyTree
{
    public class Person
    {
        private string firstName = String.Empty;
        private string lastName = String.Empty;
        private DateTime birthDate = DateTime.MinValue;
        private DateTime deathDate = DateTime.MinValue;
        private char gender;
        private Family? family = null;
        private string personNumber = string.Empty;

        public string FirstName { get => firstName; set => firstName = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public DateTime BirthDate { get => birthDate; set => birthDate = value; }
        public DateTime DeathDate { get => deathDate; set => deathDate = value; }
        public char Gender { get => gender; set => gender = value; }
        public Family? Family { get => family; set => family = value; }
        public string PersonNumber { get => personNumber; set => personNumber = value; }

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
            //PersonNumber = GetPersonNumber();
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
            Family = family;
            FirstName = Names.RandomFirstName(Gender, family);
            LastName = lastName;
            BirthDate = birthDate;
            //PersonNumber = GetPersonNumber();
            if (DateTime.Now.Subtract(BirthDate).TotalDays / 365 > Utilities.MaxAge)
            {
                DeathDate = new Utilities.RandomDateTime(BirthDate.AddYears(Utilities.RandomNumber(Utilities.MaxAge, 0)).Year).Next();
            }
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
            FirstName = Names.RandomFirstName(Gender, Family);
            LastName = Names.RandomSurname();
            BirthDate = birthDate;
            //PersonNumber = GetPersonNumber();
            int minAge = 0;
            if (spouse != null)
            {
                minAge = Utilities.RandomNumber(spouse.Age() + 5, spouse.Age() - 5);
            }
            if (DateTime.Now.Subtract(BirthDate).TotalDays / 365 > Utilities.MaxAge)
            {
                DeathDate = new Utilities.RandomDateTime(BirthDate.AddYears(Utilities.WeightedRandomNumber(0.8, 0.2, Utilities.MaxAge, minAge)).Year).Next();
            }
        }

        public Person(Person spouse)
        {
            Gender = spouse.Gender == 'm' ? 'f' : 'm';
            FirstName = Names.RandomFirstName(Gender, null);
            LastName = Names.RandomSurname();
            BirthDate = new Utilities.RandomDateTime(spouse.BirthDate.Year, 5).Next();
            int minAge = Utilities.RandomNumber(spouse.Age() + 5, spouse.Age() - 5);
            if (DateTime.Now.Subtract(BirthDate).TotalDays / 365 > Utilities.MaxAge)
            {
                DeathDate = new Utilities.RandomDateTime(BirthDate.AddYears(Utilities.WeightedRandomNumber(0.8, 0.2, Utilities.MaxAge, minAge)).Year).Next();
            }
        }

        public bool IsAlive()
        {
            return DeathDate == DateTime.MinValue ? true : false;
        }

        public bool HasOwnFamily()
        {
            return Family.Wife == this || Family.Husband == this;
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

        public int Age(int year = 0)
        {
            double ageInDays = 0;
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
            sb.Append(")");
            sb.Append(" - " + ageText + ": " + Age() + " years");
            return sb.ToString();
        }
    }

}
