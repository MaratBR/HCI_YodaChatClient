using Autofac;
using FluentResults;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using YodaApiClient;
using YodaApiClient.Abstract;
using YodaApiClient.DataTypes;
using YodaApiClient.DataTypes.Requests;
using YodaApp.Controls;
using YodaApp.Services;
using YodaApp.Utils;
using YodaApp.ViewModels.Controls;

namespace YodaApp.ViewModels
{
    internal class UserSessionViewModel : ViewModelBase
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
                if (value != null)
                {
                    // TODO this is not good
                    var task = Task.WhenAll(
                        value.LoadMembers(),
                        value.LoadLastMessages()
                        );
                }
            }
        }

        private User user;

        public User User
        {
            get => user;
            set => Set(ref user, nameof(User), value);
        }

        #endregion Properties

        #region Commands

        private ICommand _updateRoomsCommand;

        public ICommand UpdateRoomsCommand => _updateRoomsCommand ?? (_updateRoomsCommand = new AsyncRelayCommand(UpdateRoomsCommandHandler));

        private Task UpdateRoomsCommandHandler()
        {
            return UpdateRooms();
        }

        private ICommand addRoomCommand;

        public ICommand AddRoomCommand => addRoomCommand ?? (addRoomCommand = new AsyncRelayCommand(AddRoomCommandHandler));

        private async Task AddRoomCommandHandler()
        {
            var vm = componentContext.Resolve<CreateNewRoomOptionsViewModel>();
            await DialogHost.Show(
                new CreateNewRoomOptions
                {
                    DataContext = vm
                },
                "Default",
                (object sender, DialogOpenedEventArgs e) =>
                {
                    vm.RequestClose += delegate
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

        private ICommand openUserFilesCommand;

        public ICommand OpenUserFilesCommand => openUserFilesCommand ?? (openUserFilesCommand = new AsyncRelayCommand(OpenUserFiles));

        private Task OpenUserFiles()
        {
            var vm = new FilesListViewModel(Api);
            var v = new FilesList { DataContext = vm };
            return DialogHost.Show(v, "Default");
        }

        #endregion Commands

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
            try
            {
                Quote = await provider.PingAsync();
            }
            catch (Exception exc)
            {
                await ErrorView.Show(exc);
            }
        }

        private async Task Connect()
        {
            try
            {
                handler = await Api.ConnectAsync();
                handler.ExceptionOccured += Handler_ExceptionOccured;
            }
            catch (Exception exc)
            {
                await ErrorView.Show(exc);
            }
        }

        private async void Handler_ExceptionOccured(object sender, YodaApiClient.Events.ExceptionEventArgs e)
        {
            await ErrorView.Show(e.Exception);
        }

        public void Disconnect()
        {
            handler.ExceptionOccured -= Handler_ExceptionOccured;
            handler.Disconnect();
            handler = null;
        }

        public async Task UpdateRooms()
        {
            Guid? roomGuid = SelectedRoom?.Id;
            var roomIds = Rooms.Select(r => r.Id).ToList();
            SelectedRoom = null;

            Rooms.Clear();

            try
            {
                foreach (var room in await Api.GetRoomsAsync())
                {
                    await AddRoom(room);
                }
            }
            catch(Exception e)
            {
                await ErrorView.Show(e);
            }


            if (roomGuid != null)
            {
                SelectedRoom = Rooms.Where(r => r.Id == roomGuid).SingleOrDefault();
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