using IDesign.Extension.Stores;
using IDesign.Extension.ViewModels;
using NUnit.Framework;

namespace IDesign.Tests.Extension.Stores
{
    public class NavigationStoreTest
    {
        private NavigationStore _navigationStore { get; set; }

        [SetUp]
        public void SetUp()
        {
            _navigationStore = new NavigationStore();
        }

        /// <summary>
        /// When a new viewmodel is set, the previous one should be returned when Back() is called
        /// </summary>
        [Test]
        public void Back_WithPreviousViewModel_ReturnsPreviousViewModel()
        {
            // previous viewmodel that will be compared
            ViewModel toCompare = _navigationStore.CurrentViewModel;
            // switch current viewmodel to a different one
            _navigationStore.CurrentViewModel = new HomeViewModel(_navigationStore);
            // check if back is same
            ViewModel result = _navigationStore.Back();
            Assert.AreEqual(toCompare, result);
        }

        /// <summary>
        /// When there are multiple viewmodels in the history, the last one should be returned
        /// </summary>
        [Test]
        public void Back_WithMultiplePreviousViewModel_ReturnsLastVisitedViewModel()
        {
            ViewModel vm1 = new HomeViewModel(_navigationStore);
            ViewModel vm2 = new HomeViewModel(_navigationStore);
            ViewModel vm3 = new HomeViewModel(_navigationStore);
            _navigationStore.CurrentViewModel = vm1;
            _navigationStore.CurrentViewModel = vm2;
            _navigationStore.CurrentViewModel = vm3;
            // because vm3 is the current one, vm2 is the previous one
            ViewModel result = _navigationStore.Back();
            Assert.AreEqual(vm2, result);
        }

        /// <summary>
        /// When multiple viewmodels are in the history, and back is ran multiple times, it should still return the correct one.
        /// </summary>
        [Test]
        public void Back_RanTwice_ReturnsViewModelVisitedTwiceAgo()
        {
            ViewModel vm1 = new HomeViewModel(_navigationStore);
            ViewModel vm2 = new StepByStepListViewModel(_navigationStore);
            ViewModel vm3 = new DetectorViewModel(_navigationStore);
            _navigationStore.CurrentViewModel = vm1;
            _navigationStore.CurrentViewModel = vm2;
            _navigationStore.CurrentViewModel = vm3;
            // because vm3 is the current one, vm1 will be the new current one when we go back twice
            _navigationStore.Back();
            ViewModel result = _navigationStore.Back();
            Assert.AreEqual(vm1, result);
        }

        /// <summary>
        /// When there's no viewmodel to go back to, null should be returned.
        /// </summary>
        [Test]
        public void Back_WithNoPreviousViewModels_ReturnsNull()
        {
            ViewModel result = _navigationStore.Back();
            Assert.AreEqual(null, result);
        }

    }
}
