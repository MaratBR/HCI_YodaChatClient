using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YodaApp.YODApi.DataTypes;

namespace YodaApp.ViewModels
{
    class UserSessionViewModel : ViewModelBase
	{
		private ObservableCollection<Room> rooms;

		public ObservableCollection<Room> Rooms
		{
			get => rooms;
			set => Set(ref rooms, nameof(Rooms), value);
		}

		private User user;

		public User User
		{
			get => user;
			set => Set(ref user, nameof(User), value);
		}
	}
}
