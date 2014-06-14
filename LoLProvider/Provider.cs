using SkypeCommander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LoLProvider
{
    public class Provider : CommandProvider
    {
        public void Lk(Command cmd)
        {
            string[] names = cmd.ArgumentsText.Split(',');

            foreach (string pname in names)
            {
                string lookupName = pname.Trim();

                using (WebClient c = new WebClient())
                {
                    c.DownloadStringCompleted += (s, e) =>
                    {
                        string page = e.Result;
                        var m = Regex.Match(page, @"onclick=\""window.location='/summoner/br/(?<ID>\d+)");
                        int id = m.Groups["ID"].Success ? Convert.ToInt32(m.Groups["ID"].Value) : 0;

                        if (id > 0)
                            cmd.Chat.SendMessage(String.Format("LoLKing '{0}': http://www.lolking.net/summoner/br/{1}", lookupName, id));
                        else
                            cmd.Chat.SendMessage(String.Format("User '{0}' was not found", lookupName));
                    };
                    c.DownloadStringAsync(new Uri("http://www.lolking.net/search?name=" + Uri.EscapeDataString(lookupName)));
                }
            }
        }

        public void Ln(Command cmd)
        {
            string name = Uri.EscapeDataString(cmd.ArgumentsText);
            cmd.Chat.SendMessage(
                String.Format("http://www.lolnexus.com/scouter/search?name={0}&server=BR"
                    , name
                )
            );
        }
    }
}
