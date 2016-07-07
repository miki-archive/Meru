using Discord;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Node
{
    public class Node
    {
        static Channel c;
        static string c_id;
        static string c_args;

        public Node()
        {

        }

        public Node(string id, string args = "", Channel outputChannel = null)
        {
            c = outputChannel;
            c_id = id;
            c_args = args;
        }

        public static void Create(string id, string code)
        {
            StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + @"\" + id + ".js");
            sw.Write(
                "if(process.argv.length > 2)" +
                "{" +
                    "var input = process.argv[2];" +
                    "input = input.replace(/_/g, ' ');" +
                "}");
            sw.Write(code);
            sw.Close();
        }

        public void Run()
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"C:\Program Files\nodejs\node.exe";
            c_args = c_args.Replace(' ', '_');
            start.Arguments = string.Format("{0} {1}", c_id, c_args);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            RunProcessRealtime(start);
            c.SendMessage(":white_check_mark: " + c_id + ".js successfully ended.");
        }

        void RunProcessRealtime(ProcessStartInfo p)
        {
            try
            {
                if (File.Exists(Directory.GetCurrentDirectory() + @"\" + p.Arguments.Split(' ')[0] + ".js"))
                {
                    Process process = Process.Start(p);
                    process.EnableRaisingEvents = true;
                    process.OutputDataReceived += (s, e) =>
                    {
                        c.SendMessage("[" + c_id + "] " + e.Data);
                    };
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                    process.Start();
                }
            }
            catch
            {
            }
        }

        public static async Task<string> Run(string id, string args = "", Channel outputChannel = null)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"C:\Program Files\nodejs\node.exe";
            args = args.Replace(' ', '_');
            start.Arguments = string.Format("{0} {1}", id, args);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            string output = await Task.Run(() => new Node().RunProcessAsync(start));
            return output;
        }
        string RunProcessAsync(ProcessStartInfo p)
        {
            try
            {
                if (File.Exists(Directory.GetCurrentDirectory() + @"\" + p.Arguments.Split(' ')[0] + ".js"))
                {
                    Process process = Process.Start(p);
                    process.Start();
                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd();
                    Console.WriteLine("CLEVERBOT| " + process.StandardOutput.ReadToEnd());
                    Console.WriteLine("CLEVERBOT| " + process.StandardError.ReadToEnd());

                    return output != "" ? output : ":white_check_mark:";
                }
                else
                {
                    Console.WriteLine(Directory.GetCurrentDirectory() + @"\" + p.Arguments.Split(' ')[0] + ".js");
                    return ":no_entry_sign: Node '" + p.Arguments.Split(' ')[0] + ".js'not found.";
                }
            }
            catch (Exception e)
            {
                return ":no_entry_sign: " + e.Message;
            }
        }
    }
}

