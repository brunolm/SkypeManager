using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkypeManager
{
    public class HubManager
    {
        //private static HubConnection hubConnection = new HubConnection("http://local.knowledgeexchange.com/");
        //private static IHubProxy postProxy = hubConnection.CreateHubProxy("PostHub");

        public static void Attach()
        {
        //    hubConnection.Start().ContinueWith(task =>
        //    {
        //        if (task.IsFaulted)
        //        {
        //            Console.WriteLine("There was an error opening the connection:{0}", task.Exception.GetBaseException());
        //        }
        //        else
        //        {
        //            Console.WriteLine("Connected");
        //        }

        //    }).Wait();

        //    postProxy.On("PostCreated", (json) =>
        //    {
        //        //{"ID":"ca54db71-fcb4-11e3-bf74-bc5ff4bcd237","Title":"eeee","Text":"eeee"}
        //        dynamic post = JObject.Parse(json);

        //        // Chat.SendMessage("User {0} asked: {1}\nLink: {2}");
        //    });

        //    hubConnection.Start();
        }
    }
}
