using System.ComponentModel;

namespace YodaApp.ViewModels
{
    internal class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected void Set<T>(ref T dest, string propertyName, T val)
        {
            dest = val;
            OnPropertyChanged(propertyName);
        }
    }
}