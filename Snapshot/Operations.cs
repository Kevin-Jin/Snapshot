using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snapshot
{
    internal static class Operations
    {
        private static Task<Tuple<int, string>> GetOutput(string processFileName, string arguments)
        {
            var startInfo = new ProcessStartInfo();
            startInfo.RedirectStandardError = startInfo.RedirectStandardOutput = startInfo.RedirectStandardInput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            var completionSource = new TaskCompletionSource<Tuple<int, string>>();
            var output = new StringBuilder();
            var outputReceived = new DataReceivedEventHandler((sendingProcess, outLine) => output.Append(outLine.Data).Append('\n'));

            Process p = new Process();
            startInfo.FileName = processFileName;
            startInfo.Arguments = arguments;
            p.StartInfo = startInfo;
            p.EnableRaisingEvents = true;
            p.OutputDataReceived += outputReceived;
            p.ErrorDataReceived += outputReceived;
            p.Exited += ((sender, e) =>
            {
                using (Process exitedProc = sender as Process)
                    completionSource.SetResult(new Tuple<int, string>(exitedProc.ExitCode, output.ToString()));
            });

            try
            {
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
            }
            catch (Exception e)
            {
                completionSource.SetException(e);
            }
            return completionSource.Task;
        }

        internal static IList<string> GetOpenedProcesses(ISet<string> processesToFind)
        {
            return Process.GetProcesses().Select(result => result.ProcessName).Intersect(processesToFind).ToList();
        }

        internal static async Task<IList<FileInfo>> GetFilesOpenedByProcess(string processName)
        {
            var output = await GetOutput("handle.exe", "-p " + processName);
            return null;
        }
    }
}
