using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IA.SDK;

namespace IA.Addon.Happy
{
    public class Addon : BaseAddon
    {
        public override void Create()
        {
            base.Create();

            instance.CreateModule(x =>
            {
                x.name = "happy module";
                x.events = new List<CommandEvent>()
                {
                    new CommandEvent(cmd =>
                    {
                        cmd.name = "test";
                        cmd.processCommand = async (msg, args) =>
                        {
                            await msg.Channel.SendMessage("Hello!");
                        };
                    })
                };
            });
        }
    }
}
