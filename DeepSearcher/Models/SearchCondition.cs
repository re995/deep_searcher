using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DeepSearcher.Annotations;
using DeepSearcher.Indexing;
using DeepSearcher.Interfaces;
using DeepSearcher.Models.TypeConverters;

namespace DeepSearcher.Models
{
    public class SearchCondition : INotifyPropertyChanged
    {
        private SearchType _type;
        private SearchParameter _parameter;
        private string _value;
        private bool _include;

        public SearchType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }

        public SearchParameter Parameter
        {
            get
            {
                return _parameter;
            }
            set
            {
                _parameter = value;
                OnPropertyChanged();
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public bool Include
        {
            get
            {
                return _include;
            }
            set
            {
                _include = value;
                OnPropertyChanged();
            }
        }

        public static SearchCondition Empty
        {
            get
            {
                return new SearchCondition();
            }
        }

        private SearchCondition()
        {
        
        }

        public SearchCondition(SearchType type, SearchParameter parameter, string value,bool include)
        {
            Type = type;
            Parameter = parameter;
            Value = value;
            Include = include;
        }

        public virtual bool IsIncluded(ISearchItem fileName)
        {
            if (Type== SearchType.Unknown || Parameter == SearchParameter.Unknown || string.IsNullOrEmpty(Value))
            {
                return true;
            }

            switch (Type)
            {
                case SearchType.Contains:
                    return SwitchContains(fileName) == Include;
                case SearchType.NotContains:
                    return !SwitchContains(fileName) == Include;
                case SearchType.Is:
                    return SwitchEquals(fileName) == Include;
                case SearchType.Not:
                    return !SwitchEquals(fileName) == Include;
                default:
                    return false;
            }
        }

        public virtual  bool IsIncluded(IndexedItem file)
        {
            if (Type == SearchType.Unknown || Parameter == SearchParameter.Unknown || string.IsNullOrEmpty(Value))
            {
                return true;
            }

            switch (Type)
            {
                case SearchType.Contains:
                    return SwitchContains(file) == Include;
                case SearchType.NotContains:
                    return !SwitchContains(file) == Include;
                case SearchType.Is:
                    return SwitchEquals(file) == Include;
                case SearchType.Not:
                    return !SwitchEquals(file) == Include;
                default:
                    return false;
            }
        }

        private bool SwitchContains(IndexedItem file)
        {
            switch (Parameter)
            {
                case SearchParameter.Extension:
                    {
                        return file.Extension.ToLower().Contains(Value.ToLower());
                    }
                case SearchParameter.Name:
                    {
                        return Path.GetFileNameWithoutExtension(file.Name).ToLower().Contains(Value.ToLower());
                    }
                case SearchParameter.NameOrExtension:
                    {
                        return Path.GetFileNameWithoutExtension(file.Name).ToLower().Contains(Value.ToLower()) || file.Extension.ToLower().Contains(Value.ToLower());
                    }
                case SearchParameter.WholePath:
                    {
                        return file.FullName.ToLower().Contains(Value.ToLower());
                    }
            }
            return false;
        }

        private bool SwitchContains(ISearchItem fileName)
        {
            switch (Parameter)
            {
                case SearchParameter.Extension:
                {
                    return fileName.Extension.ToLower().Contains(Value.ToLower());
                }
                case SearchParameter.Name:
                {
                    return Path.GetFileNameWithoutExtension(fileName.Name).ToLower().Contains(Value.ToLower());
                }
                case SearchParameter.NameOrExtension:
                {
                    return Path.GetFileNameWithoutExtension(fileName.Name).ToLower().Contains(Value.ToLower()) || fileName.Extension.ToLower().Contains(Value.ToLower());
                }
                case SearchParameter.WholePath:
                {
                    return fileName.FullName.ToLower().Contains(Value.ToLower());
                }
            }
            return false;
        }

        private bool SwitchEquals(ISearchItem fileName)
        {
            switch (Parameter)
            {
                case SearchParameter.Extension:
                    {
                        return fileName.Extension.ToLower().Equals(Value.ToLower());
                    }
                case SearchParameter.Name:
                    {
                        return Path.GetFileNameWithoutExtension(fileName.Name).ToLower().Equals(Value.ToLower());
                    }
                case SearchParameter.NameOrExtension:
                    {
                        return Path.GetFileNameWithoutExtension(fileName.Name).ToLower().Equals(Value) || fileName.Extension.Equals(Value.ToLower());
                    }
                case SearchParameter.WholePath:
                    {
                        return fileName.FullName.ToLower().Equals(Value.ToLower());
                    }
            }
            return false;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} ,Include = {3}", Parameter.SearchParameterToString(), Type.SearchTypeToString(), Value, Include);
        }
    }
}