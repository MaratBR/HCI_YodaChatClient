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
			await handler.JoinRoomAsync(room.Id);
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
			await handler.LeaveRoomAsync(room.Id);
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
			handler = await Api.ConnectAsync();
			handler.MessageReceived += Handler_MessageReceived;
			handler.UserActionPerformed += Handler_UserActionPerformed;
		}

		private void Handler_UserActionPerformed(object sender, ChatUserActionEventArgs e)
		{
			var dto = e.ActionDto;
			if (dto.ActionType == UserActionType.Joined)
			{
				if (dto.UserId == User.Id)
				{
					roomVMs[dto.RoomId].Status = RoomStatus.Joined;
					LeaveRoomCommand.RaiseCanExecuteChanged();
				}
			}
			else if (dto.ActionType == UserActionType.Left)
			{
				if (dto.UserId == User.Id)
				{
					roomVMs[dto.RoomId].Status = RoomStatus.Left;
					JoinRoomCommand.RaiseCanExecuteChanged();
				}
			}
		}

		public void Disconnect()
		{
			// TODO add actual method for disconnection
			handler = null;
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
			foreach (var room in await Api.GetRoomsAsync())
			{
				await AddRoom(room);
			}
		}

		public async Task UpdateUser() => User = await Api.GetUserAsync();

		public async Task AddRoom(CreateRoomRequest request)
		{
			var room = await Api.CreateRoomAsync(request);

			await AddRoom(room);
		}

		public async Task AddRoom(Room room)
		{
			var roomHandler = await handler.GetRoomHandlerAsync(room.Id);
			var vm = new RoomViewModel(roomHandler);
			roomVMs[room.Id] = vm;
			Rooms.Add(vm);
		}
	}
}
