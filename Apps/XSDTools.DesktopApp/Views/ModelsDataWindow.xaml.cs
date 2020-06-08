using JToolbox.Desktop.Dialogs;
using System.Windows;
using XSDTools.DesktopApp.Models;

namespace XSDTools.DesktopApp.Views
{
    public partial class ModelsDataWindow : Window
    {
        private readonly IDialogsService dialogsService;
        private ModelsData modelsData;

        public ModelsDataWindow(IDialogsService dialogsService)
        {
            this.dialogsService = dialogsService;

            InitializeComponent();
        }

        public ModelsData ShowAsDialog()
        {
            ShowDialog();
            return modelsData;
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbFileName.Text) || string.IsNullOrEmpty(tbNamespace.Text))
            {
                dialogsService.ShowError("File name and namespace can not be empty");
                return;
            }

            modelsData = new ModelsData
            {
                FileName = tbFileName.Text,
                Namespace = tbNamespace.Text
            };
            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}