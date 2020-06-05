using JToolbox.Desktop.Dialogs.Builders;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;

namespace JToolbox.Desktop.Dialogs
{
    public interface IDialogsService
    {
        TaskDialogResult Show(TaskDialogBuilder builder);

        void ShowInfo(string message, string details = null, IntPtr? owner = null);

        void ShowWarning(string message, string details = null, IntPtr? owner = null);

        void ShowError(string error, string details = null, IntPtr? owner = null);

        void ShowException(Exception exception, string message = null, IntPtr? owner = null);

        void ShowCriticalException(Exception exception, string message = null, IntPtr? owner = null);

        bool ShowYesNoQuestion(string question, IntPtr? owner = null);

        T ShowCustomButtonsQuestion<T>(string question, IEnumerable<CustomButtonData<T>> customButtons, IntPtr? owner = null);

        void ShowProgressDialog(string caption, string text, string instruction, IntPtr? owner = null);

        List<string> Open(CommonOpenDialogBuilder builder);

        string OpenFile(string title, string initialDirectory = null, List<DialogFilterPair> filters = null);

        List<string> OpenFiles(string title, string initialDirectory = null, List<DialogFilterPair> filters = null);

        string OpenFolder(string title, string initialDirectory = null);

        string Save(CommonSaveDialogBuilder builder);

        string SaveFile(string title, string initialDirectory = null, string defaultFileName = null, DialogFilterPair filter = null);
    }
}