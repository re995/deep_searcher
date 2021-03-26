using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using DeepSearcher.Annotations;

namespace DeepSearcher.Models
{
    public class SizeRange : INotifyPropertyChanged
    {
        private double _sizeFrom;
        private long _modifierFrom;
        private double _sizeTo;
        private long _modifierTo;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public SizeRange()
        {
            SizeFrom = 0;
            ModifierFrom = 1024;
            SizeTo = 0;
            ModifierTo = 1024;
        }

        public double SizeFrom
        {
            get
            {
                return _sizeFrom;
            }
            set
            {
                _sizeFrom = value;
                OnPropertyChanged();
            }
        }

        public long ModifierTo
        {
            get
            {
                return _modifierTo;
            }
            set
            {
                _modifierTo = value;
                OnPropertyChanged();
            }
        }

        public double SizeTo
        {
            get
            {
                return _sizeTo;
            }
            set
            {
                _sizeTo = value;
                OnPropertyChanged();
            }
        }

        public long ModifierFrom
        {
            get
            {
                return _modifierFrom;
            }
            set
            {
                _modifierFrom = value;
                OnPropertyChanged();
            }
        }

        public bool ValidateSize()
        {
            return FinalSizeFrom <= FinalSizeTo;
        }

        public double FinalSizeFrom
        {
            get
            {
                return Math.Min(SizeTo * ModifierTo, SizeFrom * ModifierFrom);
            }
        }

        public double FinalSizeTo
        {
            get
            {
                return Math.Max(SizeTo*ModifierTo, SizeFrom*ModifierFrom);
            }
        }

        public bool IsInRange(double size)
        {
            return size >= FinalSizeFrom && size <= FinalSizeTo;
        }
    }
}
