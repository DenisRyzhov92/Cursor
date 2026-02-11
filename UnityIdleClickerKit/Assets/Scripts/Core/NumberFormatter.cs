using System;

namespace IdleClickerKit.Core
{
    public static class NumberFormatter
    {
        private static readonly string[] Suffixes = { "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No" };

        public static string Compact(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                return "0";
            }

            var sign = value < 0 ? "-" : string.Empty;
            var abs = Math.Abs(value);

            if (abs < 1000d)
            {
                return sign + abs.ToString("0.##");
            }

            var index = 0;
            while (abs >= 1000d && index < Suffixes.Length - 1)
            {
                abs /= 1000d;
                index++;
            }

            string pattern;
            if (abs >= 100d)
            {
                pattern = "0";
            }
            else if (abs >= 10d)
            {
                pattern = "0.0";
            }
            else
            {
                pattern = "0.00";
            }

            return sign + abs.ToString(pattern) + Suffixes[index];
        }
    }
}
