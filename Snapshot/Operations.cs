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

        internal static List<string> GetOpenedProcesses(IEnumerable<string> processesToFind = null)
        {
            //MainWindowTitle is null if the application does not have a window
            if (processesToFind != null)
                return Process.GetProcesses().Where(result => !string.IsNullOrWhiteSpace(result.MainWindowTitle)).Select(result => result.ProcessName).Intersect(processesToFind).ToList();
            else
                return Process.GetProcesses().Where(result => !string.IsNullOrWhiteSpace(result.MainWindowTitle)).Select(result => result.ProcessName).ToList();
        }

        internal static async Task<List<Tuple<string, string>>> GetFilesOpenedByProcess(string processName)
        {
            var entries = new List<Tuple<string, string>>();
            var extensions = ApplicationConfig.Instance.ExtensionAssociations.ContainsKey(processName.ToLower()) ? ApplicationConfig.Instance.ExtensionAssociations[processName.ToLower()] : null;
            if (extensions == null)
                return entries;
            var task = GetOutput("handle.exe", "-p " + processName);
            var output = await task;
            var beginFilesList = new Regex("------------------------------------------------------------------------------\r?\n.*?\r?\n", RegexOptions.Multiline | RegexOptions.Compiled);
            var eachFile = new Regex("\\s*?: File  \\((?<permissions>.*?)\\)   (?<file>.*?)\r?\n", RegexOptions.Multiline | RegexOptions.Compiled);
            var sections = beginFilesList.Split(output.Item2); //[0] is copyright info, [1] is actual output...
            if (sections.Length > 1)
                for (var match = eachFile.Match(beginFilesList.Split(output.Item2)[1]); match.Success; match = match.NextMatch())
                    if (extensions.Contains(match.Groups["file"].Value.Substring(match.Groups["file"].Value.LastIndexOf('.') + 1).ToLower()))
                        entries.Add(new Tuple<string, string>(match.Groups["permissions"].Value, match.Groups["file"].Value));
            return entries;
        }
    }
}
