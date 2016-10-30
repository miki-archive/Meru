using System;
using System.Collections.Generic;
using System.IO;
using IA.SDK;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IA.Events;

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
            if(!Directory.Exists(CurrentDirectory) || Directory.GetFiles(CurrentDirectory).Length == 0)
            {
                Log.Warning("No modules found, ignoring...");
                Directory.CreateDirectory(CurrentDirectory);
                return;
            }

            string[] allFiles = Directory.GetFiles(CurrentDirectory);

            foreach (string s in allFiles)
            {
                System.Reflection.Assembly addon = System.Reflection.Assembly.LoadFile(s);

                string newS = s.Split('/')[s.Split('/').Length -1];
                newS = newS.Remove(newS.Length - 4);

                IAddon currentAddon = addon.CreateInstance(newS + ".Addon") as IAddon;
                if (currentAddon != null)
                {
                    ModuleInstance m = currentAddon.GetModule();
                    Module newModule = new Module(m);
                    await newModule.InstallAsync(bot);
                    Log.Done($"loaded module \"{newS}\" successfully");
                }
                else
                {
                    Log.Error($"failed to load module \"{newS}\"");
                }
            }
        }
    }
}
