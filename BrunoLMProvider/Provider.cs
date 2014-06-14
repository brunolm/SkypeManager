using SkypeCommander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrunoLMProvider
{
    public class Provider : CommandProvider
    {
        public void Youtube(Command cmd)
        {
            if (cmd.Arguments.Count() == 0)
                return;

            Dictionary<string, string> videoParams = new Dictionary<string, string>();

            videoParams.Add("autoplay", "1");
            videoParams.Add("loop", "1");
            videoParams.Add("iv_load_policy", "3");
            videoParams.Add("modestbranding", "1");
            videoParams.Add("autohide", "1");


            for (int i = 1; i <= cmd.Arguments.Count() - 1; ++i)
            {
                if (cmd.Arguments[i].StartsWith("-"))
                {
                    if (cmd.Arguments[i].Contains("a"))
                        videoParams.Remove("autoplay");
                    if (cmd.Arguments[i].Contains("l"))
                        videoParams.Remove("loop");
                }
            }

            string videoID = Regex.Match(cmd.Arguments[0], "v=(?<ID>[^&]+)").Groups["ID"].Value;
            string videoUrl = String.Format("http://www.youtube.com/v/{0}?{1}"
                , videoID
                , String.Join("&", videoParams.Select(o => String.Format("{0}={1}", o.Key, o.Value))));

            cmd.Chat.SendMessage(videoUrl);
        }
    }
}
