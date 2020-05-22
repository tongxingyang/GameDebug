using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;

namespace GameDebug
{
    public static class GameDebug
    {
        public static bool EnableLog = false;
        public static bool EnableLogWarning = false;
        public static bool EnableLogError = false;
        public static bool EnableLogColor = true;
        public static bool EnableShowTime = true;
        public static bool EnableShowTimeStick = true;
        public static bool EnableSaveFile = true;
        public static bool EnableFileStackTrace = true;
        public static string Prefix = "  >>>  ";
        
        private static string LogStringType1 = " :: ";
        private static string LogStringType2 = " () ";
        private static readonly StringBuilder StringBuffer = new StringBuilder();
        private static StreamWriter LogFileWriter = null;
        private static string LogFileDir = string.Empty;
        private static string LogFileName = String.Empty;
        private static IDebugConsole CurrentConsole;
        private static Assembly DebugAssembly;
        private static readonly Dictionary<LogColor,string> Colors = new Dictionary<LogColor, string>();
        private static int MainThreadID;

        public static void Init(int threadId, IDebugConsole debugConsole, string logFileDir = null)
        {
            MainThreadID = threadId;
            CurrentConsole = debugConsole;
            LogFileDir = logFileDir;
            if (string.IsNullOrEmpty(LogFileDir))
            {
                LogFileDir = AppDomain.CurrentDomain.BaseDirectory + "/Log/";
            }
            InitColor();
            LogLogHead();
        }
        
        private static void LogLogHead()
        {
            string str = DateTime.Now.ToString("HH:mm:ss.fff") + " ";
            StringBuffer.Length = 0;
            StringBuffer.Append("===============================================================================");
            Internal_Log(StringBuffer, LogColor.Black);
            StringBuffer.Length = 0;
            StringBuffer.Append("====================================GameDebug===================================");
            Internal_Log(StringBuffer, LogColor.Black);
            StringBuffer.Length = 0;
            StringBuffer.Append("===============================================================================");
            Internal_Log(StringBuffer, LogColor.Black);
            StringBuffer.Length = 0;
            StringBuffer.Append("Time:\t" + str+"");
            Internal_Log(StringBuffer, LogColor.Black);
            StringBuffer.Length = 0;
            StringBuffer.Append("Path:\t" + LogFileDir+"");
            Internal_Log(StringBuffer, LogColor.Black);
            StringBuffer.Length = 0;
            StringBuffer.Append("================================================================================");
            Internal_Log(StringBuffer, LogColor.Black);
        }

        private static void InitColor()
        {
            Colors.Add(LogColor.White, "FFFFFF");
            Colors.Add(LogColor.Green, "00FF00");
            Colors.Add(LogColor.Blue, "99CCFF");
            Colors.Add(LogColor.Red, "FF0000");
            Colors.Add(LogColor.Yellow, "FFFF00");
            Colors.Add(LogColor.Purple, "CC6699");
            Colors.Add(LogColor.Orange, "FF9933");
            Colors.Add(LogColor.Black, "000000");
        }

        private static void Internal_Log(StringBuilder stringBuilder, LogColor logColor = LogColor.White)
        {
            
            if (EnableShowTime)
            {
                stringBuilder.Insert(0, DateTime.Now.ToString("HH:mm:ss.fff") + " ");
            }
          
            if (EnableShowTimeStick)
            {
                if (Thread.CurrentThread.ManagedThreadId == MainThreadID)
                    stringBuilder.Append("  (at Frame: ").Append(Time.frameCount).Append(" sec: ").Append(Time.realtimeSinceStartup.ToString("F3")).Append(" ) ");
                else if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
                    stringBuilder.Append("  (from anonymous thread ").Append(" with id ").Append(Thread.CurrentThread.ManagedThreadId).Append(" ) ");
                else
                    stringBuilder.Append("  (from thread ").Append(Thread.CurrentThread.Name).Append(" with id ").Append(Thread.CurrentThread.ManagedThreadId).Append(" ) ");
            }
            
            if (CurrentConsole is UnityDebugConsole && EnableLogColor)
            {
                stringBuilder.Insert(0, $"<color=#{Colors[logColor]}>");
                stringBuilder.Append("</color>");
            }
            
            if (CurrentConsole != null)
            {
                CurrentConsole.Log(stringBuilder.ToString(), (object) null);
            }
            else
            {
                ConsoleColor foregroundColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(stringBuilder.ToString());
                Console.ForegroundColor = foregroundColor;
            }
            stringBuilder.Append(" [N]: ");
            LogToFile(stringBuilder.ToString());
            stringBuilder.Length = 0;
        }

