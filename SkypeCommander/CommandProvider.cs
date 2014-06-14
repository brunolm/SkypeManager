using log4net;
using SKYPE4COMLib;
using SkypeCommander.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SkypeCommander
{
    public abstract class CommandProvider
    {
        public ChatMember GetCurrentMember(IEnumerable<ChatMember> members)
        {
            return GetMember(members, SkypeCommands.Skype.CurrentUser);
        }

        public ChatMember GetMember(IEnumerable<ChatMember> members, User user)
        {
            if (members == null || user == null)
                return null;

            return members.FirstOrDefault(o => o.Handle == user.Handle);
        }

        public IDictionary<ChatMember, User> GetMemberUsersByPartial(IDictionary<ChatMember, User> memberUsers, string name)
        {
            var d = new Dictionary<ChatMember, User>();

            var memberUser = memberUsers.FirstOrDefault(o => o.Value.Handle == name);

            if (memberUser.Key != null)
            {
                d.Add(memberUser.Key, memberUser.Value);
                return d;
            }

            d = memberUsers
                .Where(o => (o.Value.FullName.StartsWith(name, StringComparison.InvariantCultureIgnoreCase))
                    || (o.Value.DisplayName.StartsWith(name, StringComparison.InvariantCultureIgnoreCase))
                    || (o.Value.Handle.StartsWith(name, StringComparison.InvariantCultureIgnoreCase)))
                .ToDictionary(k => k.Key, v => v.Value);

            if (d.Count == 0)
            {
                d = memberUsers
                        .Where(o => (o.Value.FullName.IndexOf(name, StringComparison.InvariantCultureIgnoreCase) >= 0)
                            || (o.Value.DisplayName.IndexOf(name, StringComparison.InvariantCultureIgnoreCase) >= 0)
                            || (o.Value.Handle.IndexOf(name, StringComparison.InvariantCultureIgnoreCase) >= 0))
                        .ToDictionary(k => k.Key, v => v.Value);
            }

            return d;
        }

        public KeyValuePair<ChatMember, User>? GetMemberUserByPartial(IDictionary<ChatMember, User> memberUsers, string name)
        {
            var d = GetMemberUsersByPartial(memberUsers, name);

            if (d != null)
            {
                var o = d.FirstOrDefault();
                if (o.Key != null)
                    return o;
            }

            return null;
        }

        public ChatMember GetMemberByPartial(IDictionary<ChatMember, User> memberUsers, string name)
        {
            return GetMemberUsersByPartial(memberUsers, name).Select(o => o.Key).FirstOrDefault();
        }

        protected virtual MethodInfo GetMethod(string name)
        {
            var methods = GetType().GetMethods();
            MethodInfo method = null;

            foreach (var m in methods)
            {
                if (m.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    var parameters = m.GetParameters();
                    if (parameters == null || parameters.Count() != 1)
                        continue;

                    if (parameters.First().ParameterType == typeof(Command))
                    {
                        method = m;
                        break;
                    }
                }
            }

            return method;
        }

        public bool CommandExist(string name)
        {
            return GetMethod(name) != null;
        }

        public void Invoke(Command cmd)
        {
            var method = GetMethod(cmd.Name);

            if (method != null)
            {
                try
                {
                    new Thread(() =>
                    {
                        method.Invoke(this, new object[] { cmd });
                    }).Start();
                }
                catch (Exception ex)
                {
                    LogManager.GetLogger(method.DeclaringType)
                        .Error(String.Format("Invoking: {0}.{1}", method.DeclaringType.FullName, method.Name), ex);
                }
            }
        }
    }
}
