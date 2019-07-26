using System;
using System.Text.RegularExpressions;

namespace Glue.Util
{
    public class FormatDuration
    {
        public static long MillisFromString(string input)
        {
            long time = 0;

            if (input == null || input.Length == 0)
            {
                return time;
            }

            // Strip whitespace or match string has to be more complicated
            string inputCooked = Regex.Replace(input, @"\s+", "");

            Match match = Regex.Match(
                inputCooked,
                @"^((?<hours>\d+)h)?((?<minutes>\d+)m)?((?<seconds>\d+)s)?((?<millis>\d+)ms)?$", 
                    RegexOptions.ExplicitCapture | 
                    RegexOptions.Compiled | 
                    RegexOptions.CultureInvariant | 
                    // RegexOptions.RightToLeft | 
                    RegexOptions.IgnoreCase
                    );

            int h = match.Groups["hours"].Success ? int.Parse(match.Groups["hours"].Value) : 0;
            int m = match.Groups["minutes"].Success ? int.Parse(match.Groups["minutes"].Value) : 0;
            int s = match.Groups["seconds"].Success ? int.Parse(match.Groups["seconds"].Value) : 0;
            int ms = match.Groups["millis"].Success ? int.Parse(match.Groups["millis"].Value) : 0;

            time += h * 60 * 60 * 1000;
            time += m *      60 * 1000;
            time += s *           1000;
            time += ms;

            return time;
        }

        public static string StringFromMillis(long milliseconds)
        {
            string formattedTime = "";

            if (0 == milliseconds)
            {
                formattedTime = "0ms";
            }
            else
            {
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(milliseconds);
                if (timeSpan.Hours > 0)
                {
                    formattedTime += timeSpan.Hours.ToString() + "h ";
                }
                if (timeSpan.Minutes > 0)
                {
                    formattedTime += timeSpan.Minutes.ToString() + "m ";
                }
                if (timeSpan.Seconds > 0)
                {
                    formattedTime += timeSpan.Seconds.ToString() + "s ";
                }
                if (timeSpan.Milliseconds > 0)
                {
                    formattedTime += timeSpan.Milliseconds.ToString() + "ms ";
                }
            }

            return formattedTime.Trim();
        }
    }
}
