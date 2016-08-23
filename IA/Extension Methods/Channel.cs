using System.Threading.Tasks;

namespace Discord
{
    public static class ChannelExtension
    {
        public static async Task<Message> SendMessageAndDelete(this Channel channel, string message, int seconds)
        {
            Message m = await channel.SendMessage(message);
            if(seconds > 0) await Task.Run(() => DeleteMessage(m, seconds));
            return m;
        }

        static async Task DeleteMessage(Message message, int seconds)
        {
            await Task.Delay(seconds * 1000);
            await message.Delete();
        }
    }
}
