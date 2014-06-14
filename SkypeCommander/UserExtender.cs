using SKYPE4COMLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkypeCommander
{
    public static class UserExtender
    {
        public static string GetDisplayName(this User user)
        {
            return String.IsNullOrWhiteSpace(user.DisplayName)
                ? (String.IsNullOrWhiteSpace(user.FullName) ? user.Handle : user.FullName)
                : user.DisplayName;
        }
    }
}
