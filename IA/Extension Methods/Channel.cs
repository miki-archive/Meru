using System.Threading.Tasks;

namespace Discord
{
    public static class ChannelExtension
    {
        public static async Task<Message> SendMessageAndDelete(this Channel channel, string message, int seconds)
        {
            Message m = await channel.SendMessage(message);
            if(seconds > 0)
            { 
                await Task.Run(() => DeleteMessage(seconds, m));
            }
            return m;
        }

        static async Task DeleteMessage(int s, Message message)
        {
            await Task.Delay(s * 1000);
            await message.Delete();
        }
    }
}
