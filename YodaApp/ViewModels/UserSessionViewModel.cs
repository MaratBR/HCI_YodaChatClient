using Autofac;
using MaterialDesignThemes.Wpf;
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
using YodaApiClient.DataTypes.DTO;
using YodaApiClient.Events;
using YodaApp.Controls;
using YodaApp.Services;
using YodaApp.Utils;

namespace YodaApp.ViewModels
{
    class UserSessionViewModel : ViewModelBase
	{
		private IChatClient handler;
		private readonly IComponentContext componentContext;
		private readonly IApiProvider provider;
		private readonly IAppUIService _windows;
		private readonly Dictionary<Guid, RoomViewModel> roomVMs = new Dictionary<Guid, RoomViewModel>();

		#region Properties

		private string quote;

		public string Quote
		{
			get { return quote; }
			set => Set(ref quote, nameof(Quote), value);
		}


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
			await handler.JoinRoomAsync(room.Id);
		}


		private AbstractCommand _leaveRoomCommand;

		public AbstractCommand LeaveRoomCommand => _leaveRoomCommand ?? (_leaveRoomCommand = new AsyncRelayCommand<RoomViewModel>(LeaveRoom, room => !room.IsLeft));

		private async Task LeaveRoom(RoomViewModel room)
		{
			if (room.Status == RoomStatus.Left)
				return;
			room.Status = RoomStatus.AwaitingServerReponse;
			await handler.LeaveRoomAsync(room.Id);
		}

		private ICommand _updateRoomsCommand;

		public ICommand UpdateRoomsCommand => _updateRoomsCommand ?? (_updateRoomsCommand = new AsyncRelayCommand(UpdateRoomsCommandHandler));

		private Task UpdateRoomsCommandHandler()
		{
			return UpdateRooms();
		}

		private ICommand _createRoomCommand;

		public ICommand CreateRoomCommand => _createRoomCommand ?? (_createRoomCommand = new AsyncRelayCommand(CreateRoomCommandHandler));

		private async Task CreateRoomCommandHandler()
		{
			var vm = componentContext.Resolve<NewRoomViewModel>();
			await DialogHost.Show(
				new CreateNewRoomForm
				{
					DataContext = vm
				},
				(object sender, DialogOpenedEventArgs e) =>
				{
					vm.CloseForm += (object _sender, EventArgs _e) =>
					{
						e.Session.Close();
					};
				}
				);
			await UpdateRooms();
		}

		private ICommand _updateQuoteCommand;

		public ICommand UpdateQuoteCommand => _updateQuoteCommand ?? (_updateQuoteCommand = new AsyncRelayCommand(UpdateQuote));

		private ICommand _closeSelectedRoomCommand;

		public ICommand CloseSelectedRoomCommand => _closeSelectedRoomCommand ?? (_closeSelectedRoomCommand = new RelayCommand(CloseSelectedRoom));

		private void CloseSelectedRoom()
		{
			SelectedRoom = null;
		}

		#endregion

		public UserSessionViewModel(IApi api, IAppUIService windows, IApiProvider provider, IComponentContext componentContext)
		{
			_windows = windows;
			Api = api;
			this.provider = provider;
			this.componentContext = componentContext;
		}

		public IApi Api { get; }

		public async void Update()
		{
			await Connect();
			await UpdateUser();
			await UpdateRooms();
			await UpdateQuote();
		}

		private async Task UpdateQuote()
		{
			Quote = null;
			Quote = await provider.PingAsync();
		}

		private async Task Connect()
		{
			handler = await Api.ConnectAsync();
		}

		public void Disconnect()
		{
			handler.Disconnect();
			handler = null;
		}

		public async Task UpdateRooms()
		{
			Guid? roomGuid = SelectedRoom?.Id;
			SelectedRoom = null;

			foreach (var room in Rooms)
			{
				await LeaveRoom(room);
			}

			Rooms.Clear();

			foreach (var room in await Api.GetRoomsAsync())
			{
				await AddRoom(room);
			}

			foreach (var room in Rooms)
			{
				await JoinRoom(room);
				await Task.Delay(100);
			}

			if (roomGuid != null)
			{
				SelectedRoom = Rooms.Where(r => r.Id == roomGuid).SingleOrDefault();
			}

			JoinRoomCommand.RaiseCanExecuteChanged();
			LeaveRoomCommand.RaiseCanExecuteChanged();
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