        private static void Internal_LogWarning(StringBuilder stringBuilder, LogColor logColor = LogColor.Yellow)
        {
                        
            if (EnableShowTime)
            {
                stringBuilder.Insert(0, DateTime.Now.ToString("HH:mm:ss.fff") + "  ");
            }

            if (CurrentConsole is UnityDebugConsole && EnableLogColor)
            {
                stringBuilder.Insert(0, $"<color=#{Colors[logColor]}>");
                stringBuilder.Append("</color>");
            }

            if (EnableShowTimeStick)
            {
                if (Thread.CurrentThread.ManagedThreadId == MainThreadID)
                    stringBuilder.Append(" (at Frame: ").Append(Time.frameCount).Append(" sec: ").Append(Time.realtimeSinceStartup.ToString("F3")).Append(" ) ");
                else if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
                    stringBuilder.Append(" (from anonymous thread ").Append(" with id ").Append(Thread.CurrentThread.ManagedThreadId).Append(" ) ");
                else
                    stringBuilder.Append(" (from thread ").Append(Thread.CurrentThread.Name).Append(" with id ").Append(Thread.CurrentThread.ManagedThreadId).Append(" ) ");
            }
            
            if (CurrentConsole != null)
            {
                CurrentConsole.LogWarning(stringBuilder.ToString(), (object) null);
            }
            else
            {
                ConsoleColor foregroundColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(stringBuilder.ToString());
                Console.ForegroundColor = foregroundColor;
            }
            stringBuilder.Append(" [W]: ");
            LogToFile(stringBuilder.ToString());
            stringBuilder.Length = 0;
        }

        private static void Internal_LogError(StringBuilder stringBuilder, LogColor logColor = LogColor.Red)
        {
                        
            if (EnableShowTime)
            {
                stringBuilder.Insert(0, DateTime.Now.ToString("HH:mm:ss.fff") + "  ");
            }

            if (CurrentConsole is UnityDebugConsole && EnableLogColor)
            {
                stringBuilder.Insert(0, $"<color=#{Colors[logColor]}>");
                stringBuilder.Append("</color>");
            }

            if (EnableShowTimeStick)
            {
                if (Thread.CurrentThread.ManagedThreadId == MainThreadID)
                    stringBuilder.Append(" (at Frame: ").Append(Time.frameCount).Append(" sec: ").Append(Time.realtimeSinceStartup.ToString("F3")).Append(" ) ");
                else if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
                    stringBuilder.Append(" (from anonymous thread ").Append(" with id ").Append(Thread.CurrentThread.ManagedThreadId).Append(" ) ");
                else
                    stringBuilder.Append(" (from thread ").Append(Thread.CurrentThread.Name).Append(" with id ").Append(Thread.CurrentThread.ManagedThreadId).Append(" ) ");
            }
            
            if (CurrentConsole != null)
            {
                CurrentConsole.LogError(stringBuilder.ToString(), (object) null);
            }
            else
            {
                ConsoleColor foregroundColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(stringBuilder.ToString());
                Console.ForegroundColor = foregroundColor;
            }
            stringBuilder.Append(" [E]: ");
            LogToFile(stringBuilder.ToString(), true);
            stringBuilder.Length = 0;
        }
        
        [Conditional("ENABLELOG")]
        public static void Log(object obj, LogColor logColor = LogColor.White)
        {
            if(!EnableLog) return;
            StringBuffer.Length = 0;
            StringBuffer.Append(Prefix).Append(GetLogCaller()).Append(LogStringType2).Append((obj?.ToString() ?? "null"));
            Internal_Log(StringBuffer, logColor);
        }

        [Conditional("ENABLELOG")]
        public static void Log(string message = "", LogColor logColor = LogColor.White)
        {
            if(!EnableLog) return;
            StringBuffer.Length = 0;
            StringBuffer.Append(Prefix).Append(GetLogCaller()).Append(LogStringType2).Append(message);
            Internal_Log(StringBuffer, logColor);
        }

        [Conditional("ENABLELOG")]
        public static void Log(string format, LogColor logColor, params object[] args)
        {
            if(!EnableLog) return;
            StringBuffer.Length = 0;
            StringBuffer.Append(Prefix).Append(GetLogCaller()).Append(LogStringType2).AppendFormat(format, args);
            Internal_Log(StringBuffer, logColor);
        }

        [Conditional("ENABLELOG")]
        public static void Log(string format, params object[] args)
        {
            if(!EnableLog) return;
            StringBuffer.Length = 0;
            StringBuffer.Append(Prefix).Append(GetLogCaller()).Append(LogStringType2).AppendFormat(format, args);
            Internal_Log(StringBuffer);
        }

        [Conditional("ENABLELOG")]
        public static void Log(this IDebugLogTag obj, string message = "", LogColor logColor = LogColor.White)
        {
            if(!EnableLog) return;
            StringBuffer.Length = 0;
            StringBuffer.Append(Prefix).Append(GetLogTag(obj)).Append(LogStringType1).Append(GetLogCaller()).Append(LogStringType2).Append(message);
            Internal_Log(StringBuffer, logColor);
        }

