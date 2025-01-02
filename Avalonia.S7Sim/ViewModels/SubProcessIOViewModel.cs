using Avalonia.Controls;
using Avalonia.S7Sim.Services;
using Avalonia.S7Sim.Services.Shell;
using CommunityToolkit.Mvvm.ComponentModel;
using IronPython.Runtime.Operations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Scripting.Utils;
using S7Sim.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Avalonia.S7Sim.ViewModels
{
    public partial class SubProcessIOViewModel : ViewModelBase
    {
        public Process? SubProcess { get; private set; }
        private readonly CancellationTokenSource tokenSource = new();

        public event Action? CloseWindow;

        [ObservableProperty]
        private string stdOut = "";

        public delegate void OnStringValueChangedDelegate(string? oldValue, string newValue);
        public event OnStringValueChangedDelegate? OnStdOutChangedEvent;

        private readonly object stdOutLock = new();
        private readonly ScriptsViewModel? scriptsViewModel;
        private readonly PipeHost? pipeHost;
        private readonly IS7DataBlockService? s7DataBlockService;
        private bool forceExit = false;

        public SubProcessIOViewModel()
        {
            var serviceProvider = App.AppCurrent?.ServiceProvider;
            scriptsViewModel = serviceProvider?.GetRequiredService<ScriptsViewModel>();
            pipeHost = serviceProvider?.GetRequiredService<PipeHost>();

        }

        public void SetOwnerWindow(Window? window)
        {
            if (pipeHost?.Shell is ShellCommand shellCommand)
            {
                shellCommand.SetOwner(window);
            }
        }

        partial void OnStdOutChanged(string? oldValue, string newValue)
        {
            OnStdOutChangedEvent?.Invoke(oldValue, newValue);
        }

        string GenPipeName()
        {
            Process current = Process.GetCurrentProcess();
            return $"{current.Id}{current.MachineName}{current.ProcessName}{Random.Shared.Next()}";
        }

        public void StartScript(string filePath)
        {
            Process process = new Process();
            process.StartInfo.FileName = "PythonRun.exe";

            process.StartInfo.ArgumentList.Add("-f");
            process.StartInfo.ArgumentList.Add(filePath);

            string pipeName = GenPipeName();
            process.StartInfo.ArgumentList.Add("-n");
            process.StartInfo.ArgumentList.Add(pipeName);

            pipeHost?.RunAsync(pipeName, tokenSource.Token);
            StdOut += $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss} Info] NamedPipe run on name of '{pipeName}'{Environment.NewLine}";
            process.StartInfo.ArgumentList.Add("-s");
            process.StartInfo.ArgumentList.AddRange(scriptsViewModel?.EngineSearchPaths.Select(path => path.Path) ?? []);

            SubProcess = process;
            SubProcess.StartInfo.UseShellExecute = false;
            SubProcess.StartInfo.CreateNoWindow = true;
            SubProcess.StartInfo.RedirectStandardError = true;
            SubProcess.StartInfo.RedirectStandardInput = true;
            SubProcess.StartInfo.RedirectStandardOutput = true;
            StdOut += $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss} Info] {process.StartInfo.FileName} {string.Join(' ', process.StartInfo.ArgumentList)}{Environment.NewLine}";
            SubProcess.Start();
            StartUpdate();
        }

        public void StartUpdate()
        {
            var thread = new Thread(() =>
            {
                _ = UpdateStandardOut(tokenSource.Token);
                _ = UpdateStandardError(tokenSource.Token);
                SubProcess?.WaitForExit();
                if (!forceExit)
                {
                    Stop();
                    lock (stdOutLock)
                    {
                        StdOut += "\nThis window will be closed in 5 Seconds...";
                    }
                    Task.Delay(TimeSpan.FromSeconds(5)).Wait();
                }
                CloseWindow?.Invoke();
            });
            thread.Start();
        }

        async Task UpdateStandardOut(CancellationToken stopToken)
        {
            while (!stopToken.IsCancellationRequested)
            {
                if (SubProcess != null)
                {
                    var buffer = new char[1024].AsMemory();
                    var readLength = await SubProcess.StandardOutput.ReadAsync(buffer, stopToken).ConfigureAwait(false);
                    if (readLength > 0)
                    {
                        lock (stdOutLock)
                        {
                            StdOut += new string(buffer.ToArray()[..readLength]);
                        }
                    }
                }
            }
        }

        async Task UpdateStandardError(CancellationToken stopToken)
        {
            while (!stopToken.IsCancellationRequested)
            {
                if (SubProcess != null)
                {
                    var buffer = new char[1024].AsMemory();
                    var readLength = await SubProcess.StandardError.ReadAsync(buffer, stopToken);
                    if (readLength > 0)
                    {
                        lock (stdOutLock)
                        {
                            StdOut += new string(buffer.ToArray()[..readLength]);
                        }
                    }
                }
            }
        }

        public void Stop(bool force = false)
        {
            try
            {
                forceExit = force;
                tokenSource.Cancel();
                tokenSource.Dispose();
                if (force)
                {
                    SubProcess?.Kill();
                }
                else
                {
                    SubProcess?.WaitForExit();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
