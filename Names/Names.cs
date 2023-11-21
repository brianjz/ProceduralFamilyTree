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
        // public static string RandomName(char gender, string type, Family? family, DateTime? birthDate)
        public static string RandomName(Person person, string type, string? ignoredName = null)
        {
            // TODO: A lot of this logic got messy as it grew. Look into refactoring.
            var items = new List<string>();
            if (person.Gender == 'm')
            {
                items = GetMaleNames().ToList(); // need ToList, otherwise it is only a reference and possibly gets cleared
                if (person.BirthFamily != null && person.BirthFamily.GenderCount(person.Gender) == 0) {
                    // chance based on a few internet searches on how often first male is named after father, or used as a middle name
                    int chanceNamedJunior = person.BirthDate.Year switch
                    {
                        < 1950 => 52,
                        < 2000 => 28,
                        _ => 2
                    };
                    if (Utilities.RandomNumber(100, 0) <= chanceNamedJunior && !person.BirthFamily.ChildrensNames().Contains(person.BirthFamily.Husband.FirstName)) {
                        items.Clear();
                        items.Add(person.BirthFamily.Husband.FirstName);
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
            if(type == "middle" && person.BirthFamily != null) {
                if (Utilities.RandomNumber(100, 0) <= 10) {
                    chosenName = person.BirthFamily.Wife.LastName;
                    finalNameChosen = true;
                }
            }
            else 
            {
                if (person.BirthFamily != null && items.Count > 1)
                {
                    if(Utilities.RandomNumber(100) < 20) {
                        // Chance to be named after an ancestor
                        List<Person> allAncestorOptions = person.BirthFamily.Husband.FindAncestorsByGender(person.Gender);
                        allAncestorOptions.AddRange(person.BirthFamily.Wife.FindAncestorsByGender(person.Gender));
                        var ancestorNameOptions = new List<Person>();
                        for (int i = 0; i < allAncestorOptions.Count; i++) { // remove parents from options
                            Person ra = allAncestorOptions[i];
                            if (ra != person.BirthFamily.Husband && ra.FirstName != person.BirthFamily.Husband.FirstName 
                                && ra != person.BirthFamily.Wife && ra.FirstName != person.BirthFamily.Wife.FirstName) {
                                ancestorNameOptions.Add(ra);
                            }
                        }
                        var randomAncestor = Utilities.RandomNumber(ancestorNameOptions.Count);
                        chosenName = ancestorNameOptions.Count > 0 ? ancestorNameOptions[randomAncestor].FirstName : chosenName;
                        if(person.BirthFamily.ChildrensNames().Contains(chosenName)) { // handles edge cases
                            randomAncestor = Utilities.RandomNumber(ancestorNameOptions.Count);
                            chosenName = ancestorNameOptions.Count > 0 ? ancestorNameOptions[randomAncestor].FirstName : chosenName;
                        }
                        finalNameChosen = true;
                    }
                }
            }
            bool nameExists = false;
            if(person.BirthFamily != null && type == "first") {
                if(person.BirthFamily.ChildrensNames().Contains(chosenName)) {
                    nameExists = true;
                }
            }
            if (items.Count > 1) {
                if(!finalNameChosen || chosenName == ignoredName) {
                    int valve = 0;
                    while (nameExists || chosenName == ignoredName)
                    {
                        index = Utilities.RandomNumber(items.Count);
                        chosenName = items[index];
                        if(person.BirthFamily != null) {
                            if(!person.BirthFamily.ChildrensNames().Contains(chosenName)) {
                                nameExists = false;
                            }
                            valve++;
                            if(valve > 50) {
                                nameExists = false; // safety valve
                            }
                        }
                    };
                }
            }

            return chosenName;
        }

    }
}
