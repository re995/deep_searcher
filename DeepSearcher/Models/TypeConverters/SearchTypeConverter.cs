using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSearcher.Models.TypeConverters
{
    internal static class SearchTypeConverter
    {
        internal static string SearchTypeToString(this SearchType type)
        {
            switch (type)
            {
                case SearchType.Contains:
                    return "Contains";
                case SearchType.NotContains:
                    return "Not Contains";
                case SearchType.Is:
                    return "Is";
                case SearchType.Not:
                    return "Is Not";
            }
            return "Unknown";
        }

        internal static SearchType StringToSearchType(this string str)
        {
            if (str.Contains("["))
            {
                str = str.Split(',')[1].TrimStart().Replace("]", "").Replace("[", "");
            }
            switch (str)
            {
                case "Contains":
                    return SearchType.Contains;
                case "Not Contains":
                    return SearchType.NotContains;
                case "Is":
                    return SearchType.Is;
                case "Is Not":
                    return SearchType.Not;
            }
            return SearchType.Unknown;
        }
    }
}
