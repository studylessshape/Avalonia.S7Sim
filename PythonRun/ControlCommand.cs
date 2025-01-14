using PipeProtocol.Client;

namespace PythonRun
{
    public class ControlCommand : IControlCommand
    {
        private CancellationTokenSource _cancellationTokenSource;
        public CancellationToken StopToken => _cancellationTokenSource.Token;
        public ControlCommand()
        {
            _cancellationTokenSource = new();
        }

        public void Stop()
        {
            try
            {
                _cancellationTokenSource.Cancel();
            }
            catch (Exception)
            {
            }
        }

        public async Task StopAsync()
        {
            try
            {
                await _cancellationTokenSource.CancelAsync();
            }
            catch (Exception)
            {
            }
        }
    }
}
