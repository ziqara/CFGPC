using System;
using System.IO;

namespace DDMLib
{
    public static class ErrorLogger
    {
        private static readonly string LogFilePath = "errors.log";
        public static void LogError(string methodName, string message)
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {methodName} | {message}{Environment.NewLine}";

            File.AppendAllText(LogFilePath, logEntry);

        }
    }
}
