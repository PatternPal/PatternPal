using System.ComponentModel;

namespace IDesign.Extension
{
    public class DesignPattern : INotifyPropertyChanged
    {
        private bool isChecked;
        public event PropertyChangedEventHandler PropertyChanged;
        public string Name { get; set; }
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        public DesignPattern(string name)
        {
            Name = name;
            IsChecked = true;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
