using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.FileHandling
{
    public class FileWriter : IDisposable
    {
        string filePath;

        StreamWriter file;

        public FileWriter(string fileName)
        {
            filePath = Directory.GetCurrentDirectory() + "\\" + fileName;
            file = new StreamWriter(new FileStream(filePath + ".config", FileMode.Create));
            file.WriteLine($"# {fileName} created with {Bot.VersionText}");
        }
        public FileWriter(string fileName, string extension)
        {
            extension = extension.TrimStart('.');

            filePath = Directory.GetCurrentDirectory()+ "\\" + fileName;
            file = new StreamWriter(new FileStream(filePath + "." + extension, FileMode.Create));
            file.WriteLine($"# {fileName} created with {Bot.VersionText}");
        }

        public void Dispose()
        {
            file.Dispose();
        }

        public void Write(string variable)
        {
            file.WriteLine(variable);
            file.Flush();
        }
        public void Write(string variable, string comment)
        {
            file.WriteLine($"# {comment}");
            file.WriteLine(variable);
            file.Flush();
        }
        public void WriteComment(string comment)
        {
            file.WriteLine($"# {comment}");
            file.Flush();
        }

        public void Finish()
        {
            file.Flush();
            Dispose();
        }
    }
}
