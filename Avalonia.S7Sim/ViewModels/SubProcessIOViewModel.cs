using Avalonia.Controls;
using Avalonia.S7Sim.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Avalonia.S7Sim.ViewModels
{
    public partial class SubProcessIOViewModel : ViewModelBase
    {
        public Process? SubProcess { get; private set; }
        private CancellationTokenSource tokenSource = new();

        public event Action? CloseWindow;

        [ObservableProperty]
        private string stdOut = "";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanExecuteControl))]
        [NotifyCanExecuteChangedFor(nameof(StopCCommand))]
        [NotifyCanExecuteChangedFor(nameof(ReStartCommand))]
        private bool inProcess;

        public bool CanExecuteControl => !InProcess;

        public delegate void OnStringValueChangedDelegate(string? oldValue, string newValue);
        public event OnStringValueChangedDelegate? OnStdOutChangedEvent;

        private readonly object stdOutLock = new();
        public object StdOutLock => stdOutLock;

        public TextBox? LogBox { get; set; }

        private readonly ScriptsViewModel? scriptsViewModel;
        private readonly PipeHost? pipeHost;
        private bool forceExit = false;
        private bool isRestart = false;
        private ControlCommand? controlCommand;
        private string? filePath;
        private string? pipeName;
        private Task? _task;

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
            tokenSource = new();

            Process process = new();
            // Program file
            process.StartInfo.FileName = "PythonRun.exe";
            // Script file path
            process.StartInfo.ArgumentList.Add("-f");
            process.StartInfo.ArgumentList.Add(filePath);
            this.filePath = filePath;
            // Namedpipe target
            pipeName = GenPipeName();
            process.StartInfo.ArgumentList.Add("-n");
            process.StartInfo.ArgumentList.Add(pipeName);
            // Remote stop command by namedpipe
            controlCommand = new ControlCommand($"{pipeName}_py");

            StdOut += $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss} Info] NamedPipe run on name of '{pipeName}'{Environment.NewLine}";
            // Search path
            process.StartInfo.ArgumentList.Add("-s");
            process.StartInfo.ArgumentList.AddRange(scriptsViewModel?.EngineSearchPaths ?? []);

            #region Process Initialize
            SubProcess = process;
            SubProcess.StartInfo.UseShellExecute = false;
            SubProcess.StartInfo.CreateNoWindow = true;
            SubProcess.StartInfo.RedirectStandardError = true;
            SubProcess.StartInfo.RedirectStandardInput = true;
            SubProcess.StartInfo.RedirectStandardOutput = true;
            #endregion

            StdOut += $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss} Info] {process.StartInfo.FileName} {string.Join(' ', process.StartInfo.ArgumentList)}{Environment.NewLine}";

            isRestart = false;

            SubProcess.Start();
            StartUpdate();
        }

        public void StartUpdate()
        {
            _task = Task.Run(async () =>
            {
                _ = UpdateStandardOut(tokenSource.Token);
                _ = UpdateStandardError(tokenSource.Token);
                _ = pipeHost?.RunOnTaskAsync(pipeName, tokenSource.Token);
                var processTask = SubProcess?.WaitForExitAsync(tokenSource.Token);
                if (processTask != null)
                {
                    await processTask;
                }
                if (!forceExit)
                {
                    await StopAsync();
                    if (!isRestart)
                    {
                        lock (stdOutLock)
                        {
                            StdOut += "\nThis window will be closed in 5 Seconds...";
                        }
                        Task.Delay(TimeSpan.FromSeconds(5)).Wait();
                    }
                }
                if (!isRestart)
                {
                    CloseWindow?.Invoke();
                }
            }, tokenSource.Token);
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
                        lock (StdOutLock)
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
                        lock (StdOutLock)
                        {
                            StdOut += new string(buffer.ToArray()[..readLength]);
                        }
                    }
                }
            }
        }

        void ProcessExit(bool force = false)
        {
            try
            {
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



        public void Stop(bool force = false)
        {
            try
            {
                forceExit = force;
                if (SubProcess?.HasExited == false)
                {
                    controlCommand?.Stop();
                }
                tokenSource.Cancel();
                tokenSource.Dispose();
            }
            catch (Exception)
            {
            }

            ProcessExit(force);
        }

        public async Task StopAsync(bool force = false)
        {
            try
            {
                forceExit = force;
                if (controlCommand != null && SubProcess?.HasExited == false)
                {
                    await controlCommand.StopAsync();
                }
                await tokenSource.CancelAsync();
            }
            catch (Exception)
            {
            }

            ProcessExit(force);
        }

        [RelayCommand(CanExecute = nameof(CanExecuteControl))]
        public async Task StopC()
        {
            InProcess = true;

            await StopAsync();

            InProcess = false;
        }

        [RelayCommand]
        public async Task Kill()
        {
            InProcess = true;
            await StopAsync(true);
            InProcess = false;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteControl))]
        private async Task ReStartAsync()
        {
            InProcess = true;
            isRestart = true;
            await StopAsync();

            StdOut += $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss} Warn] Restart .....\r\n";

            if (filePath != null)
            {
                StartScript(filePath);
            }
            InProcess = false;
        }
    }
}
