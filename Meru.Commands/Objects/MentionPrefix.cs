using Meru.Common;
using System.Threading.Tasks;

namespace Meru.Commands
{
	public class MentionPrefix : Prefix
	{
		public MentionPrefix() : base("")
		{ }

		public override async Task<string> GetPrefixAsync(IMessage msg)
		{
			if( msg.Content.StartsWith($"<@{Value}>"))
			{
				return $"<@{Value}>";
			}
			else if(msg.Content.StartsWith($"<@!{Value}>"))
			{
				return $"<@!{Value}>";
			}
			return Value;
		}

		public override async Task<bool> MatchesAsync(IMessage msg)
		{
			if(Value == "")
			{
				Value = (await msg.GetSelfAsync()).Id.ToString();
			}

			return msg.Content.StartsWith($"<@{Value}>") ||
				   msg.Content.StartsWith($"<@!{Value}>");
		}
	}
}