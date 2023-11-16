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
                maleNamesLoaded = true;
                MaleNames = Properties.Resources.MaleNames.Split("\r\n").ToList();
            }
            return MaleNames;
        }
        private static List<string> GetFemaleNames()
        {
            if (!femaleNamesLoaded)
            {
                femaleNamesLoaded = true;
                FemaleNames = Properties.Resources.FemaleNames.Split("\r\n").ToList();
            }
            return FemaleNames;
        }
        private static List<string> GetSurnames()
        {
            if (!suramesLoaded)
            {
                suramesLoaded = true;
                Surnames = Properties.Resources.Surnames.Split("\r\n").ToList();
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
        public static string RandomName(char gender, string type, Family? family, DateTime? birthDate)
        {
            var items = new List<string>();
            if (gender == 'm')
            {
                items = GetMaleNames().ToList(); // need ToList, otherwise it is only a reference and possibly gets cleared
                if (family != null && family.GenderCount(gender) == 0 && type == "first") {
                    // chance based on a few internet searches on how often first male is named after father
                    int chanceNamedJunior = birthDate?.Year switch
                    {
                        < 1950 => 52,
                        < 2000 => 28,
                        _ => 2
                    };
                    if (Utilities.RandomNumber(100, 0) <= chanceNamedJunior) {
                        items.Clear();
                        items.Add(family.Husband.FirstName);
                    }
                }
            }
            else
            {
                items = GetFemaleNames().ToList();
            }

            int index = Utilities.RandomNumber(items.Count);
            string chosenName = items[index];
            bool finalNameChosen = false;

            // Chance to have middle name be mother's maiden name
            if(type == "middle" && family != null) {
                if (Utilities.RandomNumber(100, 0) <= 10) {
                    chosenName = family.Wife.LastName;
                    finalNameChosen = true;
                }
            }
            else 
            {
                if (family != null && items.Count > 1)
                {
                    if(Utilities.RandomNumber(100) < 20) {
                        // Chance to be named after an ancestor
                        List<Person> ancestorOptions = family.Husband.FindAncestorsByGender(gender);
                        ancestorOptions.AddRange(family.Wife.FindAncestorsByGender(gender));
                        for (int i = 0; i < ancestorOptions.Count; i++) { // remove parents from options
                            Person ra = ancestorOptions[i];
                            if (ra == family.Husband || ra == family.Wife) {
                                ancestorOptions.RemoveAt(i);
                            }
                        }
                        var randomAncestor = Utilities.RandomNumber(ancestorOptions.Count);
                        chosenName = ancestorOptions.Count > 0 ? ancestorOptions[randomAncestor].FirstName : chosenName;
                        finalNameChosen = true;
                    }
                    if(!finalNameChosen) {
                        do
                        {
                            index = Utilities.RandomNumber(items.Count);
                            chosenName = items[index];
                        } while (family.ChildrensNames().Contains(chosenName) && chosenName != "Unnamed");
                    }
                }
            }
            return chosenName;
        }

    }
}
