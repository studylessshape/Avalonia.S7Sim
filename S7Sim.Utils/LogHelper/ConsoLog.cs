﻿using System;

namespace S7Sim.Utils.LogHelper
{
    public static class ConsoLog
    {
        private static ConsoLogInstance? _instance;

        public static ConsoLogInstance Instance
        {
            get
            {
                _instance ??= new ConsoLogInstance();
                return _instance;
            }
        }

        public class ConsoLogInstance
        {
            internal ConsoLogInstance() { }
            public void Log(string message, LogLevel level = LogLevel.Info, DateTime? now = null)
            {
                ConsoLog.Log(message, level, now);
            }

            public void LogInfo(string message, DateTime? now = null)
            {
                ConsoLog.LogInfo(message, now);
            }

            public void LogSuccess(string message, DateTime? now = null)
            {
                ConsoLog.LogSuccess(message, now);
            }

            public void LogWarn(string message, DateTime? now = null)
            {
                ConsoLog.LogWarn(message, now);
            }

            public void LogError(string message, DateTime? now = null)
            {
                ConsoLog.LogError(message, now);
            }
        }

        public static void Log(string message, LogLevel level = LogLevel.Info, DateTime? now = null)
        {
            var normalForegroundColor = Console.ForegroundColor;
            var normalBackgroundColor = Console.BackgroundColor;

            Console.Write($"[{now ?? DateTime.Now:yyyy-MM-dd HH:mm:ss} ");

            switch (level)
            {
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Green;
                    goto default;
                case LogLevel.Warn:
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    goto default;
                case LogLevel.Success:
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Green;
                    goto default;
                //case LogLevel.Debug:
                //    Console.ForegroundColor = ConsoleColor.Yellow;
                //    goto default;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Red;
                    goto default;
                default:
                    Console.Write($"{level}");
                    Console.ForegroundColor = normalForegroundColor;
                    Console.BackgroundColor = normalBackgroundColor;
                    break;
            }

            Console.Write($"] {message}{Environment.NewLine}");
        }

        public static void LogInfo(string message, DateTime? now = null)
        {
            Log(message, LogLevel.Info, now);
        }

        public static void LogSuccess(string message, DateTime? now = null)
        {
            Log(message, LogLevel.Success, now);
        }

        public static void LogWarn(string message, DateTime? now = null)
        {
            Log(message, LogLevel.Warn, now);
        }

        public static void LogError(string message, DateTime? now = null)
        {
            Log(message, LogLevel.Error, now);
        }
    }
}
