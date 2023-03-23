#region

using System;
using System.Diagnostics;
using System.IO;

using EnvDTE;

using Process = System.Diagnostics.Process;

#endregion

namespace PatternPal.Extension.Grpc
{
    internal static class GrpcBackgroundServiceHelper
    {
        private static Process _backgroundService;

        internal static void StartBackgroundService(
            DTE dte)
        {
            string backgroundServicePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "PatternPal",
                "PatternPal.exe");
            _backgroundService = Process.Start(
                new ProcessStartInfo(backgroundServicePath)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                });

            if (null == _backgroundService)
            {
                throw new Exception("Unable to start background service");
            }
        }

        internal static void KillBackgroundService()
        {
            _backgroundService.Kill();
        }
    }
}
