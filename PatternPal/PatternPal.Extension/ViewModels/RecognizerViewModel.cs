#region

using PatternPal.Extension.Resources;

#endregion

namespace PatternPal.Extension.ViewModels
{
    /// <summary>
    /// The view model for the recognize window.
    /// </summary>
    public class RecognizerViewModel : ViewModel
    {
        /// <inheritdoc />
        public override string Title => ExtensionUIResources.DetectorTitle;
    }
}