        [Conditional("ENABLELOG")]
        public static void Log(this IDebugLogTag obj, string format, LogColor logColor, params object[] args)
        {
            if(!EnableLog) return;
            StringBuffer.Length = 0;
            StringBuffer.Append(Prefix).Append(GetLogTag(obj)).Append(LogStringType1).Append(GetLogCaller()).Append(LogStringType2).AppendFormat(format, args);
            Internal_Log(StringBuffer, logColor);
        }

        [Conditional("ENABLELOG")]
        public static void Log(this IDebugLogTag obj, string format, params object[] args)
        {
            if(!EnableLog) return;
            StringBuffer.Length = 0;
            StringBuffer.Append(Prefix).Append(GetLogTag(obj)).Append(LogStringType1).Append(GetLogCaller()).Append(LogStringType2).AppendFormat(format, args);
            Internal_Log(StringBuffer);
        }
        
        [Conditional("ENABLELOGWARNING")]
        public static void LogWarning(object obj, LogColor logColor = LogColor.Yellow)
        {
            if(!EnableLogWarning) return;
            StringBuffer.Length = 0;
            StringBuffer.Append(Prefix).Append(GetLogCaller()).Append(LogStringType2).Append((obj?.ToString() ?? "null"));
            Internal_LogWarning(StringBuffer, logColor);
        }

        [Conditional("ENABLELOGWARNING")]
        public static void LogWarning(string message, LogColor logColor = LogColor.Yellow)
        {
            if(!EnableLogWarning) return;
            StringBuffer.Length = 0;
            StringBuffer.Append(Prefix).Append(GetLogCaller()).Append(LogStringType2).Append(message);
            Internal_LogWarning(StringBuffer, logColor);
        }

        [Conditional("ENABLELOGWARNING")]
        public static void LogWarning(string format, LogColor logColor, params object[] args)
        {
            if(!EnableLogWarning) return;
            StringBuffer.Length = 0;
            StringBuffer.Append(Prefix).Append(GetLogCaller()).Append(LogStringType2).AppendFormat(format, args);
            Internal_LogWarning(StringBuffer, logColor);
        }

        [Conditional("ENABLELOGWARNING")]
        public static void LogWarning(string format, params object[] args)
        {
            if(!EnableLogWarning) return;
            StringBuffer.Length = 0;
            StringBuffer.Append(Prefix).Append(GetLogCaller()).Append(LogStringType2).AppendFormat(format, args);
            Internal_LogWarning(StringBuffer);
        }

        [Conditional("ENABLELOGWARNING")]
        public static void LogWarning(this IDebugLogTag obj, string message, LogColor logColor = LogColor.Yellow)
        {
            if(!EnableLogWarning) return;
            StringBuffer.Length = 0;
            StringBuffer.Append(Prefix).Append(GetLogTag(obj)).Append(LogStringType1).Append(GetLogCaller()).Append(LogStringType2).Append(message);
            Internal_LogWarning(StringBuffer, logColor);
        }

        [Conditional("ENABLELOGWARNING")]
        public static void LogWarning(this IDebugLogTag obj, string format, LogColor logColor, params object[] args)
        {
            if(!EnableLogWarning) return;
            StringBuffer.Length = 0;
            StringBuffer.Append(Prefix).Append(GetLogTag(obj)).Append(LogStringType1).Append(GetLogCaller()).Append(LogStringType2).AppendFormat(format, args);
            Internal_LogWarning(StringBuffer, logColor);
        }

        [Conditional("ENABLELOGWARNING")]
        public static void LogWarning(this IDebugLogTag obj, string format, params object[] args)
        {
            if(!EnableLogWarning) return;
            StringBuffer.Length = 0;
            StringBuffer.Append(Prefix).Append(GetLogTag(obj)).Append(LogStringType1).Append(GetLogCaller()).Append(LogStringType2).AppendFormat(format, args);
            Internal_LogWarning(StringBuffer);
        }
        
        [Conditional("ENABLELOGERROR")]
        public static void LogError(object obj, LogColor logColor = LogColor.Red)
        {
            if(!EnableLogError) return;
            StringBuffer.Length = 0;
            StringBuffer.Append(Prefix).Append(GetLogCaller()).Append(LogStringType2).Append((obj?.ToString() ?? "null"));
            Internal_LogError(StringBuffer, logColor);
        }

        [Conditional("ENABLELOGERROR")]
        public static void LogError(string message, LogColor logColor = LogColor.Red)
        {
            if(!EnableLogError) return;
            StringBuffer.Length = 0;
            StringBuffer.Append(Prefix).Append(GetLogCaller()).Append(LogStringType2).Append(message);
            Internal_LogError(StringBuffer, logColor);
        }

