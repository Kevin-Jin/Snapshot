using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snapshot
{
    internal static class Operations
    {
        private const UInt32 WM_CLOSE = 0x0010;

        public delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool EnumThreadWindows(uint dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        internal static Task<Tuple<int, string>> GetOutput(string processFileName, string arguments)
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

        internal static string ExecutablePath(this Process process)
        {
            try
            {
                return process.MainModule.FileName;
            }
            catch
            {
                var query = "SELECT ExecutablePath, ProcessID FROM Win32_Process";
                var searcher = new ManagementObjectSearcher(query);

                foreach (ManagementObject item in searcher.Get())
                {
                    var id = item["ProcessID"];
                    var path = item["ExecutablePath"];

                    if (path != null && id.ToString() == process.Id.ToString())
                        return path.ToString();
                }
            }

            return "";
        }

        internal static void CloseWindow(this Process proc)
        {
            if (proc.MainWindowHandle == IntPtr.Zero)
            {
                foreach (ProcessThread pt in proc.Threads)
                {
                    EnumThreadWindows((uint)pt.Id, new EnumThreadDelegate((IntPtr hWnd, IntPtr lParam) =>
                    {
                        PostMessage(hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                        return true;
                    }), IntPtr.Zero);
                }
            }
            else if (proc.CloseMainWindow())
            {
                proc.Close();
            }
        }

        internal static List<Tuple<string, Action>> GetOpenedProcesses()
        {
            //MainWindowTitle is null if the application does not have a window
            var processes = Process.GetProcesses().Where(result => !string.IsNullOrWhiteSpace(result.MainWindowTitle));
            return new List<Tuple<string, Action>>(processes.Select(result => new Tuple<string, Action>(result.ExecutablePath(), result.CloseWindow)).ToList());
        }

        internal static async Task<List<Tuple<string, string>>> GetFilesOpenedByProcess(string processName)
        {
            var entries = new List<Tuple<string, string>>();
            var extensions = ApplicationConfig.Instance.GetExtensionAssociations(processName.ToLower());
            if (extensions == null)
                return entries;
            var exclusions = ApplicationConfig.Instance.GetExclusions(processName.ToLower());
            var task = GetOutput("handle.exe", "-accepteula -p " + processName);
            var output = await task;
            var beginFilesList = new Regex("------------------------------------------------------------------------------\r?\n.*?\r?\n", RegexOptions.Multiline | RegexOptions.Compiled);
            var eachFile = new Regex("\\s*?: File  \\((?<permissions>.*?)\\)   (?<file>.*?)\r?\n", RegexOptions.Multiline | RegexOptions.Compiled);
            var sections = beginFilesList.Split(output.Item2); //[0] is copyright info, [1] is actual output...
            if (sections.Length > 1)
                for (var match = eachFile.Match(beginFilesList.Split(output.Item2)[1]); match.Success; match = match.NextMatch())
                    if (extensions.Contains(match.Groups["file"].Value.Substring(match.Groups["file"].Value.LastIndexOf('.') + 1).ToLower()) && (exclusions != null && !exclusions.Any(regex => regex.IsMatch(match.Groups["file"].Value))))
                        entries.Add(new Tuple<string, string>(match.Groups["permissions"].Value, match.Groups["file"].Value));
            return entries;
        }

        internal static void OpenInternetExplorerTabs(List<string> urls)
        {
            if (urls.Any())
            {
                var ie = new SHDocVw.InternetExplorer();
                ie.Navigate2(urls[0]);
                ie.Width = Screen.PrimaryScreen.WorkingArea.Width;
                ie.Height = Screen.PrimaryScreen.WorkingArea.Height;
                ie.Left = ie.Top = 0;
                for (var i = 1; i < urls.Count; i++)
                    ie.Navigate2(urls[i], 0x800);
                ie.Visible = true;
            }
        }

        internal static List<string> GetInternetExplorerUrls()
        {
            var list = new List<string>();
            foreach (SHDocVw.InternetExplorer ieTab in new SHDocVw.ShellWindowsClass())
                if (!string.IsNullOrWhiteSpace(ieTab.LocationURL) && !ieTab.LocationURL.StartsWith("file://"))
                    list.Add(ieTab.LocationURL);
            return list;
        }
    }
}
