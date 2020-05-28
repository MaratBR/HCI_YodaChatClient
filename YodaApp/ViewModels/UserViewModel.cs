using YodaApiClient.DataTypes.DTO;

namespace YodaApp.ViewModels
{
    internal class UserViewModel : ViewModelBase
    {
        private string name;

        public string Name
        {
            get { return name; }
            set => Set(ref name, nameof(Name), value);
        }

        private int id;

        public int Id
        {
            get { return id; }
            set => Set(ref id, nameof(Id), value);
        }

        public UserViewModel(ChatUserDto dto)
        {
            Id = dto.Id;
            Name = dto.Name;
        }

        private bool isOnline;

        public bool IsOnline
        {
            get { return isOnline; }
            set => Set(ref isOnline, nameof(IsOnline), value);
        }

        public UserViewModel(ChatMembershipDto user)
        {
            Id = user.User.Id;
            Name = user.User.UserName;
            IsOnline = user.IsOnline;
        }
    }
}