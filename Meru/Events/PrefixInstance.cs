using IA.Models;
using IA.Models.Context;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace IA.Events
{
    public static class ConcurrentUtils
    {
        public static async Task<TValue> GetOrAddAsync<TKey, TValue>(
    this ConcurrentDictionary<TKey, TValue> dictionary,
    TKey key, Func<TKey, Task<TValue>> valueFactory)
        {
            TValue resultingValue;
            if (dictionary.TryGetValue(key, out resultingValue))
            {
                return resultingValue;
            }
            return dictionary.GetOrAdd(key, await valueFactory(key));
        }
    }

    public class PrefixInstance
    {
        public static PrefixInstance Default = null;

        public short Id { internal set; get; }
        public string Value { internal set; get; }
        public string DefaultValue { internal set; get; }

        public bool Changable { internal set; get; }
        public bool ForceCommandExecution { internal set; get; }

        public bool IsDefault => this == Default;

        private ConcurrentDictionary<ulong, string> cache = new ConcurrentDictionary<ulong, string>();

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

        async Task<Identifier> CreateNewAsync(long id)
        {
            Identifier i = null;

            using (var context = new IAContext())
            {
                i = context.Identifiers.Add(new Identifier() { GuildId = id, DefaultValue = DefaultValue, Value = DefaultValue });
                await context.SaveChangesAsync();
            }

            return i;
        }

        public async Task<string> GetForGuildAsync(ulong id)
        {
            if (Changable)
            {
                return await cache.GetOrAddAsync(id, async (x) =>
                {
                    long guildId = id.ToDbLong();
                    Identifier identifier = null;

                    using (var context = new IAContext())
                    {
                        identifier = await context.Identifiers.FindAsync(guildId, DefaultValue);
                        if (identifier == null)
                        {
                            identifier = await CreateNewAsync(guildId);
                        }
                    }

                    return identifier.Value;
                });
            }
            return DefaultValue;
        }
    }
}