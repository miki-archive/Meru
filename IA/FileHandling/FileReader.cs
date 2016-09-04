using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.FileHandling
{
    public class FileReader : IDisposable
    {
        StreamReader file;

        string filePath;

        public FileReader(string fileName)
        {
            filePath = Directory.GetCurrentDirectory() + "\\" + fileName;
            file = new StreamReader(new FileStream(filePath + ".config", FileMode.OpenOrCreate));
        }
        public FileReader(string fileName, string extension)
        {
            extension = extension.TrimStart('.');

            filePath = Directory.GetCurrentDirectory() + "\\" + fileName;
            file = new StreamReader(new FileStream(filePath + "." + extension, FileMode.OpenOrCreate));
        }

        public void Dispose()
        {
            file.Dispose();
        }

        public static bool FileExist(string fileName)
        {
            return File.Exists(Directory.GetCurrentDirectory() + "\\" + fileName);
        }

        public string ReadLine()
        {
            while (true)
            {
                string currentLine = file.ReadLine();
                if (currentLine == null)
                {
                    Log.WarningAt("filereader", "no data found.");
                    break;
                }

                if (!currentLine.StartsWith("#")) return currentLine;
            }
            return "";
        }

        public void Finish()
        {
            Dispose();
        }
    }
}
