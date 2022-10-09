using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Quilt
{

    public abstract class BaseVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static readonly PropertyChangedEventArgs s_allPropertiesChangedArgs = new PropertyChangedEventArgs(null);


        protected void RaisePropertyChangedEvent([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RaiseAllPropertiesChangedEvent()
        {
            PropertyChanged?.Invoke(this, s_allPropertiesChangedArgs);
        }

        protected bool Set<T>(ref T field, T newValue = default(T), [CallerMemberName] string propertyName = null, params string[] addtlPropertyChanges)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            field = newValue;

            RaisePropertyChangedEvent(propertyName);

            foreach (var addtlProperty in addtlPropertyChanges)
            {
                RaisePropertyChangedEvent(addtlProperty);
            }

            return true;
        }
    }
}
