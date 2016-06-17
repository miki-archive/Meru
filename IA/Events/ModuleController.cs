using Discord;
using IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    class ModuleController
    {
        Dictionary<string, EventModule> modules = new Dictionary<string, EventModule>();

        public void AddCommand(string module, Action<EventInformation> info)
        {
            EventInformation eInfo = new EventInformation("not_set", null);
            info.Invoke(eInfo);
            if(!modules.ContainsKey(module))
            {
                Log.DoneAt("ModuleController", "module " + module + " exists!");
                AddModule(x =>
                {
                    x.name = module;
                    x.enabled = true;
                });
            }
            modules[module].commandEvents.Add(eInfo.name, new CommandEvent(eInfo));
            Log.DoneAt("ModuleController", "Command " + eInfo.name + " loaded!");
        }

        public void AddMention(string module, Action<EventInformation> info)
        {
            EventInformation eInfo = new EventInformation("not_set", null);
            info.Invoke(eInfo);
            if (!modules.ContainsKey(module))
            {
                Log.DoneAt("ModuleController", "module " + module + " exists!");
                AddModule(x =>
                {
                    x.name = module;
                    x.enabled = true;
                });
            }
            modules[module].mentionEvents.Add(eInfo.name, new MentionEvent(eInfo));
            Log.DoneAt("ModuleController", "Command " + eInfo.name + " loaded!");
        }

        public void AddModule(Action<EventModuleInformation> mInfo)
        {
            EventModuleInformation moduleInfo = new EventModuleInformation();
            mInfo.Invoke(moduleInfo);
            if (modules.ContainsKey(moduleInfo.name))
            {
                Log.ErrorAt("ModuleController", "Module already exists.");
                return;
            }
            modules.Add(moduleInfo.name, new EventModule(mInfo));
            Log.DoneAt("ModuleController", "Module created!");
        }

        public async Task Check(MessageEventArgs e)
        {
            if(e.Message.IsMentioningMe())
            {
                foreach (KeyValuePair<string, EventModule> item in modules)
                {
                    item.Value.OnMention(e);
                }
            }

            foreach (KeyValuePair<string, EventModule> item in modules)
            {
                item.Value.OnCommand(e);
            }
            await Task.Delay(0);
        }

        public string List(MessageEventArgs e)
        {
            string output = "";
            foreach (KeyValuePair<string, EventModule> item in modules)
            {
                output += item.Value.List(e) + "\n\n";
            }
            return output;
        }

        internal void ToggleCommand(MessageEventArgs e, string v)
        {
            Event ev = null;
            foreach (KeyValuePair<string, EventModule> item in modules)
            {
                ev = item.Value.FindEventWithName(v);
                if(ev != null)
                {
                    if (!ev.info.ContainsKey(e.Channel.Id))
                    {
                        ev.Load(e.Channel.Id);
                    }
                    ev.SetActive(e.Channel.Id, !ev.info[e.Channel.Id].enabled);
                    SQL.Query("UPDATE event_information_" + ev.baseEventInformation.name + " SET enabled=" + ev.info[e.Channel.Id].enabled + " WHERE id=" + e.Channel.Id);
                }
            }
            if(ev != null)
            {
                e.Channel.SendMessage(v + " does not exist.");
            }
            else
            {
                e.Channel.SendMessage(":ok_hand:");
            }
        }

        public void ToggleModule(MessageEventArgs e, string v)
        {
            foreach (KeyValuePair<string, EventModule> item in modules)
            {
                if (item.Key == v)
                {
                }
            }

        }

        public void FinishLoading()
        {
            List<KeyValuePair<string, EventModule>> temp = modules.ToList();
            temp.Sort((x, y) => { return x.Key.CompareTo(y.Key); });
            modules = temp.ToDictionary(v => v.Key, v => v.Value);
        }
    }
}
