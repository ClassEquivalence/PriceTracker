namespace PriceTracker.Core.Utils
{
    public static class StringHelperExtensions
    {
        public static string SubstringBeforeFirstEntryOrEmpty(this string str, string entry)
        {
            var splitted = str.Split(entry);
            return splitted.Length > 0 ? splitted[0] : string.Empty;
        }
        public static string SubstringWithAndAfterFirstEntryOrEmpty(this string str, string entry)
        {
            var splitted = str.Split(entry);
            return splitted.Length > 1 ? entry + splitted[1] : string.Empty;
        }
    }
}
