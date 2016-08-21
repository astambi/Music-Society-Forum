using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Music_Society_Forum.Classes
{
    public class Utils
    {
        public static string CutText(string text, int maxLength = 750)
        {
            if (text == null || text.Length <= maxLength)
                return text;
            return text.Substring(0, maxLength) + "...";
        }
    }
}