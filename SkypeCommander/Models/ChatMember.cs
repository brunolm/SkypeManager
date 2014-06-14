using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkypeCommander.Models
{
    public class ChatMember : SkypeUser
    {
        public SKYPE4COMLib.TChatMemberRole Role { get; set; }

        public bool CanSetRoleTo(SKYPE4COMLib.TChatMemberRole tChatMemberRole)
        {
            if (Role == SKYPE4COMLib.TChatMemberRole.chatMemberRoleCreator
                && tChatMemberRole != SKYPE4COMLib.TChatMemberRole.chatMemberRoleCreator)
            {
                return true;
            }

            if (Role == SKYPE4COMLib.TChatMemberRole.chatMemberRoleMaster
                && tChatMemberRole != SKYPE4COMLib.TChatMemberRole.chatMemberRoleCreator
                && tChatMemberRole != SKYPE4COMLib.TChatMemberRole.chatMemberRoleMaster)
            {
                return true;
            }

            return false;
        }
    }
}
