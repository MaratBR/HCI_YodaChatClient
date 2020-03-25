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

namespace YodaApp.ViewModels
{
    class UserSessionViewModel : ViewModelBase
	{
		private readonly IApi _api;
		private IChatApiHandler handler;
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
				if (SelectedRoom != null)
				{
					SelectedRoom.HasMessageChanged -= SelectedRoom_HasMessageChanged;
				}
				Set(ref selectedRoom, nameof(SelectedRoom), value);
				if (SelectedRoom != null)
				{
					SelectedRoom.HasMessageChanged += SelectedRoom_HasMessageChanged;
				}
				JoinRoomCommand.RaiseCanExecuteChanged();
				LeaveRoomCommand.RaiseCanExecuteChanged();
				SendToRoomCommand.RaiseCanExecuteChanged();
			}
		}

		private void SelectedRoom_HasMessageChanged(object sender, EventArgs e)
		{
			SendToRoomCommand.RaiseCanExecuteChanged();
		}

		private User user;

		public User User
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
			await handler.JoinRoom(room.Id);
		}


		private AbstractCommand _leaveRoomCommand;

		public AbstractCommand LeaveRoomCommand => _leaveRoomCommand ?? (_leaveRoomCommand = new AsyncRelayCommand<RoomViewModel>(LeaveRoom, room => !room.IsLeft));

		private async Task LeaveRoom(RoomViewModel room)
		{
			room.Status = RoomStatus.AwaitingServerReponse;

			await handler.LeaveRoom(room.Id);
		}


		private AbstractCommand _sendToRoomCommand;

		public AbstractCommand SendToRoomCommand => _sendToRoomCommand ?? (_sendToRoomCommand = new AsyncRelayCommand<RoomViewModel>(SendToRoom, room => room.HasMessage));

		private Task SendToRoom(RoomViewModel room)
		{
			return handler.SendToRoom(room.Draft, room.Id);
		}

		#endregion

		public UserSessionViewModel(IApi api)
		{
			_api = api;
		}

		public async Task Update()
		{
			await UpdateUser();
			await UpdateRooms();
			await Connect();
		}

		private async Task Connect()
		{
			handler = await _api.Connect();
			handler.MessageReceived += Handler_MessageReceived;
			handler.UserJoined += Handler_UserJoined;
			handler.UserLeft += Handler_UserLeft;
		}

		public void Disconnect()
		{
			handler = null;
		}

		private void Handler_UserLeft(object sender, ChatUserLeftEventArgs e)
		{
			if (e.UserId == User.Id)
			{
				roomVMs[e.RoomId].Status = RoomStatus.Left;
				JoinRoomCommand.RaiseCanExecuteChanged();
			}

			Console.WriteLine($"User {e.UserId} joined {e.RoomId}");
			//throw new NotImplementedException();
		}

		private void Handler_UserJoined(object sender, ChatUserJoinedEventArgs e)
		{
			if (e.UserId == User.Id)
			{
				roomVMs[e.RoomId].Status = RoomStatus.Joined;
				LeaveRoomCommand.RaiseCanExecuteChanged();
			}

			Console.WriteLine($"User {e.UserId} left {e.RoomId}");
			//throw new NotImplementedException();
		}

		private void Handler_MessageReceived(object sender, ChatMessageEventArgs e)
		{
			if (roomVMs.ContainsKey(e.Message.RoomId))
			{
				roomVMs[e.Message.RoomId].Messages.Add(e.Message);
			}
		}

		public async Task UpdateRooms()
		{
			foreach (var room in await _api.GetRooms())
			{
				AddRoom(room);
			}
		}

		public async Task UpdateUser() => User = await _api.GetUserAsync();

		public async Task AddRoom(CreateRoomRequest request)
		{
			var room = await _api.CreateRoom(request);

			AddRoom(room);
		}

		public void AddRoom(Room room)
		{
			var vm = new RoomViewModel(room);
			roomVMs[room.Id] = vm;
			Rooms.Add(vm);
		}
	}
}
