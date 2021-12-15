using System.ComponentModel;

namespace IDesign.Extension.ViewModels
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        public abstract string Title { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Invoke PropertyChanged event for given property name.
        ///     Used for updating content in ViewModels.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
