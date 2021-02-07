using System;

namespace MegoTest.Common
{
    public static class Rnd
    {
        private static Random rnd = new Random();
        public static float InRange(float min, float max)
        {
            var value = (float) rnd.NextDouble() * (max - min) + min;
            return value;
        }

        public static float InRange(int min, int max)
        {
            var value = rnd.Next(min, max);
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns> Seconds interval between 1_000 ms (1 s) (inclusive) and 10_001 ms (~ 10 s) (inclusive) </returns>
        public static float TaskTimeMilliseconds => InRange(1_000.0f, 10_001.0f);

        /// <summary>
        ///
        /// </summary>
        /// <returns> Random RequestStatusCode OK or ERROR </returns>
        public static RequestStatusCode TaskStatus => InRange(0, 2) == 0 ? RequestStatusCode.OK : RequestStatusCode.ERROR;
    }
}
