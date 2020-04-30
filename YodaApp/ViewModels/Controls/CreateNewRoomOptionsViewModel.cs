using Autofac;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YodaApp.Controls;
using YodaApp.Services;
using YodaApp.Utils;

namespace YodaApp.ViewModels.Controls
{
    class CreateNewRoomOptionsViewModel
    {
        private IComponentContext context;

        public event EventHandler RequestClose;

        public CreateNewRoomOptionsViewModel(IComponentContext context)
        {
            this.context = context;
            joinRoomFormViewModel = new JoinRoomFormViewModel(context.Resolve<IAuthenticationService>().GetCurrentSession());
            newRoomViewModel = context.Resolve<NewRoomViewModel>();
            newRoomViewModel.CloseForm += CloseForm;
            joinRoomFormViewModel.Joined += CloseForm;
        }

        private void CloseForm(object sender, EventArgs e)
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        private JoinRoomFormViewModel joinRoomFormViewModel;
        private NewRoomViewModel newRoomViewModel;

        public JoinRoomFormViewModel JoinRoomFormVM => joinRoomFormViewModel;

        public NewRoomViewModel CreateNewFormVM => newRoomViewModel;
    }
}
