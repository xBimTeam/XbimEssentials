using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Xbim.Essentials.NetCore.Tests
{
    internal static class ProcessTaskHelpers
    {

        /// <summary>
        /// Runs an external process and returns the combined stdout/stderr output.
        /// </summary>
        /// <param name="fileName">The executable file name or path.</param>
        /// <param name="arguments">Command line arguments.</param>
        /// <param name="timeoutMs">Timeout in milliseconds (default 60 seconds).</param>
        /// <returns>Combined stdout and stderr output.</returns>
        public static async Task<string> RunProcessAsync(string fileName, string arguments, int timeoutMs = 60000)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null)
            {
                throw new InvalidOperationException($"Failed to start process: {fileName}");
            }

            var stdOutTask = process.StandardOutput.ReadToEndAsync();
            var stdErrTask = process.StandardError.ReadToEndAsync();

            await Task.WhenAll(stdOutTask, stdErrTask);
            await process.WaitForExitAsync();

            var stdOut = await stdOutTask;
            var stdErr = await stdErrTask;

            return stdOut + (string.IsNullOrEmpty(stdErr)
                ? string.Empty
                : (Environment.NewLine + "ERROR:" + Environment.NewLine + stdErr));
        }
    }
}