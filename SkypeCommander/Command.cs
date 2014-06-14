using SKYPE4COMLib;
using SkypeCommander.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkypeCommander
{
    public class Command
    {
        /// <summary>
        /// Gets the Skype object.
        /// </summary>
        public Skype Skype { get; private set; }

        /// <summary>
        /// Gets the name of the invoked command.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets an array of the arguments.
        /// </summary>
        public string[] Arguments { get; private set; }

        /// <summary>
        /// Gets the arguments in string format.
        /// </summary>
        public string ArgumentsText { get; private set; }
        
        /// <summary>
        /// Gets if the command has arguments.
        /// </summary>
        public bool HasArguments { get { return Arguments != null && Arguments.Length > 0; } }

        /// <summary>
        /// Gets the Member object who sent the message.
        /// </summary>
        public ChatMember SenderMember { get; private set; }

        /// <summary>
        /// Gets the User object who sent the message.
        /// </summary>
        public User SenderUser { get; private set; }

        /// <summary>
        /// Gets the chat Message object.
        /// </summary>
        public ChatMessage Message { get; private set; }

        /// <summary>
        /// Gets the current Chat object.
        /// </summary>
        public Chat Chat
        {
            get
            {
                return Message.Chat;
            }
        }

        /// <summary>
        /// Gets all chat members.
        /// </summary>
        public IEnumerable<ChatMember> ChatMembers { get; private set; }

        /// <summary>
        /// Gets a KeyValuePair of Members and their equivalent Users.
        /// </summary>
        public IDictionary<ChatMember, User> ChatMembersUsers
        {
            get
            {
                return ChatMembers
                    .Select(o => new KeyValuePair<ChatMember, User>(o, SkypeCommands.GetUser(o.Handle)))
                    .ToDictionary(k => k.Key, v => v.Value);
            }
        }

        public IEnumerable<User> ChatUsers
        {
            get
            {
                return ChatMembers.Select(o => SkypeCommands.GetUser(o.Handle as string));
            }
        }

        public Command(Skype skype, string commandPreffix, ChatMessage msg)
        {
            this.Skype = skype;
            var text = msg.Body.Substring(commandPreffix.Length);
            var pieces = text.Split(' ');

            Name = pieces.First();
            Arguments = pieces.Skip(1).ToArray();
            ArgumentsText = String.Join(" ", pieces.Skip(1));

            ChatMembers = msg.Chat.MemberObjects.OfType<IChatMember>()
                .Select(o => new ChatMember { Handle = o.Handle })
                .ToList();

            SenderUser = msg.Sender;
            SenderMember = ChatMembers.FirstOrDefault(m => m.Handle == SenderUser.Handle);

            Message = msg;
        }

        /// <summary>
        /// Sends a message back to the sender.
        /// </summary>
        /// <param name="message">Message to be sent.</param>
        /// <param name="args">Arguments to insert into the message.</param>
        public void ReplyPrivate(string message, params object[] args)
        {
            if (Skype.CurrentUserHandle == SenderUser.Handle)
            {
                Chat.SendMessage(String.Format(message, args));
            }
            else
            {
                Skype.SendMessage(SenderUser.Handle, String.Format(message, args));
            }
        }

        /// <summary>
        /// Sends a message back to the chat.
        /// </summary>
        /// <param name="message">Message to be sent.</param>
        /// <param name="args">Arguments to insert into the message.</param>
        public void ReplyChat(string message, params object[] args)
        {
            Chat.SendMessage(String.Format(message, args));
        }
    }
}
