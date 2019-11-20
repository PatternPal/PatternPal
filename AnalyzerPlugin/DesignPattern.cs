using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzerPlugin
{
    public class DesignPattern : INotifyPropertyChanged
    {
        public string Name { get; set; }

        // Declare the event
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        public DesignPattern(string name)
        {
            Name = name;
            IsChecked = true;
        }


        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

}
