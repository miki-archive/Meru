using IA.SDK;
using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK
{
    public class DiscordMessage : IDiscordMessage
    {
        public virtual ulong Id
        {
            get
            {
                return 0;
            }
        }
        public virtual DiscordUser Author { get {
                return null;
            }
        }
        public virtual DiscordChannel Channel
        {
            get
            {
                return null;
            }
        }
        public virtual DiscordGuild Guild
        {
            get
            {
                return null;
            }
        }

        public virtual string Content { get
            {
                return "";
            }
        }
        public virtual DateTimeOffset Timestamp {
            get
            {
                return new DateTime(0);
            }
        }

        public virtual IReadOnlyCollection<ulong> MentionedUserIds { get
            {
                return null;
            }
        }
        public virtual IReadOnlyCollection<ulong> MentionedRoleIds
        {
            get
            {
                return null;
            }
        }

        public virtual async Task DeleteAsync()
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }
        public virtual async Task ModifyAsync(string message)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }
        public virtual async Task PinAsync()
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }
        public virtual async Task UnpinAsync()
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }
    }
}
