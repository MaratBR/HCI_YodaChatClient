using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YodaApiClient;
using YodaApiClient.DataTypes;

namespace YodaApp.ViewModels
{
    class UserSessionViewModel : ViewModelBase
	{
		private readonly IApi _api;

		#region Properties

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

		#endregion

		public UserSessionViewModel(IApi api)
		{
			_api = api;
		}

		public async Task Init()
		{
			await UpdateUser();
			await UpdateRooms();
		}

		public async Task UpdateRooms() => Rooms = new ObservableCollection<Room>(await _api.GetRooms());

		public async Task UpdateUser() => User = await _api.GetUserAsync();

		public async Task AddRoom(CreateRoomRequest request)
		{
			var room = await _api.CreateRoom(request);

			Rooms.Add(room);
		}
	}
}
