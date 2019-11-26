using System.ComponentModel;

namespace IDesign.Core
{
    public class DesignPattern : INotifyPropertyChanged
    {
        private bool isChecked;

        public DesignPattern(string name)
        {
            Name = name;
            IsChecked = true;
        }

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