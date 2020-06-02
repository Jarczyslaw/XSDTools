using JToolbox.Desktop.Dialogs;
using Prism.Commands;
using Prism.Mvvm;

namespace XSDTools.DesktopApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string logs = string.Empty;
        private readonly IDialogsService dialogsService;

        public MainWindowViewModel(IDialogsService dialogsService)
        {
            this.dialogsService = dialogsService;
        }

        public DelegateCommand RemoveExternalDependenciesCommand => new DelegateCommand(() =>
        {
        });

        public DelegateCommand GenerateModelsCommand => new DelegateCommand(() =>
        {
        });

        public DelegateCommand GenerateSampleXmlCommand => new DelegateCommand(() =>
        {
        });

        public DelegateCommand ValidateXmlCommand => new DelegateCommand(() =>
        {
        });

        public string Logs
        {
            get => logs;
            set => SetProperty(ref logs, value);
        }
    }
}