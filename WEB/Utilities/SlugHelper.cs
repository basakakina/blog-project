using System.Text;
using System.Text.RegularExpressions;

namespace WEB.Utils
{
    public static class SlugHelper
    {
        public static string ToSlug(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            s = s.Trim().ToLowerInvariant();
            s = Regex.Replace(s, @"\s+", "-");              
            s = Regex.Replace(s, @"[^a-z0-9\-]", "");       
            s = Regex.Replace(s, @"-+", "-");               
            return s;
        }
    }
}
