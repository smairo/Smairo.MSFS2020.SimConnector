using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace Smairo.MSFS2020.Model
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string sPropertyName = null)
        {
            var hEventHandler = PropertyChanged;
            if (hEventHandler != null && !string.IsNullOrEmpty(sPropertyName))
            {
                hEventHandler(this, new PropertyChangedEventArgs(sPropertyName));
            }
        }

        protected bool SetProperty<T>(ref T tField, T tValue, [CallerMemberName] string sPropertyName = null)
        {
            return SetProperty(ref tField, tValue, out _, sPropertyName);
        }

        protected bool SetProperty<T>(
            ref T tField,
            T tValue,
            out T tPreviousValue,
            [CallerMemberName] string sPropertyName = null)
        {
            if (!Equals(tField, tValue))
            {
                tPreviousValue = tField;
                tField = tValue;
                OnPropertyChanged(sPropertyName);
                return true;
            }

            tPreviousValue = default;
            return false;
        }
    }
}