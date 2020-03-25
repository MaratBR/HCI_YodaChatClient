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

    enum RoomStatus
    {
        Joined,
        Left,
        AwaitingServerReponse
    }

    class RoomViewModel : ViewModelBase
    {
        private readonly Room room;

        #region Properties

        public string Name => room.Name;
        public string Description => room.Description;
        public Guid Id => room.Id;

        public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>();

        private RoomStatus status = RoomStatus.Left;

        public RoomStatus Status
        {
            get => status;
            set
            {
                Set(ref status, nameof(Status), value);
                OnPropertyChanged(nameof(IsJoined));
                OnPropertyChanged(nameof(IsLeft));
                OnPropertyChanged(nameof(IsAwaitingServerResponse));
            }
        }

        public bool IsJoined => Status == RoomStatus.Joined;
        public bool IsAwaitingServerResponse => Status == RoomStatus.AwaitingServerReponse;
        public bool IsLeft => Status == RoomStatus.Left;

        private string draft;

        public string Draft
        {
            get => draft;
            set
            {
                Set(ref draft, nameof(Draft), value);
                HasMessageChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public bool HasMessage => !string.IsNullOrWhiteSpace(Draft);

        #endregion

        public event EventHandler HasMessageChanged;

        #region Commands


        #endregion

        public RoomViewModel(Room room)
        {
            this.room = room;
        }
    }
}
