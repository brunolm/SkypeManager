using log4net;
using SKYPE4COMLib;
using SkypeCommander.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SkypeCommander
{
    public class SkypeCommands
    {
        private static ILog log = LogManager.GetLogger(typeof(SkypeCommands));

        public static Skype Skype = new Skype();

        public const string CommandPreffix = "//";

        public static List<string> Providers { get; set; }

        #region Settings

        public static void LoadSettings()
        {
            Providers = Directory.GetFiles("Providers", "*Provider.dll", SearchOption.TopDirectoryOnly)
                .OfType<string>().Select(o => new FileInfo(o).Name).ToList();
        }

        #endregion

        #region Skype methods

        public static string GetName(User user)
        {
            if (user == null)
                throw new NullReferenceException("user");

            return String.IsNullOrWhiteSpace(user.DisplayName)
                ? user.FullName
                : user.DisplayName;
        }

        public static User GetUser(string handler)
        {
            return Skype.get_User(handler);
        }

        public static bool IsMod(ChatMember member)
        {
            return member.Role == TChatMemberRole.chatMemberRoleCreator
                || member.Role == TChatMemberRole.chatMemberRoleMaster;
        }

        #endregion

        #region Events

        public static void OnMessageStatus(ChatMessage pMessage, TChatMessageStatus Status)
        {
            bool execute = Status == TChatMessageStatus.cmsReceived || Status == TChatMessageStatus.cmsSending;

            if (pMessage.Body.StartsWith(CommandPreffix) && execute)
            {
                log.Debug(String.Format("Command acquired: {0}", pMessage.Body));

                var cmd = new SkypeCommander.Command(SkypeCommands.Skype, CommandPreffix, pMessage);

                var defaultProvider = new DefaultProvider();
                defaultProvider.Invoke(cmd);

                #region 3rd party providers

                foreach (var providerName in Providers)
                {
                    var assembly = Assembly.LoadFrom(@"Providers\" + providerName);
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.IsSubclassOf(typeof(CommandProvider)))
                        {
                            CommandProvider provider = (CommandProvider)assembly.CreateInstance(type.FullName);
                            provider.Invoke(cmd);
                        }
                    }
                }

                #endregion
            }
        }

        public static void OnError(SKYPE4COMLib.Command pCommand, int Number, string Description)
        {
            log.Error(String.Format("#{0}: {1}", Number, Description));
        }

        #endregion
    }

    [Flags]
    public enum ChatOptions
    {
        JoiningEnabled = 1,
        JoinersBecomeApplicants = 2,
        JoinersBecomeListeners = 4,
        HistoryDisclosed = 8,
        UsersAreListeners = 16,
        TopicAndPictureLockedForUsers = 32,
    }
}