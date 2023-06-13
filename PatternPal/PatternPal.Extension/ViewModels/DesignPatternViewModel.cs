#region

using System.ComponentModel;

using PatternPal.Protos;

#endregion

namespace PatternPal.Extension.ViewModels
{
    // TODO Comment
    public class DesignPatternViewModel : INotifyPropertyChanged
    {
        private bool _isChecked;

        public DesignPatternViewModel(
            Recognizer recognizer)
        {
            Recognizer = recognizer;
            IsChecked = true;
        }

        public Recognizer Recognizer { get; }

        public string Name => Recognizer.ToString();

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(
            string name)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(name));
        }
    }
}
