using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CompilerDemo.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(propertyName, new PropertyChangedEventArgs(propertyName));
    }
}
