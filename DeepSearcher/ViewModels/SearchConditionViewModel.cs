using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DeepSearcher.Helpers;
using DeepSearcher.Models;
using DeepSearcher.Models.TypeConverters;

namespace DeepSearcher.ViewModels
{
    public class SearchConditionViewModel : INotifyPropertyChanged
    {
        #region Fields

        private SearchCondition _condition;

        private bool _selected;

        private int _selectedSearchParameter;

        private int _selectedSearchType;

        private bool _included;

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public SearchConditionViewModel(SearchCondition condition)
        {
            Condition = condition;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #region WPF Properties

        public SearchCondition Condition
        {
            get
            {
                return _condition;
            }
            set
            {
                _condition = value;
                OnPropertyChanged();
            }
        }

        public string Type
        {
            get
            {
                return Condition.Type.SearchTypeToString();
            }
            set
            {
                Condition.Type = value.StringToSearchType();
                OnPropertyChanged();
                OnPropertyChanged("Condition");
            }
        }

        public string Value
        {
            get
            {
                return Condition.Value;
            }
            set
            {
                Condition.Value = value;
                OnPropertyChanged();
                OnPropertyChanged("Condition");
            }
        }

        public string SearchIn
        {
            get
            {
                return Condition.Parameter.SearchParameterToString();
            }
            set
            {
                Condition.Parameter = value.StringToSearchParameter();
                OnPropertyChanged();
                OnPropertyChanged("Condition");
            }
        }

        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                OnPropertyChanged();
            }
        }

        public int SelectedSearchParameter
        {
            get
            {
                return _selectedSearchParameter;
            }
            set
            {
                _selectedSearchParameter = value;
                Condition.Parameter = SearchConditionComboBoxSource.GetSearchParameterByIndex(value);
                OnPropertyChanged();
                OnPropertyChanged("Condition");
            }
        }


        public int SelectedSearchType
        {
            get
            {
                return _selectedSearchType;
            }
            set
            {
                _selectedSearchType = value;
                Condition.Type = SearchConditionComboBoxSource.GetSearchTypeByIndex(value);
                OnPropertyChanged();
                OnPropertyChanged("Condition");
            }
        }

        public bool Included
        {
            get
            {
                return _included;
            }
            set
            {
                _included = value;
                Condition.Include = value;
                OnPropertyChanged();
                OnPropertyChanged("Condition");
            }
        }	

        #endregion

        #region Methods




        #endregion
    }
}