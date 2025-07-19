using System;

namespace NamelessRogue.Engine.Utility
{
    public static class RandomDateTimeGenerator
    {
        private static readonly Random _random = new Random();

        public static DateTime RandomDate(DateTime start, DateTime end)
        {
            if (start >= end)
                throw new ArgumentException("Start date must be earlier than end date.");

            TimeSpan range = end - start;
            double randFraction = _random.NextDouble();
            TimeSpan randomSpan = new TimeSpan((long)(range.Ticks * randFraction));
            return start + randomSpan;
        }
    }
}