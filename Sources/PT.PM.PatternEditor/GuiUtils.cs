﻿using Avalonia.Controls;
using PT.PM.Common.Exceptions;
using PT.PM.Common.Utils;
using PT.PM.PatternEditor.ViewModels;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace PT.PM.PatternEditor
{
    internal class GuiUtils
    {
        internal static void ProcessErrorOnDoubleClick(ListBox errorsListBox, TextBox inputTextBox)
        {
            errorsListBox.Focus();

            if (errorsListBox.SelectedItem is ErrorViewModel errorViewModel &&
                errorViewModel.Exception is PMException pmException &&
                !pmException.TextSpan.IsZero)
            {
                inputTextBox.SelectionStart = pmException.TextSpan.Start;
                inputTextBox.SelectionEnd = pmException.TextSpan.End;
                inputTextBox.CaretIndex = inputTextBox.SelectionEnd + 1;
            }
        }

        public static void OpenDirectory(string directoryName)
        {
            string normDirPath = directoryName.NormalizeDirPath();
            if (Directory.Exists(normDirPath))
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo { FileName = "explorer.exe", Arguments = $"\"{normDirPath}\"" });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start(new ProcessStartInfo { FileName = "xdg-open", Arguments = directoryName, CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start(new ProcessStartInfo { FileName = "open", Arguments = directoryName, CreateNoWindow = true });
                }
            }
        }
    }
}
