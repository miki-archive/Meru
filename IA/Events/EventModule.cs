using Discord;
using IA.Data;
using MySql.Data.MySqlClient;
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
                        item.Value.Trigger(e);
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

        public Event FindEventWithName(string name)
        {
            foreach(CommandEvent item in commandEvents.Values)
            {
                if(item.baseEventInformation.name == name)
                {
                    return item;
                }
            }
            foreach (MentionEvent item in mentionEvents.Values)
            {
                if (item.baseEventInformation.name == name)
                {
                    return item;
                }
            }
            return null;
        }

        public string List(MessageEventArgs e)
        {
            string output = "**" + moduleInformation.name + "**";
            if (commandEvents.Count > 0)
            {
                output += "\n`Commands:` ";
                foreach (KeyValuePair<string, CommandEvent> command in commandEvents)
                {
                    if (!command.Value.info.ContainsKey(e.Channel.Id))
                    { 
                        command.Value.Load(e.Channel.Id);
                    }
                    if (!command.Value.info[e.Channel.Id].enabled)
                    {
                        output += command.Value.baseEventInformation.name + ", ";
                    }
                }
                output = output.Remove(output.Length - 2);
            }
            if (mentionEvents.Count > 0)
            {
                output += "\n`Mention Events:` ";
                foreach (KeyValuePair<string, MentionEvent> mention in mentionEvents)
                {
                    if (!mention.Value.info.ContainsKey(e.Channel.Id))
                    {
                        mention.Value.Load(e.Channel.Id);
                    }
                    output += mention.Value.baseEventInformation.name + ", ";
                }
                output = output.Remove(output.Length - 2);
            }
            return output;
        }

        public void SetActive(bool value)
        {
            moduleInformation.enabled = value;
        }
    }
}
