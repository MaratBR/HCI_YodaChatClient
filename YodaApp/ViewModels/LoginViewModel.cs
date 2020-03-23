using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YodaApp.Persistence;
using YodaApp.Utils;
using YodaApp.YODApi;
using YodaApp.YODApi.DataTypes;

namespace YodaApp.ViewModels
{
	class UserAuthenticatedEventArgs : EventArgs
	{
		public User User { get; set; }
	}

    class LoginViewModel : ViewModelBase
    {
		private string login;

		public string Login
		{
			get { return login; }
			set { Set(ref login, nameof(Login), value); }
		}

		private bool loading;

		public bool Loading
		{
			get { return loading; }
			set { Set(ref loading, nameof(Loading), value); }
		}

		private string error;

		public string Error
		{
			get { return error; }
			set { Set(ref error, nameof(Error), value); }
		}

		private bool hasError;

		public bool HasError
		{
			get { return hasError; }
			set { Set(ref hasError, nameof(HasError), value); }
		}


		public event EventHandler<UserAuthenticatedEventArgs> UserAuthenticated;

		#region Log in

		private ICommand loginCommand;

		public ICommand LoginCommand => loginCommand ?? (loginCommand = new AsyncRelayCommand<PasswordBox>(LoginCommandHandler));

		public async Task LoginCommandHandler(PasswordBox passwordBox)
		{
			Loading = true;
			var pwd = passwordBox.Password;

			var result = await Api.Instance.Authenticate(Login, pwd);

			HasError = !result.IsSuccess;


			if (result.IsSuccess)
			{

				Api.Instance.SetToken(result.Value.AccessToken);
				var user = await Api.instance.GetCurrentUser();
				if (user != null)
				{
					OpenMainView(user);
				}
				else
				{
					MessageBox.Show("Failed to load user from the server");
				}
			}
			else
			{
				Error = result.Errors.First().Message;
			}

			Loading = false;
		}

		#endregion



		private ICommand loadConfigCommand;

		public ICommand LoadConfigCommand => loadConfigCommand ?? (loadConfigCommand = new AsyncRelayCommand(LoadConfigCommandHandler));

		private async Task LoadConfigCommandHandler()
		{
			string accessToken = Store.Instance.GetAccessToken();

			if (accessToken != null)
			{
				Api.Instance.SetToken(accessToken);

				var user = await Api.Instance.GetCurrentUser();

				if (user != null)
				{
					OpenMainView(user);
				}
			}
			Loading = false;
		}

		private void OpenMainView(User user)
		{
			UserAuthenticated?.Invoke(this, new UserAuthenticatedEventArgs { User = user });
		}
	}
}
