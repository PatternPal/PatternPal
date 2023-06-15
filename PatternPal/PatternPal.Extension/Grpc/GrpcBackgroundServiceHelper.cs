﻿#region

using System;
using System.Diagnostics;
using System.IO;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

#endregion

namespace PatternPal.Extension.Grpc
{
    /// <summary>
    /// Helper class for starting and stopping the background service.
    /// </summary>
    internal static class GrpcBackgroundServiceHelper
    {
        private static Process _backgroundService;

        /// <summary>
        /// Starts the background service. This method is called when the extension is loaded.
        /// </summary>
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
                VsShellUtilities.ShowMessageBox(
                    ExtensionWindowPackage.PackageInstance,
                    $"The background service failed to start: {exception.Message}",
                    "Background service error",
                    OLEMSGICON.OLEMSGICON_CRITICAL,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }
        /// <summary>
        ///  Kills the background service. This method is called when the extension is unloaded.
        /// </summary>
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