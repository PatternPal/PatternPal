using System.ComponentModel;
using IDesign.Core.Models;

namespace IDesign.Extension.ViewModels
{
    public class DesignPatternViewModel : INotifyPropertyChanged
    {
        private bool isChecked;


        public DesignPatternViewModel(string name, DesignPattern pattern, string wikiPage)
        {
            WikiPage = wikiPage;
            Name = name;
            Pattern = pattern;
            IsChecked = true;
        }

        public DesignPattern Pattern { get; set; }

        public string Name { get; set; }

        public string WikiPage { get; set; }

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