namespace GameDebug
{
    public interface IDebugConsole
    {
        void Log(string message, object context = null);
        void LogWarning(string message, object context = null);
        void LogError(string message, object context = null);
    }
}