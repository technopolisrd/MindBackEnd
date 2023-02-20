using System.Runtime.Serialization;
using System.Runtime.CompilerServices;

namespace Common.Core.Base
{
    /// <summary>
    /// Base client applied to the Business Entities
    /// </summary>
    [DataContract]
    public abstract class EntityBase : NotificationObject, IExtensibleDataObject
    {
        #region Property Change Notification

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(true, propertyName);
        }

        protected virtual void OnPropertyChanged(bool makeDirty, [CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
        }

        #endregion

        #region IExtensibleDataObject Members

        public ExtensionDataObject ExtensionData { get; set; }

        #endregion
    }
}
