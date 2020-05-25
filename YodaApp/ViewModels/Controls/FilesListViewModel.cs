using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YodaApiClient;
using YodaApiClient.DataTypes;
using YodaApp.Utils;

namespace YodaApp.ViewModels.Controls
{
    class FilesListViewModel : ViewModelBase
    {
        private readonly IApi api;

        public FilesListViewModel(IApi api)
        {
            this.api = api;
        }

        public ObservableCollection<AttachmentViewModel> Files { get; } = new ObservableCollection<AttachmentViewModel>();

        private async Task Refresh()
        {
            Refreshing = true;
            var files = await api.GetUserFiles();
            Files.Clear();
            foreach (var vm in files.Select(f => new AttachmentViewModel(api, f)))
                Files.Add(vm);
            Refreshing = false;
        }

        private ICommand refreshCommand;

        public ICommand RefreshCommand => refreshCommand ?? (refreshCommand = new AsyncRelayCommand(Refresh));


        private bool refreshing;

        public bool Refreshing
        {
            get { return refreshing; }
            set => Set(ref refreshing, nameof(Refreshing), value);
        }

    }
}
