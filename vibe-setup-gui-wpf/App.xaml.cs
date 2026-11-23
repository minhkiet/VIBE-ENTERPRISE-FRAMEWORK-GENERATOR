using System.Diagnostics;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CursorSetupWpf.Services;
using CursorSetupWpf.ViewModels;

namespace CursorSetupWpf
{
    public partial class App : Application
    {
        static readonly string LogPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CursorSetup", "cursor-setup-debug.log");

        internal static void Log(string msg)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(LogPath)!);
                File.AppendAllText(LogPath, $"[{DateTime.Now:HH:mm:ss.fff}] {msg}\n");
            }
            catch { }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Log("OnStartup START");

            // Global exception handlers
            AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            {
                Log($"FATAL [AppDomain]: {args.ExceptionObject}");
                Environment.Exit(1);
            };
            DispatcherUnhandledException += (_, args) =>
            {
                Log($"FATAL [Dispatcher]: {args.Exception}");
                MessageBox.Show($"Unhandled error:\n{args.Exception}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };
            TaskScheduler.UnobservedTaskException += (_, args) =>
            {
                Log($"ERROR [Task]: {args.Exception}");
                args.SetObserved();
            };

            base.OnStartup(e);
            var automation = AutomationArgs.Parse(e.Args);
            Log("AUTO args=" + string.Join(" ", e.Args));
            Log($"AUTO target={automation.Target} install={automation.AutoInstall} noDialogs={automation.NoDialogs}");
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (MainWindow?.DataContext is MainViewModel vm)
                {
                    vm.ApplyAutomation(automation);
                    Log("AUTO vm applied");
                    if (automation.AutoInstall && !string.IsNullOrWhiteSpace(automation.Target))
                    {
                        Log("AUTO install requested");
                        _ = vm.RunAutomatedInstallAsync();
                    }
                }
                else
                {
                    Log("AUTO vm unavailable");
                }
            }), DispatcherPriority.Loaded);
            Log("OnStartup END - MainWindow should now be visible");
        }
    }
}
