using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YodaApiClient;
using YodaApiClient.DataTypes;
using YodaApp.Services;
using YodaApp.Utils;

namespace YodaApp.ViewModels
{
    class UserSessionViewModel : ViewModelBase
	{
		private IChatApiHandler handler;
		private IWindowService _windows;
		private readonly Dictionary<Guid, RoomViewModel> roomVMs = new Dictionary<Guid, RoomViewModel>();

		#region Properties

		private ObservableCollection<RoomViewModel> rooms = new ObservableCollection<RoomViewModel>();

		public ObservableCollection<RoomViewModel> Rooms
		{
			get => rooms;
			set => Set(ref rooms, nameof(Rooms), value);
		}

		private RoomViewModel selectedRoom;

		public RoomViewModel SelectedRoom
		{
			get => selectedRoom;
			set
			{
				Set(ref selectedRoom, nameof(SelectedRoom), value);
				JoinRoomCommand.RaiseCanExecuteChanged();
				LeaveRoomCommand.RaiseCanExecuteChanged();
			}
		}

		private IUser user;

		public IUser User
		{
			get => user;
			set => Set(ref user, nameof(User), value);
		}

		#endregion

		#region Commands

		private AbstractCommand _joinRoomCommand;

		public AbstractCommand JoinRoomCommand => _joinRoomCommand ?? (_joinRoomCommand = new AsyncRelayCommand<RoomViewModel>(JoinRoom, room => !room.IsJoined));

		private async Task JoinRoom(RoomViewModel room)
		{
			if (room.Status == RoomStatus.Joined)
				return;
			room.Status = RoomStatus.AwaitingServerReponse;
#if DEBUG
			// TODO Remove
			await Task.Delay(500);
#endif
			await handler.JoinRoom(room.Id);
		}


		private AbstractCommand _leaveRoomCommand;

		public AbstractCommand LeaveRoomCommand => _leaveRoomCommand ?? (_leaveRoomCommand = new AsyncRelayCommand<RoomViewModel>(LeaveRoom, room => !room.IsLeft));

		private async Task LeaveRoom(RoomViewModel room)
		{
			if (room.Status == RoomStatus.Left)
				return;
			room.Status = RoomStatus.AwaitingServerReponse;
#if DEBUG
			// TODO Remove
			await Task.Delay(500);
#endif
			await handler.LeaveRoom(room.Id);
		}

		private ICommand _updateRoomsCommand;

		public ICommand UpdateRoomsCommand => _updateRoomsCommand ?? (_updateRoomsCommand = new AsyncRelayCommand(UpdateRoomsCommandHandler));

		private Task UpdateRoomsCommandHandler()
		{
			return UpdateRooms();
		}

		private ICommand _createRoomCommand;

		public ICommand CreateRoomCommand => _createRoomCommand ?? (_createRoomCommand = new RelayCommand(CreateRoomCommandHandler));

		private void CreateRoomCommandHandler()
		{
			_windows.ShowNewRoomWindow();
		}

		#endregion

		public UserSessionViewModel(IApi api, IWindowService windows)
		{
			_windows = windows;
			Api = api;
		}

		public IApi Api { get; }

		public async void Update()
		{
			await Connect();
			await UpdateUser();
			await UpdateRooms();
		}

		private async Task Connect()
		{
			handler = await Api.Connect();
			handler.MessageReceived += Handler_MessageReceived;
			handler.UserJoined += Handler_UserJoined;
			handler.UserLeft += Handler_UserLeft;
		}

		public void Disconnect()
		{
			// TODO add actual method for disconnection
			handler = null;
		}

		private void Handler_UserLeft(object sender, ChatUserLeftEventArgs e)
		{
			if (e.UserId == User.Id)
			{
				roomVMs[e.RoomId].Status = RoomStatus.Left;
				JoinRoomCommand.RaiseCanExecuteChanged();
			}

			Console.WriteLine($"User {e.UserId} left {e.RoomId}");
		}

		private void Handler_UserJoined(object sender, ChatUserJoinedEventArgs e)
		{
			if (e.UserId == User.Id)
			{
				roomVMs[e.RoomId].Status = RoomStatus.Joined;
				LeaveRoomCommand.RaiseCanExecuteChanged();
			}

			Console.WriteLine($"User {e.UserId} joined {e.RoomId}");
		}

		private void Handler_MessageReceived(object sender, ChatMessageEventArgs e)
		{
			if (roomVMs.ContainsKey(e.Message.Room.Id))
			{
				roomVMs[e.Message.Room.Id].Messages.Add(new MessageViewModel(e.Message));
			}
		}

		public async Task UpdateRooms()
		{
			Rooms.Clear();
			foreach (var room in await Api.GetRooms())
			{
				AddRoom(room);
			}
		}

		public async Task UpdateUser() => User = await Api.GetUserAsync();

		public async Task AddRoom(CreateRoomRequest request)
		{
			var room = await Api.CreateRoom(request);

			AddRoom(room);
		}

		public void AddRoom(Room room)
		{
			var vm = new RoomViewModel(handler.GetRoomHandler(room.Id));
			roomVMs[room.Id] = vm;
			Rooms.Add(vm);
		}
	}
}
