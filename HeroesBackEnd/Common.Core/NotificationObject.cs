using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Common.Core
{
    [DataContract]
    public abstract class NotificationObject : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        protected event PropertyChangedEventHandler _PropertyChanged;

        List<PropertyChangedEventHandler> _PropertyChangedSubscribers = new List<PropertyChangedEventHandler>();

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (!_PropertyChangedSubscribers.Contains(value))
                {
                    _PropertyChanged += value;
                    _PropertyChangedSubscribers.Add(value);
                }
            }
            remove
            {
                _PropertyChanged -= value;
                _PropertyChangedSubscribers.Remove(value);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (_PropertyChanged != null)
                _PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
