namespace S7Sim.Services
{
    public interface IShellCommand
    {
        string AcceptInputString(string label);
        int AcceptInputInt(string label);
        float AcceptInputFloat(string label);
        void ShowMessageBox(string message, int? icon = null);
        void SendLogMessage(string log, int level = 0);
    }
}
