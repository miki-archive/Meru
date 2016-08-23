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
            file = new StreamWriter(filePath + ".config");
            file.WriteLine($"# {fileName} created with {IABot.VersionText}");
        }
        public FileWriter(string fileName, string extension)
        {
            extension = extension.TrimStart('.');

            filePath = Directory.GetCurrentDirectory()+ "\\" + fileName;
            file = new StreamWriter(filePath + "." + extension);
            file.WriteLine($"# {fileName} created with {IABot.VersionText}");
        }

        public void Dispose()
        {
            file.Dispose();
        }

        public void Write(string variable, string comment = "")
        {
            if (comment != "") file.WriteLine($"# {comment}");
            file.WriteLine(variable);
        }
        public void WriteComment(string comment)
        {
            file.WriteLine($"# {comment}");
        }

        public void Finish()
        {
            file.Flush();
            file.Close();
            Dispose();
        }
    }
}
