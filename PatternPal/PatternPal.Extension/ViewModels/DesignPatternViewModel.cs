#region

using System.ComponentModel;

using PatternPal.Protos;

#endregion

namespace PatternPal.Extension.ViewModels
{
    public class DesignPatternViewModel : INotifyPropertyChanged
    {
        private bool m_IsChecked;

        public DesignPatternViewModel(
            Recognizer recognizer)
        {
            Recognizer = recognizer;
            IsChecked = true;
        }

        //public DesignPatternViewModel(
        //    string name,
        //    DesignPattern pattern,
        //    string wikiPage)
        //{
        //    WikiPage = wikiPage;
        //    Name = name;
        //    Pattern = pattern;
        //    IsChecked = true;
        //}

        public Recognizer Recognizer { get; }

        public string Name => Recognizer.ToString();

        //public string WikiPage { get; set; }

        public bool IsChecked
        {
            get => m_IsChecked;
            set
            {
                m_IsChecked = value;
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
