using System.Linq;
using System.Text;

namespace MTracking.Core.Extensions
{
    public static class StringExtension
    {
        public static string ReplaceFromSymbols(this string str)
        {
            var sb = new StringBuilder(str);

            return sb.Replace("\u005e\u005e", "\u05F4").Replace("\u005e", "\u05F3").Replace("##", " ").Replace(";", ",").ToString();
        }

        public static string ReplaceToHebrewGershayim(this string str)
        {
            var sb = new StringBuilder(str);

            return sb.Replace("\u0022", "\u05F4").ToString();
        }

        public static string ReplaceToHebrewGeresh(this string str)
        {
            var sb = new StringBuilder(str);

            return sb.Replace("\u0027", "\u05F3").ToString();
        }

        public static string ReplaceToSymbols(this string str)
        {
            var sb = new StringBuilder(str);

            return sb.Replace("\u05F4", "\u005e\u005e").Replace("\u05F3", "\u005e").Replace("\n", "##").Replace(",", ";").ToString();
        }

        public static string RemoveQuotes(this string str)
        {
            var sb = new StringBuilder(str);

            return sb.Replace("\"", "").ToString();
        }

        public static string OnlyDigits(this string str)
        {
            var sb = new StringBuilder();

            foreach (var s in str.Where(char.IsDigit)) 
                sb.Append(s);

            return sb.ToString();
        }

        public static string СlearToDigitsAndPunctuations(this string str)
        {
            var sb = new StringBuilder();

            foreach (var s in str.Where(x => char.IsDigit(x) || char.IsPunctuation(x)))
                sb.Append(s);

            return sb.ToString();
        }
    }
}
