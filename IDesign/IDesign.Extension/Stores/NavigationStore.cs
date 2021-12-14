using IDesign.Extension.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDesign.Extension.Stores
{
    public class NavigationStore
    {
        public event Action CurrentViewModelChanged;
        private ViewModel _currentViewModel { get; set; }
        public ViewModel CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }
            set
            {
                _currentViewModel = value;
                CurrentViewModelChanged?.Invoke();
            }
        }
    }
}
