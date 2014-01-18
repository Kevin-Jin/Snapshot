using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snapshot
{
    static class Program
    {
        [SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        private static async void DebugOutput()
        {
            var openedFiles = await Operations.GetFilesOpenedByProcess("winword");
            if (!openedFiles.Any())
                Console.WriteLine("(No files opened by winword)");
            foreach (var entry in openedFiles)
                Console.WriteLine(entry.Item1 + ": " + entry.Item2);

            Console.WriteLine(ApplicationConfig.Instance.Folder);
            Console.WriteLine(ApplicationConfig.Instance);
            new ProjectConfig(new Dictionary<string, List<string>>() { { "test", new List<string>() { "test" } } });
            //Console.WriteLine(new ProjectConfig("test.json"));
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AllocConsole();

            DebugOutput();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
