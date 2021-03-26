using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepSearcher.Models;
using DeepSearcher.Models.TypeConverters;

namespace DeepSearcher.Helpers
{
    internal class SearchConditionComboBoxSource
    {
        public static Dictionary<SearchParameter, string> SearchParameters { get; set; }

        public static Dictionary<SearchType, string> SearchTypes { get; set; }

        static SearchConditionComboBoxSource()
        {
            InitSearchParameters();
            InitSearchTypes();
        }

        private static void InitSearchParameters()
        {
            SearchParameters = new Dictionary<SearchParameter, string>();
            var searchParameters = Enum.GetValues(typeof (SearchParameter));
            foreach (SearchParameter value in searchParameters)
            {
                if (value != SearchParameter.Unknown)
                {
                    SearchParameters.Add(value, value.SearchParameterToString());
                }
            }
        }

        private static void InitSearchTypes()
        {
            SearchTypes = new Dictionary<SearchType, string>();
            var searchTypes = Enum.GetValues(typeof (SearchType));
            foreach (SearchType value in searchTypes)
            {
                if (value != SearchType.Unknown)
                {
                    SearchTypes.Add(value, value.SearchTypeToString());
                }
            }
        }

        internal static SearchParameter GetSearchParameterByIndex(int index)
        {
            return SearchParameters.Keys.ToArray()[index];
        }

        internal static SearchType GetSearchTypeByIndex(int index)
        {
            return SearchTypes.Keys.ToArray()[index];
        }

    }
}
