using System;
using System.Reflection;

namespace GameDebug
{
    public class UnityDebugConsole : IDebugConsole
    {
        private readonly object[] args = new object[]
        {
            string.Empty,
        };

        private readonly MethodInfo logMethodInfo;
        private readonly MethodInfo logWarningMethodInfo;
        private readonly MethodInfo logErrorMethodInfo;

        public UnityDebugConsole()
        {
            Type type = Type.GetType("UnityEngine.Debug, UnityEngine");
            if (type != null)
            {
                this.logMethodInfo = type.GetMethod("Log", new Type[1]
                {
                    typeof (object)
                });
                this.logWarningMethodInfo = type.GetMethod("LogWarning", new Type[1]
                {
                    typeof (object)
                });
                this.logErrorMethodInfo = type.GetMethod("LogError", new Type[1]
                {
                    typeof (object)
                });
            }
        }
        
        public void Log(string message, object context = null)
        {
            this.args[0] = message;
            this.logMethodInfo.Invoke(null, this.args);
        }

        public void LogWarning(string message, object context = null)
        {
            this.args[0] = message;
            this.logWarningMethodInfo.Invoke(null, this.args);
        }

        public void LogError(string message, object context = null)
        {
            this.args[0] = message;
            this.logErrorMethodInfo.Invoke(null, this.args);
        }
    }
}