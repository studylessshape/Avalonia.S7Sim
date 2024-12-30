using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Diagnostics;
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

        private readonly object stdOutLock = new();

        public void SetProcess(Process process)
        {
            SubProcess = process;
            SubProcess.StartInfo.UseShellExecute = false;
            SubProcess.StartInfo.CreateNoWindow = true;
            SubProcess.StartInfo.RedirectStandardError = true;
            SubProcess.StartInfo.RedirectStandardInput = true;
            SubProcess.StartInfo.RedirectStandardOutput = true;
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
                Stop();
                lock (stdOutLock)
                {
                    StdOut += "\nThis window will be closed in 5 Seconds...";
                }
                Task.Delay(TimeSpan.FromSeconds(5)).Wait();
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
