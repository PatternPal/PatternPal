#region

using System;
using System.Diagnostics;
using System.Net.Http;

using Grpc.Net.Client;
using Grpc.Net.Client.Web;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using PatternPal.Protos;

#endregion

namespace PatternPal.Extension.Grpc
{
    internal static class GrpcHelper
    {
        internal static GrpcChannel Channel { get; }
        internal static RecognizerService.RecognizerServiceClient RecognizerClient { get; }
        internal static StepByStepService.StepByStepServiceClient StepByStepClient { get; }

        static GrpcHelper()
        {
            Channel = GrpcChannel.ForAddress(
                "http://localhost:5000",
                new GrpcChannelOptions
                {
                    HttpHandler = new GrpcWebHandler(new HttpClientHandler()),
                });

            RecognizerClient = new RecognizerService.RecognizerServiceClient(Channel);
            StepByStepClient = new StepByStepService.StepByStepServiceClient(Channel);
        }

        #region InfoBar

        private static IVsInfoBarUIElement _currentInfoBarElement;

        internal static void ShowErrorMessage(
            string errorMessage)
        {
            const string LOG_SOURCE = "PatternPal";

            ThreadHelper.ThrowIfNotOnUIThread();

            IVsInfoBarUIFactory infoBarFactory = (IVsInfoBarUIFactory)Package.GetGlobalService(typeof( SVsInfoBarUIFactory ));
            IVsUIShell shell = (IVsUIShell)Package.GetGlobalService(typeof( SVsUIShell ));

            _currentInfoBarElement?.Close();

            if (ErrorHandler.Failed(
                    shell.FindToolWindow(
                        (uint)__VSFINDTOOLWIN.FTW_fFrameOnly,
                        typeof( ExtensionWindow ).GUID,
                        out IVsWindowFrame frame))
                || frame is null)
            {
                VsShellUtilities.LogError(
                    LOG_SOURCE,
                    $"Failed to find tool window to attach error: {errorMessage}");
                return;
            }

            if (ErrorHandler.Failed(
                frame.GetProperty(
                    (int)__VSFPROPID7.VSFPROPID_InfoBarHost,
                    out object infoBarHost)))
            {
                VsShellUtilities.LogError(
                    LOG_SOURCE,
                    $"Failed to find info bar host to attach error: {errorMessage}");
                return;
            }

            void RestartBackgroundService() => GrpcBackgroundServiceHelper.StartBackgroundService();
            IVsInfoBar infoBar = BackgroundProcessIsRunning()
                ? new InfoBarModel(
                    new[ ]
                    {
                        new InfoBarTextSpan(errorMessage)
                    },
                    KnownMonikers.StatusError)
                : new InfoBarModel(
                    new[ ]
                    {
                        new InfoBarTextSpan("PatternPal has crashed")
                    },
                    new[ ]
                    {
                        new InfoBarHyperlink(
                            "Restart",
                            (Action)RestartBackgroundService)
                    },
                    KnownMonikers.StatusError);

            _currentInfoBarElement = infoBarFactory.CreateInfoBar(infoBar);

            uint eventCookie = 0;

            // ReSharper disable once AccessToModifiedClosure
            void OnClose() => _currentInfoBarElement?.Unadvise(eventCookie);

            _currentInfoBarElement?.Advise(
                new InfoBarUIEvents(OnClose),
                out eventCookie);

            ((IVsInfoBarHost)infoBarHost).AddInfoBar(_currentInfoBarElement);
        }

        private static bool BackgroundProcessIsRunning() => Process.GetProcessesByName("PatternPal").Length > 0;

        private class InfoBarUIEvents : IVsInfoBarUIEvents
        {
            private readonly Action _onClose;

            public InfoBarUIEvents(
                Action onClose)
            {
                _onClose = onClose;
            }

            public void OnClosed(
                IVsInfoBarUIElement infoBarUIElement)
            {
                _onClose();
            }

            public void OnActionItemClicked(
                IVsInfoBarUIElement infoBarUIElement,
                IVsInfoBarActionItem actionItem)
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                (actionItem.ActionContext as Action)?.Invoke();
                infoBarUIElement.Close();
            }
        }

        #endregion
    }
}
