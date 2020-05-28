using Autofac;
using System;
using YodaApp.Services;

namespace YodaApp.ViewModels.Controls
{
    internal class CreateNewRoomOptionsViewModel
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