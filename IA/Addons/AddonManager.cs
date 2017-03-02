using IA.SDK;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace IA.Addons
{
    public class AddonManager
    {
        public string CurrentDirectory { get; private set; } = Directory.GetCurrentDirectory() + "/modules/";

        /// <summary>
        /// Loads addons in ./modules folder
        /// </summary>
        public async Task Load(Bot bot)
        {
            if (!Directory.Exists(CurrentDirectory) || Directory.GetFiles(CurrentDirectory).Length == 0)
            {
                Log.Warning("No modules found, ignoring...");
                Directory.CreateDirectory(CurrentDirectory);
                return;
            }

            string[] allFiles = Directory.GetFiles(CurrentDirectory);

            foreach (string s in allFiles)
            {
                string newS = s.Split('/')[s.Split('/').Length - 1];
                newS = newS.Remove(newS.Length - 4);

                try
                {
                    if (!s.EndsWith(".dll"))
                    {
                        continue;
                    }

                    Assembly addon = Assembly.Load(File.ReadAllBytes(s));

                    IAddon currentAddon = addon.CreateInstance(newS + ".Addon") as IAddon;

                    if (currentAddon != null)
                    {
                        RuntimeAddonInstance aInstance = new RuntimeAddonInstance();
                        aInstance = new RuntimeAddonInstance(await currentAddon.Create(aInstance));

                        foreach (ModuleInstance nm in aInstance.modules)
                        {
                            Events.Module newModule = new Events.Module(nm);
                            await newModule.InstallAsync(bot);
                        }
                        Log.Done($"loaded Add-On {newS} successfully");
                    }
                    else
                    {
                        Log.Error($"\"{newS}\" is not a module");
                    }
                }
                catch
                {
                    Log.Warning($"Module {newS} is not compatible with this version (v{Bot.VersionNumber})");
                }
            }
        }

        public async Task LoadSpecific(Bot bot, string module)
        {
            string s = CurrentDirectory + module + ".dll";

            Assembly addon = Assembly.Load(File.ReadAllBytes(s));

            string newS = s.Split('/')[s.Split('/').Length - 1];
            newS = newS.Remove(newS.Length - 4);

            IAddon currentAddon = addon.CreateInstance(newS + ".Addon") as IAddon;

            if (currentAddon != null)
            {
                RuntimeAddonInstance aInstance = new RuntimeAddonInstance();
                aInstance = new RuntimeAddonInstance(await currentAddon.Create(aInstance));

                foreach (ModuleInstance nm in aInstance.modules)
                {
                    if (bot.Events.GetModuleByName(nm.data.name) != null)
                    {
                        Log.Warning("Module already loaded, stopping load");
                        return;
                    }
                    Events.Module newModule = new Events.Module(nm);
                    await newModule.InstallAsync(bot);
                }

                Log.Done($"Loaded Add-On \"{newS}\" successfully");
            }
            else
            {
                Log.Error($"failed to reload module \"{newS}\"");
            }
        }

        public async Task Reload(Bot bot, string module)
        {
            string s = CurrentDirectory + module + ".dll";

            Assembly addon = Assembly.Load(File.ReadAllBytes(s));

            string newS = s.Split('/')[s.Split('/').Length - 1];
            newS = newS.Remove(newS.Length - 4);

            IAddon currentAddon = addon.CreateInstance(newS + ".Addon") as IAddon;

            if (currentAddon != null)
            {
                RuntimeAddonInstance aInstance = new RuntimeAddonInstance();
                aInstance = new RuntimeAddonInstance(await currentAddon.Create(aInstance));

                foreach (ModuleInstance nm in aInstance.modules)
                {
                    await bot.Events.GetModuleByName(nm.data.name).UninstallAsync(bot);

                    Events.Module newModule = new Events.Module(nm);
                    await newModule.InstallAsync(bot);
                }
                Log.Done($"Reloaded Add-On \"{newS}\" successfully");
            }
            else
            {
                Log.Error($"failed to reload module \"{newS}\"");
            }
        }

        public async Task Unload(Bot bot, string module)
        {
            string s = CurrentDirectory + module + ".dll";

            Assembly addon = Assembly.Load(File.ReadAllBytes(s));

            string newS = s.Split('/')[s.Split('/').Length - 1];
            newS = newS.Remove(newS.Length - 4);

            IAddon currentAddon = addon.CreateInstance(newS + ".Addon") as IAddon;

            if (currentAddon != null)
            {
                RuntimeAddonInstance aInstance = new RuntimeAddonInstance();
                aInstance = new RuntimeAddonInstance(await currentAddon.Create(aInstance));

                foreach (ModuleInstance nm in aInstance.modules)
                {
                    Events.Module mod = bot.Events.GetModuleByName(nm.data.name);

                    if (mod != null)
                    {
                        await mod.UninstallAsync(bot);
                    }
                }
                Log.Done($"Unloaded Add-On \"{newS}\" successfully");
            }
            else
            {
                Log.Error($"failed to unload module \"{newS}\"");
            }
        }
    }
}