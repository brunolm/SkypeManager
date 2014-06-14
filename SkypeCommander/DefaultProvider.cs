using SKYPE4COMLib;
using SkypeCommander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkypeCommander
{
    public class DefaultProvider : CommandProvider
    {
        public void Test(Command cmd)
        {
            cmd.Message.Chat.SendMessage("Status: Listening");
        }
    }
}
