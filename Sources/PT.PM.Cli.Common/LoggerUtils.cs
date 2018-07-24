﻿using PT.PM.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PT.PM.Cli.Common
{
    public static class LoggerUtils
    {
        public const int Align = -25;

        private const int TwoInPower20 = 1 << 20;

        public static void LogSystemInfo(ILogger logger, string coreName)
        {
            Process currentProcess = Process.GetCurrentProcess();

            logger.LogInfo($"{coreName + " version:",Align} {GetVersionString()}");
            logger.LogInfo($"{"Finish date:",Align} {DateTime.Now}");
            logger.LogInfo($"{"OS:",Align} {Environment.OSVersion}");
            logger.LogInfo($"{"Config:",Align} {(CommonUtils.IsDebug ? "DEBUG" : "RELEASE")} ({(Debugger.IsAttached ? "+ debugger" : "no debugger")})");

            string processBitsString = (Environment.Is64BitProcess ? "64" : "32") + "-bit";
            double peakVirtualSet = currentProcess.PeakVirtualMemorySize64 / TwoInPower20;
            double peakWorkingSet = currentProcess.PeakWorkingSet64 / TwoInPower20;
            logger.LogInfo($"{"Peak virtual/working set:",Align} {peakVirtualSet} / {peakWorkingSet} MB, {processBitsString}");
        }

        public static string GetVersionString()
        {
            Assembly assembly = Assembly.GetEntryAssembly();

            AssemblyName assemblyName = assembly.GetName();
            string buildTime = "";

            string streamName = assembly.GetManifestResourceNames().FirstOrDefault() ?? "";
            Stream stream;
            if (!string.IsNullOrEmpty(streamName) &&
                (stream = assembly.GetManifestResourceStream(streamName)) != null)
            {
                using (var reader = new StreamReader(stream))
                {
                    buildTime = reader.ReadToEnd().Trim();
                }
                buildTime = $" (build: {buildTime})";
            }

            return $"{assemblyName.Version}{buildTime}";
        }

        public static string LogMatch(string matchedCode, LineColumnTextSpan textSpan, CodeFile codeFile, string patternKey,
            bool dataFlowMatch, bool isSuppressed)
        {
            var message = new StringBuilder();
            message.AppendLine($"-- Match ----------------");
            message.AppendLine($"Type          : {(dataFlowMatch ? "Taint" : "PM")}");
            message.AppendLine($"Code          : {matchedCode}");
            message.AppendLine($"Location      : {textSpan.ToString(false)}");
            message.AppendLine($"File          : {codeFile}");
            message.AppendLine($"PatternKey    : {patternKey}");
            if (isSuppressed)
            {
                message.AppendLine($"IsSuppressed  : {isSuppressed}");
            }
            string text = message.ToString();

            return text;
        }
    }
}
