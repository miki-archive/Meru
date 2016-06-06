using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    class EventListener
    {
        public string Identifier = "`";

        Dictionary<string, CommandEvent> onCommandEvents = new Dictionary<string, CommandEvent>();
        Dictionary<string, MentionEvent> onMentionEvents = new Dictionary<string, MentionEvent>();

        public void AddCommandEvent(Action<EventInformation> populator)
        {
            EventInformation info = new EventInformation("error", null);
            populator.Invoke(info);
            onCommandEvents.Add(info.name, new CommandEvent(info));
        }

        public void AddMentionEvent(Action<EventInformation> populator)
        {
            EventInformation info = new EventInformation("error", null);
            populator.Invoke(info);
            onMentionEvents.Add(info.name, new MentionEvent(info));
        }


        public async Task OnMessageEvent(MessageEventArgs e)
        {
            Log.Message("Message Event Fired!");
            string input = e.Message.RawText;
            if (input.StartsWith(Identifier))
            {
                Log.Message("Command spotted!");

                input = input.Substring(Identifier.Length);
                if (onCommandEvents.ContainsKey(input.Split(' ')[0]))
                {
                    Log.Message("running " + input.Split(' ')[0]);
                    onCommandEvents[input.Split(' ')[0]].Trigger(e);
                }
            }
            else if (e.Message.IsMentioningMe())
            {
                await OnMentionEvent(e);
            }
            await Task.Delay(0);
        }
        public async Task OnMentionEvent(MessageEventArgs e)
        {
            foreach (KeyValuePair<string, MentionEvent> item in onMentionEvents)
            {
                item.Value.Trigger(e);
            }
            await Task.Delay(0);
        }


        public string List()
        {
            string output = "";
            foreach(KeyValuePair<string, CommandEvent> value in onCommandEvents)
            {
                output += value.Key + " ";
            }
            return output;
        }
    }
}
