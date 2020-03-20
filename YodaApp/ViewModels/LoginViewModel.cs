using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using YodaApp.Utils;

namespace YodaApp.ViewModels
{
    class LoginViewModel : ViewModelBase
    {
		private string login;

		public string Login
		{
			get { return login; }
			set { Set(ref login, nameof(Login), value); }
		}

		public async Task LoginCommandHandler(PasswordBox passwordBox)
		{
			var pwd = passwordBox.Password;

		}

		private ICommand loginCommand;

		public ICommand LoginCommand => loginCommand ?? (loginCommand = new AsyncRelayCommand<PasswordBox>(LoginCommandHandler));

	}
}