        [Conditional("ENABLELOGERROR")]
        public static void LogError(string format, LogColor logColor, params object[] args)
        {
            if(!EnableLogError) return;
            StringBuffer.Length = 0;
            StringBuffer.Append(Prefix).Append(GetLogCaller()).Append(LogStringType2).AppendFormat(format, args);
            Internal_LogError(StringBuffer, logColor);
        }

        [Conditional("ENABLELOGERROR")]
        public static void LogError(string format, params object[] args)
        {
            if(!EnableLogError) return;
            StringBuffer.Length = 0;
            StringBuffer.Append(Prefix).Append(GetLogCaller()).Append(LogStringType2).AppendFormat(format, args);
            Internal_LogError(StringBuffer);
        }

        [Conditional("ENABLELOGERROR")]
        public static void LogError(this IDebugLogTag obj, string message, LogColor logColor = LogColor.Red)
        {
            if(!EnableLogError) return;
            StringBuffer.Length = 0;
            StringBuffer.Append(Prefix).Append(GetLogTag(obj)).Append(LogStringType1).Append(GetLogCaller()).Append(LogStringType2).Append(message);
            Internal_LogError(StringBuffer, logColor);
        }

        [Conditional("ENABLELOGERROR")]
        public static void LogError(this IDebugLogTag obj, string format, LogColor logColor, params object[] args)
        {
            if(!EnableLogError) return;
            StringBuffer.Length = 0;
            StringBuffer.Append(Prefix).Append(GetLogTag(obj)).Append(LogStringType1).Append(GetLogCaller()).Append(LogStringType2).AppendFormat(format, args);
            Internal_LogError(StringBuffer, logColor);
        }

        [Conditional("ENABLELOGERROR")]
        public static void LogError(this IDebugLogTag obj, string format, params object[] args)
        {
            if(!EnableLogError) return;
            StringBuffer.Length = 0;
            StringBuffer.Append(Prefix).Append(GetLogTag(obj)).Append(LogStringType1).Append(GetLogCaller()).Append(LogStringType2).AppendFormat(format, args);
            Internal_LogError(StringBuffer);
        }

        private static string GetLogTag(IDebugLogTag obj)
        {
            return obj.LogTag;
        }
        
        private static string GenLogFileName()
        {
            return DateTime.Now.GetDateTimeFormats('s')[0].Replace("-", "_").Replace(":", "_").Replace(" ", "") + ".log";
        }

        private static string CheckLogFileDir()
        {
            if (string.IsNullOrEmpty(LogFileDir))
            {
                return "";
            }
            try
            {
                if (!Directory.Exists(LogFileDir))
                    Directory.CreateDirectory(LogFileDir);
            }
            catch (Exception ex)
            {
                throw new Exception(" Create LogFileDir Error..... "+LogFileDir);
            }
            return LogFileDir;
        }
        
        private static string GetLogCaller(bool bIncludeClassName = true)
        {
            StackTrace stackTrace = new StackTrace(2, false);
            if ((object) DebugAssembly == null)
                DebugAssembly = typeof (GameDebug).Assembly;
            for (int index = 0; index < stackTrace.FrameCount; ++index)
            {
                MethodBase method = stackTrace.GetFrame(index).GetMethod();
                if ((object) method.Module.Assembly != (object) DebugAssembly)
                {
                    if (bIncludeClassName)
                        if (method.DeclaringType != null) return method.DeclaringType.Name + "::" + method.Name;
                    return method.Name;
                }
            }
            return "";
        }

        private static void LogToFile(string message, bool enableStack = false)
        {
            if(!EnableSaveFile) return;
            if (LogFileWriter == null)
            {
                LogFileName = GenLogFileName();
                LogFileDir = CheckLogFileDir();
                if(string.IsNullOrEmpty(LogFileDir)) return;
                string path = LogFileDir + LogFileName;
                try
                {
                    LogFileWriter = File.AppendText(path);
                    LogFileWriter.AutoFlush = true;
                }
                catch (Exception e)
                {
                    LogFileWriter = (StreamWriter) null;
                    throw new Exception($" WriteLogFile {LogFileDir} : {LogFileName} ");
                }
            }
            if (LogFileWriter == null) return;
            try
            {
                LogFileWriter.WriteLine(message);
                if (!enableStack && !EnableFileStackTrace)
                    return;
                StackTrace stackTrace = new StackTrace(2, true);
                for (int index = 0; index < stackTrace.FrameCount; ++index)
                {
                    StackFrame frame = stackTrace.GetFrame(index);
                    MethodBase method = frame.GetMethod();
                    LogFileWriter.WriteLine($"     文件名:   {frame.GetFileName() }  行号: { (object) frame.GetFileLineNumber() }  函数名: { method.Name}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($" Write Log Error  {ex.Message}");
            }
        }
    }
}