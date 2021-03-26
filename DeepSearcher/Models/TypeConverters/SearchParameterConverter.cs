using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSearcher.Models.TypeConverters
{
    internal static class SearchParameterConverter
    {
        internal static string SearchParameterToString(this SearchParameter parameter)
        {
            switch (parameter)
            {
                case SearchParameter.NameOrExtension:
                    return "Name Or Extension";
                case SearchParameter.Name:
                    return "File Name";
                case SearchParameter.Extension:
                    return "Extension";
                    case SearchParameter.WholePath:
                    return "Whole FullName";
            }

            return "Unknown";
        }

        internal static SearchParameter StringToSearchParameter(this string str)
        {
            if (str.Contains("["))
            {
                str = str.Split(',')[1].TrimStart().Replace("]", "").Replace("[", "");
            }
            switch (str)
            {
                case "Name Or Extension":
                    return SearchParameter.NameOrExtension;
                case "File Name":
                    return SearchParameter.Name;
                case "Extension":
                    return SearchParameter.Extension;
                case "Whole FullName":
                    return SearchParameter.WholePath;
            }
            return SearchParameter.Unknown;
        }
    }
}
