using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralFamilyTree
{
    public class Names
    {
        private static bool maleNamesLoaded = false;
        private static List<string> MaleNames = new List<string>();
        private static bool femaleNamesLoaded = false;
        private static List<string> FemaleNames = new List<string>();
        private static bool suramesLoaded = false;
        private static List<string> Surnames = new List<string>();

        private static List<string> GetMaleNames()
        {
            if (!maleNamesLoaded)
            {
                string fileName = "Names\\MaleNames.txt";
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                if (File.Exists(filePath))
                {
                    maleNamesLoaded = true;
                    MaleNames = File.ReadAllLines(filePath).ToList();
                }
                else
                {
                    MaleNames = new List<string> { "Unnamed" };
                }
            }
            return MaleNames;
        }
        private static List<string> GetFemaleNames()
        {
            if (!femaleNamesLoaded)
            {
                string fileName = "Names\\FemaleNames.txt";
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                if (File.Exists(filePath))
                {
                    femaleNamesLoaded = true;
                    FemaleNames = File.ReadAllLines(filePath).ToList();
                }
                else
                {
                    FemaleNames = new List<string> { "Unnamed" };
                }
            }
            return FemaleNames;
        }
        private static List<string> GetSurnames()
        {
            if (!suramesLoaded)
            {
                string fileName = "Names\\Surnames.txt";
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                if (File.Exists(filePath))
                {
                    suramesLoaded = true;
                    Surnames = File.ReadAllLines(filePath).ToList();
                }
                else
                {
                    Surnames = new List<string> { "Unnamed" };
                }
            }
            return Surnames;
        }

        public static string RandomSurname()
        {
            var items = GetSurnames();

            int index = Utilities.RandomNumber(items.Count);
            string randomName = items[index];
            return randomName;
        }
        public static string RandomFirstName(char gender, Family? family)
        {
            var items = new List<string>();
            if (gender == 'm')
            {
                items = GetMaleNames();
            }
            else
            {
                items = GetFemaleNames();
            }

            int index = Utilities.RandomNumber(items.Count);
            string randomName = items[index];
            if (family != null)
            {
                do
                {
                    index = Utilities.RandomNumber(items.Count);
                    randomName = items[index];
                } while (family.ChildrensNames().Contains(randomName) && randomName != "Unnamed");
            }
            return randomName;
        }

    }
}
