using SKYPE4COMLib;
using SkypeCommander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefaultProvider
{
    public class Provider : CommandProvider
    {
        #region Mod controls

        public void Kick(SkypeCommander.Command cmd)
        {
            if (SkypeCommands.IsMod(cmd.SenderMember) && cmd.HasArguments)
            {
                var member = GetMemberByPartial(cmd.ChatMembersUsers, cmd.ArgumentsText);

                if (member != null)
                    cmd.Message.Chat.Kick(member.Handle);
            }
        }

        public void SetRole(SkypeCommander.Command cmd)
        {
            if (SkypeCommands.IsMod(cmd.SenderMember) && cmd.Arguments.Length > 1)
            {
                var role = cmd.Arguments[1].ToLower();

                var member = GetMemberByPartial(cmd.ChatMembersUsers, cmd.Arguments[0]);

                if (member != null)
                {
                    bool roleFound = false;
                    var currentMember = GetCurrentMember(cmd.ChatMembers);

                    switch (role)
                    {
                        case "helper":
                            if (currentMember.CanSetRoleTo(TChatMemberRole.chatMemberRoleHelper))
                            {
                                if (member.Role != TChatMemberRole.chatMemberRoleHelper)
                                {
                                    member.Role = TChatMemberRole.chatMemberRoleHelper;
                                    roleFound = true;
                                }
                            }
                            break;
                        case "listener":
                            if (currentMember.CanSetRoleTo(TChatMemberRole.chatMemberRoleListener))
                            {
                                if (member.Role != TChatMemberRole.chatMemberRoleListener)
                                {
                                    member.Role = TChatMemberRole.chatMemberRoleListener;
                                    roleFound = true;
                                }
                            }
                            break;
                        case "master":
                            if (currentMember.CanSetRoleTo(TChatMemberRole.chatMemberRoleMaster))
                            {
                                if (member.Role != TChatMemberRole.chatMemberRoleMaster)
                                {
                                    member.Role = TChatMemberRole.chatMemberRoleMaster;
                                    roleFound = true;
                                }
                            }
                            break;
                        case "user":
                            if (currentMember.CanSetRoleTo(TChatMemberRole.chatMemberRoleUser))
                            {
                                if (member.Role != TChatMemberRole.chatMemberRoleUser)
                                {
                                    member.Role = TChatMemberRole.chatMemberRoleUser;
                                    roleFound = true;
                                }
                            }
                            break;
                    }

                    if (roleFound)
                        cmd.ReplyChat(member.Handle + " promoted to " + role);
                }
            }
        }

        public void Invite(SkypeCommander.Command cmd)
        {
            if (SkypeCommands.IsMod(cmd.SenderMember))
            {
                cmd.Chat.AddMembers(new UserCollection { SkypeCommands.GetUser(cmd.ArgumentsText) });
            }
        }

        #endregion

        //public void Whois(SkypeCommander.Command cmd)
        //{
        //    var memberUser = GetMemberUserByPartial(cmd.ChatMembersUsers, cmd.ArgumentsText).Value;

        //    var member = memberUser.Key;
        //    var user = memberUser.Value;

        //    var lastonline = user.LastOnline > DateTime.Now.AddYears(-1)
        //        ? user.LastOnline.ToString(new System.Globalization.CultureInfo("pt-BR"))
        //        : "unknown";

        //    var msg = String.Format("ID: {1}{0}Name: {2}{0}Role: {3}{0}Last online: {4}"
        //        , Environment.NewLine
        //        , member.Handle
        //        , user.GetDisplayName()
        //        , member.Role
        //        , lastonline);

        //    cmd.ReplyPrivate(msg);
        //}

        public void Masters(SkypeCommander.Command cmd)
        {
            var memberUsers = cmd.ChatMembersUsers
                .Where(o => o.Key.Role == TChatMemberRole.chatMemberRoleCreator
                    || o.Key.Role == TChatMemberRole.chatMemberRoleMaster);

            cmd.ReplyPrivate("Group Mods\n{0}", String.Join(Environment.NewLine, memberUsers.Select(o => o.Value.GetDisplayName())));
        }

        public void ForkActive(SkypeCommander.Command cmd)
        {
            var latestMessages = cmd.Chat.Messages.OfType<SKYPE4COMLib.ChatMessage>()
                .Take(500)
                .Where(o => o.Timestamp > DateTime.Now.AddMinutes(-15))
                .DistinctBy(o => o.FromHandle);

            UserCollection users = new UserCollection();
            var ulist = latestMessages.Select(o => SkypeCommands.GetUser(o.FromHandle))
                .Where(o => o.Handle != SkypeCommands.Skype.CurrentUserHandle)
                .ToList();

            var userCollection = new UserCollection();
            ulist.ForEach(u => userCollection.Add(u));

            var chat = SkypeCommands.Skype.CreateChatMultiple(userCollection);

            if (chat != null)
            {
                chat.SendMessage(String.Format("Chat created {0} with {1} members", DateTime.Now.ToShortTimeString(), ulist.Count));
            }
        }

        public void ListInactive(SkypeCommander.Command cmd)
        {
            int days = 0;
            int.TryParse(cmd.ArgumentsText, out days);

            if (days == 0)
                days = 7;

            var ulist = cmd.Chat.Messages.OfType<SKYPE4COMLib.ChatMessage>()
                .Where(o => o.Timestamp >= DateTime.Now.AddDays(-days))
                .DistinctBy(o => o.FromHandle)
                .Select(o => SkypeCommands.GetUser(o.FromHandle));

            cmd.ReplyChat("Inactive users for past {0} days\n{1}"
                , days
                , String.Join(Environment.NewLine, cmd.ChatMembers.Where(o => !ulist.Any(u => u.Handle == o.Handle)).Select(o => SkypeCommands.GetUser(o.Handle).GetDisplayName())));
        }

        public void GetChatOptions(SkypeCommander.Command cmd)
        {
            ChatOptions options = (ChatOptions)cmd.Chat.Options;
            cmd.ReplyChat(String.Format("Options: {0}", options));
        }
    }
}
