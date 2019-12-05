using System.ComponentModel;
using IDesign.Core;

namespace IDesign.Extension
{
    public class DesignPatternViewModel : INotifyPropertyChanged
    {
        private bool isChecked;

        public DesignPatternViewModel(string name, DesignPattern pattern)
        {
            Name = name;
            Pattern = pattern;
            IsChecked = true;
        }

        public DesignPattern Pattern { get; set; }

        public string Name { get; set; }

        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}