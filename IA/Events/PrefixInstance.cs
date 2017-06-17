using Meru.Models;
using Meru.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meru.Events
{
    public class PrefixInstance
    {
        public static PrefixInstance Default = null;

        public short Id { internal set; get; }
        public string Value { internal set; get; }
        public string DefaultValue { internal set; get; }

        public bool Changable { internal set; get; }
        public bool ForceCommandExecution { internal set; get; }

        public bool IsDefault => this == Default;

        internal PrefixInstance(string value, bool changable, bool forceExec)
        {
            Value = value;
            DefaultValue = value;
            Changable = changable;
            ForceCommandExecution = forceExec;
        }

        public void RegisterAsDefault()
        {
            if (Default == null)
            {
                Default = this;
            }
            else
            {
                Log.WarningAt("SetDefaultPrefix", "Default prefix is already defined!");
            }
        }

        public async Task ChangeForGuildAsync(ulong id, string prefix)
        {
            if (Changable)
            {
                using (var context = new IAContext())
                {
                    long guildId = id.ToDbLong();

                    Identifier i = await context.Identifiers.FindAsync(guildId, DefaultValue);
                    if (i == null)
                    {
                        context.Identifiers.Add(new Identifier() { GuildId = guildId, DefaultValue = DefaultValue, Value = prefix });
                    }
                    else
                    {
                        i.Value = prefix;
                    }
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task<string> GetForGuildAsync(ulong id)
        {
            if(Changable)
            {
                long guildId = id.ToDbLong();

                using (var context = new IAContext())
                {
                    Identifier identifier = await context.Identifiers.FindAsync(guildId, DefaultValue);
                    if(identifier == null)
                    {
                        context.Identifiers.Add(new Identifier() { GuildId = guildId, DefaultValue = DefaultValue, Value = DefaultValue });
                        await context.SaveChangesAsync();
                    }
                    return identifier.Value;
                }
            }
            return DefaultValue;
        }
    }
}
