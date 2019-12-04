using IDesign.Core;
using System.ComponentModel;

namespace IDesign.Extension
{
    public class DesignPatternViewModel : INotifyPropertyChanged
    {
        private bool isChecked;
        public event PropertyChangedEventHandler PropertyChanged;
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

        public DesignPatternViewModel(string name, DesignPattern pattern)
        {
            Name = name;
            Pattern = pattern;
            IsChecked = true;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}