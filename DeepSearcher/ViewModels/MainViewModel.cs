using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.Odbc;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using DeepSearcher.Commands;
using DeepSearcher.Interfaces;
using DeepSearcher.Models;
using DeepSearcher.NDesk.Options;

namespace DeepSearcher.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Fields

        private ObservableCollection<PathItem> _searchPaths;

        private ConcurrentList<ISearchItem> _searchResults;

        private string _searchSequence;

        private Visibility _progressBarVisibility;

        private ObservableCollection<SearchConditionViewModel> _searchConditions;

        private bool _searchInProgress = false;

        private double _totalFilesSize;
        private int _totalFilesCount;
        private int _totalFilesMatch;
        private decimal _deepness;
        private double _progressBarMaxValue;
        private double _currentProgressBarValue;
        private bool _deepSearch;
        private ObservableCollection<ISearchItem> _searchResultsWpf;

        #endregion

        private ICommand _searchCommand;
        private bool _dateFilter;
        private DateTime _dateFilterFrom;
        private DateTime _dateFilterTo;
        private bool _sizeFilter;
        private SizeRange _sizeRange;
        private bool _searchIndexed;
        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            SearchIndexed = true;
            SearchPaths = new ObservableCollection<PathItem>(SettingsLoader.Paths);
            SearchResults = new ConcurrentList<ISearchItem>();
            SearchResultsWPF = new ObservableCollection<ISearchItem>();
            SearchConditions = new ObservableCollection<SearchConditionViewModel>();
            DateFilterFrom = DateFilterTo = DateNow;
            SizeRange = new SizeRange();
            ProgressBarVisibility = Visibility.Hidden;
            SearchPaths.CollectionChanged += (sender, args) => SettingsLoader.Save(SearchPaths);
            HandleCommandLineArgs();
        }

        private void HandleCommandLineArgs()
        {
            string[] args = Environment.GetCommandLineArgs();
            bool show_help = false;
            int repeat = 1;

            var p = new OptionSet()
            {
                {
                    "s|seq|sequence=",
                    "the {SEQUENCE} to look for in each file",
                    v => SearchSequence = v
                },
                {
                    "d|deep|deepSearch|deepsearch|DeepSearch", "Is it going to be a deep search?",
                    v => DeepSearch = v != null
                },
            };

            try
            {
                p.Parse(args);
                if (SearchCommand.CanExecute(this))
                    SearchCommand.Execute(this);
            }
            catch (OptionException e)
            {
                MessageBox.Show("You entered incorrect parameters\n" + e.Message);
                var str = new StringBuilder();
                using (var writer = new StringWriter(str))
                {
                    p.WriteOptionDescriptions(writer);
                    MessageBox.Show(str.ToString());
                }
            }

        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #region WPF Properties

        public ObservableCollection<PathItem> SearchPaths
        {
            get
            {
                return _searchPaths;
            }
            set
            {
                _searchPaths = value;
                OnPropertyChanged();
                ResetCounters();
            }
        }

        internal void ResetCounters()
        {
            TotalFilesSize = 0;
            TotalFilesCount = 0;
            TotalFilesMatch = 0;
        }


        public ConcurrentList<ISearchItem> SearchResults
        {
            get
            {
                return _searchResults;
            }
            set
            {
                _searchResults = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ISearchItem> SearchResultsWPF
        {
            get
            {
                return _searchResultsWpf;
            }
            set
            {
                _searchResultsWpf = value;
                OnPropertyChanged();
            }
        }

        public string SearchSequence
        {
            get
            {
                return _searchSequence;
            }
            set
            {
                _searchSequence = value;
                ResetCounters();
                OnPropertyChanged();
            }
        }

        public Visibility ProgressBarVisibility
        {
            get
            {
                return _progressBarVisibility;
            }
            set
            {
                _progressBarVisibility = value;
                OnPropertyChanged();
            }
        }

        public int TotalFilesCount
        {
            get
            {
                return _totalFilesCount;
            }
            set
            {
                _totalFilesCount = value;
                OnPropertyChanged();
                OnPropertyChanged("MatchedFilesPercent");
            }
        }

        public int TotalFilesMatch
        {
            get
            {
                return _totalFilesMatch;
            }
            set
            {
                _totalFilesMatch = value;
                OnPropertyChanged();
                OnPropertyChanged("MatchedFilesPercent");
            }
        }

        public double TotalFilesSize
        {
            get
            {
                return _totalFilesSize;
            }
            set
            {
                _totalFilesSize = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SearchConditionViewModel> SearchConditions
        {
            get
            {
                return _searchConditions;
            }
            set
            {
                _searchConditions = value;
                OnPropertyChanged();
            }
        }

        public decimal Deepness
        {
            get
            {
                return _deepness;
            }
            set
            {
                _deepness = value;
                OnPropertyChanged();
            }
        }

        public bool SearchInProgress
        {
            get
            {
                return _searchInProgress;
            }
            set
            {
                _searchInProgress = value;
                OnPropertyChanged();
                OnPropertyChanged("SearchButtonText");
                OnPropertyChanged("IsIndexedSearchEnabled");
                OnPropertyChanged("IsDeepSearchEnabled");
                OnPropertyChanged("IsDeepnessLevelEnabled");
            }
        }

        public string SearchButtonText
        {
            get
            {
                return _searchInProgress ? "Cancel" : "Search";
            }
        }

        public double MatchedFilesPercent
        {
            get
            {
                if (TotalFilesCount == 0)
                {
                    return 0;
                }

                return (double)TotalFilesMatch/TotalFilesCount*100;
            }
        }

        public double ProgressBarMaxValue
        {
            get
            {
                return _progressBarMaxValue;
            }
            set
            {
                _progressBarMaxValue = value;
                OnPropertyChanged();
                OnPropertyChanged("ProgressBarIndeterminate");
            }
        }

        public bool ProgressBarIndeterminate
        {
            get
            {
                return _progressBarMaxValue == 0;
            }
        }

        public double CurrentProgressBarValue
        {
            get
            {
                return _currentProgressBarValue;
            }
            set
            {
                _currentProgressBarValue = value;
                OnPropertyChanged();
            }
        }

        public bool DeepSearch
        {
            get
            {
                return _deepSearch;
            }
            set
            {
                _deepSearch = value;
                OnPropertyChanged();
                OnPropertyChanged("IsIndexedSearchEnabled");
                OnPropertyChanged("IsDeepSearchEnabled");
                OnPropertyChanged("IsDeepnessLevelEnabled");
            }
        }

        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                    _searchCommand = new SearchCommand();
                return _searchCommand;
            }
        }

        public bool DateFilter
        {
            get
            {
                return _dateFilter;
            }
            set
            {
                _dateFilter = value;
                OnPropertyChanged();
            }
        }

        public DateTime DateFilterFrom
        {
            get
            {
                return _dateFilterFrom;
            }
            set
            {
                _dateFilterFrom = value;
                OnPropertyChanged();
            }
        }

        public DateTime DateFilterTo
        {
            get
            {
                return _dateFilterTo;
            }
            set
            {
                _dateFilterTo = value;
                OnPropertyChanged();
            }
        }

        public DateTime DateNow
        {
            get
            {
                return DateTime.Now;
            }
        }

        public CalendarDateRange SearchDatesRange
        {
            get
            {
                if (DateFilter)
                    return new CalendarDateRange(DateFilterFrom, DateFilterTo);
                return null;
            }
        }

        public bool SizeFilter
        {
            get
            {
                return _sizeFilter;
            }
            set
            {
                _sizeFilter = value;
                OnPropertyChanged();
            }
        }

        public SizeRange SizeRange
        {
            get
            {
                if (!SizeFilter)
                    return null;
                return _sizeRange;
            }
            set
            {
                _sizeRange = value;
                OnPropertyChanged();
            }
        }

        public bool SearchIndexed
        {
            get
            {
                return _searchIndexed;
            }
            set
            {
                _searchIndexed = value;
                OnPropertyChanged();
                OnPropertyChanged("IsIndexedSearchEnabled");
                OnPropertyChanged("IsDeepSearchEnabled");
                OnPropertyChanged("IsDeepnessLevelEnabled");
            }
        }

        public bool IsIndexedSearchEnabled
        {
            get
            {
                if (SearchInProgress || DeepSearch)
                    return false;
                return true;
            }
        }

        public bool IsDeepSearchEnabled
        {
            get
            {
                if (SearchInProgress || SearchIndexed)
                    return false;
                return true;
            }
        }

        public bool IsDeepnessLevelEnabled
        {
            get
            {
                if (SearchInProgress || SearchIndexed)
                    return false;
                return true;
            }
        }

        #endregion
    }
}