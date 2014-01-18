using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            var p = new Process();
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

        internal static IList<string> GetOpenedProcesses(IEnumerable<string> processesToFind)
        {
            return Process.GetProcesses().Select(result => result.ProcessName).Intersect(processesToFind).ToList();
        }

        internal static async Task<IList<Tuple<string, FileInfo>>> GetFilesOpenedByProcess(string processName)
        {
            var output = await GetOutput("handle.exe", "-p " + processName);
            var beginFilesList = new Regex("------------------------------------------------------------------------------\r?\n.*?\r?\n", RegexOptions.Multiline | RegexOptions.Compiled);
            var eachFile = new Regex("\\s*?: File  \\((?<permissions>.*?)\\)   (?<file>.*?)\r?\n", RegexOptions.Multiline | RegexOptions.Compiled);
            var entries = new List<Tuple<string, FileInfo>>();
            var sections = beginFilesList.Split(output.Item2); //[0] is copyright info, [1] is actual output...
            if (sections.Length > 1)
                for (var match = eachFile.Match(beginFilesList.Split(output.Item2)[1]); match.Success; match = match.NextMatch())
                    entries.Add(new Tuple<string, FileInfo>(match.Groups["permissions"].Value, new FileInfo(match.Groups["file"].Value)));
            return entries;
        }
    }
}
