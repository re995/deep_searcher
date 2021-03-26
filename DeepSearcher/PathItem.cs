using System.ComponentModel;
using System.Runtime.CompilerServices;
using DeepSearcher.Annotations;

namespace DeepSearcher
{
    public class PathItem : INotifyPropertyChanged
    {
        private string _path;
        private bool _checked;

        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
                OnPropertyChanged();
            }
        }

        public bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                _checked = value;
                OnPropertyChanged();
            }
        }

        public PathItem(string path, bool @checked)
        {
            Path = path;
            Checked = @checked;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}