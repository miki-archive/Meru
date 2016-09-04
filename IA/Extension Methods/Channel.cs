using System.Threading.Tasks;

namespace Discord
{
    public static class ChannelExtension
    {
        public static async Task<IUserMessage> SendMessageAndDelete(this IMessageChannel channel, string message, int seconds)
        {
            IUserMessage m = await channel.SendMessageAsync(message);
            if(seconds > 0) await Task.Run(() => DeleteMessage(m, seconds));
            return m;
        }

        static async Task DeleteMessage(IUserMessage message, int seconds)
        {
            await Task.Delay(seconds * 1000);
            await message.DeleteAsync();
        }
    }
}
