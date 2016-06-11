using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    class EventModule
    {
        public Dictionary<string, CommandEvent> commandEvents = new Dictionary<string, CommandEvent>();
        public Dictionary<string, MentionEvent> mentionEvents = new Dictionary<string, MentionEvent>();

        public EventModuleInformation moduleInformation { get; private set; }

        public EventModule(Action<EventModuleInformation> moduleInfo)
        {
            moduleInformation = new EventModuleInformation();
            moduleInfo.Invoke(moduleInformation);
        }

        public void OnCommand(MessageEventArgs e)
        {
            if (e.Message.Text.StartsWith(Global.Identifier))
            {
                string command = e.Message.Text.Split(' ')[0].Substring(Global.Identifier.Length);
                foreach (KeyValuePair<string, CommandEvent> item in commandEvents)
                {
                    if (item.Key == command)
                    {
                        if (item.Value.info[e.User.Id].enabled)
                        {
                            item.Value.Trigger(e);
                        }
                    }
                }
            }
        }

        public void OnMention(MessageEventArgs e)
        {
            foreach (KeyValuePair<string, MentionEvent> item in mentionEvents)
            {
                item.Value.Trigger(e);
            }
        }


        public string List(MessageEventArgs e)
        {
            string output = "**" + moduleInformation.name + "**";
            if (commandEvents.Count > 0)
            {
                output += "\n`Commands:` ";
                foreach (KeyValuePair<string, CommandEvent> command in commandEvents)
                {
                    output += command.Value.info[e.Channel.Id].name + ", ";
                }
            }
            if (mentionEvents.Count > 0)
            {
                output += "\n`Mention Events:` ";
                foreach (KeyValuePair<string, MentionEvent> mention in mentionEvents)
                {
                    output += mention.Value.info[e.Channel.Id].name + ", ";
                }
            }
            return output.Remove(output.Length - 2);
        }

        public void SetActive(bool value)
        {
            moduleInformation.enabled = value;
        }
    }
}
