#region

using System;
using System.Diagnostics;
using System.IO;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

#endregion

namespace PatternPal.Extension.Grpc
{
    // TODO Comment
    internal static class GrpcBackgroundServiceHelper
    {
        private static Process _backgroundService;

        internal static void StartBackgroundService()
        {
            try
            {
                Uri uri = new Uri(
                    typeof( ExtensionWindowPackage ).Assembly.CodeBase,
                    UriKind.Absolute);

                string extensionDirectory = Path.GetDirectoryName(uri.LocalPath);
                if (string.IsNullOrWhiteSpace(extensionDirectory))
                {
                    throw new Exception("Unable to find extension installation directory");
                }

                string backgroundServicePath = Path.Combine(
                    extensionDirectory,
                    "PatternPal",
                    "PatternPal.exe");

                _backgroundService = Process.Start(
                    new ProcessStartInfo(backgroundServicePath)
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                    });
            }
            catch (Exception exception)
            {
                // TODO: Improve error handling
                VsShellUtilities.ShowMessageBox(
                    ExtensionWindowPackage.PackageInstance,
                    $"The background service failed to start: {exception.Message}",
                    "Background service error",
                    OLEMSGICON.OLEMSGICON_CRITICAL,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }

        internal static void KillBackgroundService()
        {
            if (_backgroundService.HasExited)
            {
                return;
            }

            _backgroundService.Kill();
        }
    }
}
