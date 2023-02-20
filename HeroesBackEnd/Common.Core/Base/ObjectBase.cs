using System.Runtime.CompilerServices;

namespace Common.Core.Base
{
    /// <summary>
    /// Base client applied to the Client Entities
    /// </summary>
    public abstract class ObjectBase : NotificationObject
    {

        public ObjectBase()
        {
        }
        
        //public static CompositionContainer Container { get; set; }

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
    }
}
