using System;
using System.Collections.Generic;
using System.Text;
using Meru.Common;
using SlackConnector.Models;

namespace Meru.Providers.Slack.Objects
{
    class SlackUserObject : IUserObject
    {
        private SlackUser user;     

        public SlackUserObject(SlackUser user)
        {
            this.user = user;
        }

        public object Id => user.Id;

        public Type OriginalIdType => typeof(string);

        public DateTimeOffset CreatedAt => DateTime.Now;
    }
}
