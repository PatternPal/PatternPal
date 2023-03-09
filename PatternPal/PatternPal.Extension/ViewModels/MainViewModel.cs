using System.Net.Http;
using System.Windows;
using System.Windows.Input;

using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;

using PatternPal.Extension.Stores;
using PatternPal.Extension.Commands;
using PatternPal.Protos;

namespace PatternPal.Extension.ViewModels
{
    public class MainViewModel : ViewModel
    {
        public MainViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
            BackCommand = new BackCommand(navigationStore);

            GrpcChannel channel = GrpcChannel.ForAddress(
                "http://localhost:5000",
                new GrpcChannelOptions
                {
                    HttpHandler = new GrpcWebHandler(new HttpClientHandler()),
                });

            Protos.PatternPal.PatternPalClient client = new Protos.PatternPal.PatternPalClient(channel);
            IAsyncStreamReader< RecognizerResult > responseStream = client.Recognize(
                new RecognizeRequest
                {
                    File = "Hoi", Recognizer = Recognizer.All,
                }).ResponseStream;

            if (responseStream.MoveNext().Result)
            {
                RecognizerResult response = responseStream.Current;
            }
        }

        /// <summary>
        /// Navigation Store used for switching between views
        /// </summary>
        private NavigationStore _navigationStore { get; }

        public ViewModel CurrentViewModel
        {
            get => _navigationStore.CurrentViewModel;
            set => _navigationStore.CurrentViewModel = value;
        }

        public override string Title => "PatternPal";

        public ICommand BackCommand { get; }
        public Visibility BackButtonVisibility
        {
            get => CurrentViewModel.GetType() == typeof(HomeViewModel)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        /// <summary>
        /// Update view and back button when the CurrentViewModel is changed
        /// </summary>
        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
            OnPropertyChanged(nameof(BackButtonVisibility));
        }
    }
}
