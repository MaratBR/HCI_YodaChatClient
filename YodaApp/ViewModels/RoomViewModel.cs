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
using YodaApp.Controls;
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

        private static string[] WORTHY_FIRST_MESSAGES = new string[]
        {
            "text'); DROP TABLE very_important_stuff;",
            "May the force be with you my brothers",
            "Does anyone knows where can I hid a body? It's urgent",
            "There's only two genders: respect and smooch, change my mind",
            "#This #is #my #first #message #By #the #way #I #use #HASHTAGS #all #the #time #cuz #I'm #stupid #WhyDoYouNeedHashtagsAnywayTheyAreNotSupportedHere",
            "Is it workig?", // <-- typo is intentional (actually it's not but i dont care)
            "qwerty",
            "asdfg",
            "FIRST!!!1!"
        };
        private static Random rnd = new Random();

        #region Properties

        public string Name => room.Name;
        public string Description => room.Description;
        public Guid Id => room.Id;

        private bool hasNoMessages = true;

        public bool HasNoMessages
        {
            get { return hasNoMessages; }
            set => Set(ref hasNoMessages, nameof(HasNoMessages), value);
        }

        public ObservableCollection<object> RoomFeed { get; } = new ObservableCollection<object>();

        public ObservableCollection<UserViewModel> Users { get; } = new ObservableCollection<UserViewModel>();

        private MessageViewModel message;

        public MessageViewModel Message
        {
            get { return message; }
            set => Set(ref message, nameof(Message), value);
        }

        #endregion

        #region Commands

        private ICommand _fillFirstMessageCommand;

        public ICommand FillFirstMessageCommand => _fillFirstMessageCommand ?? (_fillFirstMessageCommand = new RelayCommand(FillFirstMessageCommandHandler));

        private void FillFirstMessageCommandHandler()
        {
            if (Message != null)
                Message.Text = WORTHY_FIRST_MESSAGES[rnd.Next(0, WORTHY_FIRST_MESSAGES.Length - 1)];
        }

        #endregion

        public RoomViewModel(IRoomHandler room)
        {
            this.room = room;
            room.UserJoined += Room_UserJoined;
            room.UserDeparted += Room_UserDeparted;

            CreateNewMessage();
        }

        private void Room_UserDeparted(object sender, YodaApiClient.Events.ChatEventArgs<ChatUserDepartedDto> args)
        {
            var user = Users.Where(u => u.Id == args.InnerMessage.UserId).FirstOrDefault();
            if (user != null)
                SetOnline(args.InnerMessage.UserId, false);

        }

        private void Room_UserJoined(object sender, YodaApiClient.Events.ChatEventArgs<ChatUserJoinedRoomDto> args)
        {
            if (Users.Where(u => u.Id == args.InnerMessage.User.Id) == null)
                AddUser(args.InnerMessage.User);
            else
                SetOnline(args.InnerMessage.User.Id, true);
        }

        public void CreateNewMessage()
        {
            Message = new MessageViewModel(room);
            Message.MessageSubmitted += Message_MessageSent;
        }

        public async Task LoadLastMessages()
        {
            var messages = await room.Client.Api.GetRoomMessages(Id);

            RoomFeed.Clear();
            foreach (var message in messages)
                AddToFeed(message);
        }

        private void AddToFeed(ChatMessageDto chatMessage)
        {
            AddToFeed(new MessageViewModel(room, chatMessage));
        }

        private void AddToFeed(MessageViewModel messageVM)
        {
            if (HasNoMessages)
                HasNoMessages = false;
            RoomFeed.Add(new MessageControl
            {
                DataContext = messageVM
            });
        }

        public async Task LoadMembers()
        {
            var users = await room.Client.Api.GetRoomMembersAsync(Id);
            Users.Clear();
            users.ForEach(AddUser);
        }

        private void AddUser(ChatUserDto user)
        {
            Users.Add(new UserViewModel(user));
        }

        private void AddUser(ChatMembershipDto user)
        {
            Users.Add(new UserViewModel(user));
        }

        private void SetOnline(int userId, bool value)
        {
            var user = Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user != null)
                user.IsOnline = value;
        }

        private void Message_MessageSent(object sender, EventArgs e)
        {
            var vm = (MessageViewModel)sender;
            vm.MessageSubmitted -= Message_MessageSent;
            AddToFeed(vm);
            CreateNewMessage();
        }
    }
}
