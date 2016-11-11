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
                Assembly addon = Assembly.LoadFile(s);

                string newS = s.Split('/')[s.Split('/').Length - 1];
                newS = newS.Remove(newS.Length - 4);

                BaseAddon currentAddon = addon.CreateInstance(newS + ".Addon") as BaseAddon;

                if (currentAddon != null)
                {
                    currentAddon.Create();
                    AddonInstance m = currentAddon.GetModule();
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
        }
    }
}
