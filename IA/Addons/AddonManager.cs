using System;
using System.Collections.Generic;
using System.IO;
using IA.SDK;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IA.Events;
using Jint;
using System.Reflection;
using Discord;
using Jurassic;
using Jurassic.Library;

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
        public async Task LoadJS(Bot bot)
        {
            if (!Directory.Exists(CurrentDirectory + "js/") || Directory.GetFiles(CurrentDirectory + "js/").Length == 0)
            {
                Log.Warning("No modules found, ignoring...");
                Directory.CreateDirectory(CurrentDirectory);
                return;
            }

            string[] allFiles = Directory.GetFiles(CurrentDirectory + "js/");

            foreach (string s in allFiles)
            {
                StreamReader sr = new StreamReader(s);
                string jscode = sr.ReadToEnd();
                sr.Close();

                var engine = new ScriptEngine();
                engine.EnableExposedClrTypes = true;

                engine.SetGlobalValue("addon", new ModuleInformation());

                engine.Execute(jscode);

                ClrInstanceWrapper output = (ClrInstanceWrapper)engine.CallGlobalFunction("create");

                Events.Module m = new Events.Module();
                m.defaultInfo = (ModuleInformation)output.WrappedInstance;
            

                await m.InstallAsync(bot);
            }
        }
    }
}
