using System;
using System.Collections.Generic;
using System.IO;
using IA.SDK;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Addons
{
    public class AddonManager
    {
        public string CurrentDirectory { get; private set; } = Directory.GetCurrentDirectory() + "/modules/";

        /// <summary>
        /// Loads addons in ./modules folder
        /// </summary>
        public void Load()
        {
            if(!Directory.Exists(CurrentDirectory) || Directory.GetFiles(CurrentDirectory).Length == 0)
            {
                Log.Warning("No modules found, ignoring...");
                Directory.CreateDirectory(CurrentDirectory);
                return;
            }

            string[] allFiles = Directory.GetFiles(CurrentDirectory);

            foreach(string s in allFiles)
            {
                System.Reflection.Assembly addon = System.Reflection.Assembly.LoadFile(s);
                IAddon currentAddon = addon.CreateInstance("Addon") as IAddon;
                Module m = currentAddon.GetModule();
                
            }
        }
    }
}
