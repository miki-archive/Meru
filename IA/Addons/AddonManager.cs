using System;
using System.Collections.Generic;
using System.IO;
using IA.SDK;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IA.Events;
using System.Reflection;
using Discord;

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
                try
                {
                    if(!s.EndsWith(".dll"))
                    {
                        continue;
                    }

                    Assembly addon = Assembly.Load(File.ReadAllBytes(s));

                    string newS = s.Split('/')[s.Split('/').Length - 1];
                    newS = newS.Remove(newS.Length - 4);

                    IAddon currentAddon = addon.CreateInstance(newS + ".Addon") as IAddon;

                    if (currentAddon != null)
                    {
                        await currentAddon.Create();
                        AddonInstance m = currentAddon.GetAddon();
                        foreach (ModuleInstance nm in m.modules)
                        {
                            Events.Module newModule = new Events.Module(nm);
                            await newModule.InstallAsync(bot);
                        }
                        Log.Done($"loaded Add-On \"{newS}\" successfully");
                    }
                    else
                    {
                        Log.Error($"failed to load module \"{newS}\"");
                    }
                }
                catch
                {
                    Log.Warning($"Module {s} is not compatible with Miki");
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
                await currentAddon.Create();
                AddonInstance m = currentAddon.GetAddon();

                foreach (ModuleInstance nm in m.modules)
                {
                    if(bot.Events.GetModuleByName(nm.data.name) != null)
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
                await currentAddon.Create();
                AddonInstance m = currentAddon.GetAddon();

                foreach (ModuleInstance nm in m.modules)
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
                await currentAddon.Create();
                AddonInstance m = currentAddon.GetAddon();

                foreach (ModuleInstance nm in m.modules)
                {
                    Events.Module mod = bot.Events.GetModuleByName(nm.data.name);
                    
                    if(mod != null)
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
