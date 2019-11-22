using System.ComponentModel;

namespace IDesign.Core
{
    public class DesignPatternViewModel : INotifyPropertyChanged
    {
        private bool isChecked;
        public event PropertyChangedEventHandler PropertyChanged;
        public string DesignPattern { get; set; }
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

        public DesignPatternViewModel(string name)
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
