#region

using System.ComponentModel;
using System.Runtime.CompilerServices;

using PatternPal.Protos;

#endregion

namespace PatternPal.Extension.ViewModels
{
    /// <summary>
    /// Represents a design pattern for which a recognizer exists.
    /// </summary>
    public class DesignPatternViewModel : INotifyPropertyChanged
    {
        private bool _isChecked;

        /// <summary>
        /// Creates a new instance of the <see cref="DesignPatternViewModel"/> class.
        /// </summary>
        /// <param name="recognizer">The <see cref="Protos.Recognizer"/> which this view model represents.</param>
        public DesignPatternViewModel(
            Recognizer recognizer)
        {
            Recognizer = recognizer;
            IsChecked = true;
        }

        /// <summary>
        /// The <see cref="Protos.Recognizer"/> which this view model represents.
        /// </summary>
        public Recognizer Recognizer { get; }

        /// <summary>
        /// The name of the <see cref="Protos.Recognizer"/>, used in the selection box.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public string Name => Recognizer.ToString();

        /// <summary>
        /// Whether this view model has been selected.
        /// </summary>
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invokes the <see cref="PropertyChanged"/> event.
        /// </summary>
        private void OnPropertyChanged(
            [CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
