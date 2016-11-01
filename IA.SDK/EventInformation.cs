using IA.SDK.Interfaces;
using System.Threading.Tasks;

namespace IA.SDK
{
    public enum EventAccessibility
    {
        PUBLIC,
        ADMINONLY,
        DEVELOPERONLY
    }

    public enum EventRange
    {
        USER,
        CHANNEL,
        SERVER
    }

    public delegate void ProcessServerCommand(DiscordGuild e);
    public delegate Task ProcessCommand(IDiscordMessage e, string args);
    public delegate bool CheckCommand(IDiscordMessage e, string command, string[] allAliases);
}
