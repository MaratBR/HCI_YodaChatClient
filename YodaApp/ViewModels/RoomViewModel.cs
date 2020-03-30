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
        private readonly IRoomHandler room;

        #region Properties

        public string Name => room.Name;
        public string Description => room.Description;
        public Guid Id => room.Id;

        public ObservableCollection<MessageViewModel> Messages { get; } = new ObservableCollection<MessageViewModel>();

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


        private MessageViewModel message;

        public MessageViewModel Message
        {
            get { return message; }
            set => Set(ref message, nameof(Message), value);
        }

        #endregion

        public RoomViewModel(IRoomHandler room)
        {
            this.room = room;
            CreateNewMessage();
        }

        private void CreateNewMessage()
        {
            var message = room.CreateMessage();
            Message = new MessageViewModel(message);
            Message.MessageSent += Message_MessageSent;
        }

        private void Message_MessageSent(object sender, EventArgs e)
        {
            var vm = (MessageViewModel)sender;
            vm.MessageSent -= Message_MessageSent;
            Messages.Add(vm);
            CreateNewMessage();
        }
    }
}
