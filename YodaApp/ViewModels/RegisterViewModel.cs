using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using YodaApiClient;
using YodaApiClient.DataTypes;
using YodaApiClient.DataTypes.Requests;
using YodaApp.Services;
using YodaApp.Utils;

namespace YodaApp.ViewModels
{
    class RegisterViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authentication;
        private readonly IAppUIService _windows;

        public RegisterViewModel(IAuthenticationService authentication, IAppUIService windows)
        {
            _authentication = authentication;
            _windows = windows;
            SelectedGender = alien;
            GenderDeclarations.Add(alien);
        }

        #region Properties

        private bool terms;

        public bool Terms
        {
            get { return terms; }
            set
            {
                Set(ref terms, nameof(Terms), value);
                SubmitCommand.RaiseCanExecuteChanged();
            }
        }


        private string userName;

        public string UserName
        {
            get { return userName; }
            set
            {
                Set(ref userName, nameof(UserName), value);
                SubmitCommand.RaiseCanExecuteChanged();
            }
        }

        private string email;

        public string EMail
        {
            get { return email; }
            set
            {
                Set(ref email, nameof(EMail), value);
                SubmitCommand.RaiseCanExecuteChanged();
            }
        }


        private string phone;

        public string Phone
        {
            get { return phone; }
            set
            {
                Set(ref phone, nameof(Phone), value);
                SubmitCommand.RaiseCanExecuteChanged();
            }
        }

        private GenderDeclaration gender;

        public GenderDeclaration SelectedGender
        {
            get { return gender; }
            set => Set(ref gender, nameof(SelectedGender), value);
        }

        internal class GenderDeclaration
        {
            public BitmapImage Image { get; set; }
            public byte
                Gender { get; set; }
            public string Caption { get; set; }
        }


        private readonly GenderDeclaration alien = new GenderDeclaration
        {
            Caption = "Alien\naka ???",
            Gender = Gender.UNKNOWN,
            Image = (BitmapImage)App.Current.Resources["AlienIcon"]
        };

        public List<GenderDeclaration> GenderDeclarations { get; } = new List<GenderDeclaration>
        {
            new GenderDeclaration
            {
                Caption = "Respect\naka male",
                Gender = Gender.MALE,
                Image = (BitmapImage)App.Current.Resources["RespectIcon"]
            },
            new GenderDeclaration
            {
                Caption = "Smooch\naka Female",
                Gender = Gender.FEMALE,
                Image = (BitmapImage)App.Current.Resources["SmoochIcon"]
            }
        };

        public bool IsValid => !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(EMail) && !string.IsNullOrWhiteSpace(Phone) && Terms;

        #endregion

        #region Commands

        private AbstractCommand _submitCommand;

        public AbstractCommand SubmitCommand => _submitCommand ?? (_submitCommand = new AsyncRelayCommand<PasswordBox>(Submit, _pwd => IsValid && _pwd.Password.Length >= 8));

        private async Task Submit(PasswordBox passwordBox)
        {
            var pwd = passwordBox.Password;

            var request = new RegistrationRequest
            {
                Password = pwd,
                Email = email,
                Gender = SelectedGender.Gender,
                UserName = UserName,
                PhoneNumber = Phone
            };
            try
            {
                await _authentication.GetApiProvider().RegisterUserAndCreateApi(request);
            }
            catch(ApiException exception)
            {
                MessageBox.Show(exception.Message);
                return;
            }

            _windows.ShowLogInWindow();
            _windows.CloseSignUpWindow();
        }

        #endregion
    }
}
