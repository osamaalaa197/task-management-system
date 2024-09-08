using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Consts
{
    public static class RegexPatterns
    {
        public const string CharactersOnly_ArabicAndEnglish = "(^[^-\\s][a-zA-Z-_ ]*$)|(^[\\u0600-\\u065F\\u066A-\\u06EF\\u06FA-\\u06FF ]*$)";

    }
}
