using System.Text.Json.Serialization;
using System.Text.Json;

namespace ProceduralFamilyTree
{
    public class Utilities
    {
        private static readonly Random _rnd = new();
        public static int YearsBetweenChildren { get; set; } = 5;
        public static int MaxNumberOfKids { get; set; } = 10;
        public static int MinMarriageAge { get; set; } = 18;
        public static int MaxAge { get; set; } = 100;
        // Add more properties as needed

        static Utilities()
        {
        }

        public static int RandomNumber(int max = 0, int min = 0)
        {
            if (min != 0)
            {
                return _rnd.Next(min, max);
            }
            else
            {
                return _rnd.Next(max);
            }
        }

        public static int WeightedRandomNumber(double mean, double stdDev, int maxValue = 0, int minValue = 0)
        {
            double u1 = 1.0 - _rnd.NextDouble();
            double u2 = 1.0 - _rnd.NextDouble();

            // Transform uniform random values to create a standard normal distribution
            double z = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);

            // Scale and shift the distribution to the desired mean and standard deviation
            double randomValue = (mean + (stdDev * z)) * maxValue;

            // Clamp the random value within the desired range (if needed)
            randomValue = Math.Max(minValue, Math.Min(maxValue, randomValue));

            return (int)randomValue;
        }

        public class RandomDateTime
        {
            DateTime start;
            DateTime end;
            int range;

            /// <summary>
            /// Gets a random day in the year of startYear with optional range of years
            /// </summary>
            /// <param name="startYear"></param>
            /// <param name="yearRange"></param>
            public RandomDateTime(int startYear, int yearRange = 0)
            {
                var endYear = startYear;
                if (yearRange > 0)
                {
                    startYear = _rnd.Next(startYear - yearRange, startYear);
                    endYear = _rnd.Next(startYear, startYear + yearRange);
                }
                start = new DateTime(startYear, 1, 1);
                end = new DateTime(endYear, 12, 31);
                range = (end - start).Days;
            }

            public DateTime Next()
            {
                return start.AddDays(_rnd.Next(range)).AddHours(_rnd.Next(0, 24)).AddMinutes(_rnd.Next(0, 60)).AddSeconds(_rnd.Next(0, 60));
            }
        }
    }
}
