#region

using System;
using System.Diagnostics;
using System.IO;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

#endregion

namespace PatternPal.Extension.Grpc
{
/// <summary>
/// Helper class for managing a background service process.
/// </summary>
public static class GrpcBackgroundServiceHelper
{
    private static Process _backgroundService;

    /// <summary>
    /// Starts the background service process.
    /// </summary>
    /// <param name="executablePath">The path to the executable file.</param>
    /// <param name="arguments">The command line arguments.</param>
    internal static void StartBackgroundService(string executablePath, string arguments)
    {
        try
        {
            _backgroundService = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = executablePath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            _backgroundService.Start();
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
    /// Kills the background service process.
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
