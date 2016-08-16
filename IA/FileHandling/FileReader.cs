using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.FileHandling
{
    class FileReader : IDisposable
    {
        StreamReader file;

        string filePath;

        public FileReader(string fileName)
        {
            filePath = Directory.GetCurrentDirectory() + "\\" + fileName;
            file = new StreamReader(filePath + ".config");
        }
        public FileReader(string fileName, string extension)
        {
            extension = extension.TrimStart('.');

            filePath = Directory.GetCurrentDirectory() + "\\" + fileName;
            file = new StreamReader(filePath + "." + extension);
        }

        public void Dispose()
        {
            file.Dispose();
        }

        public string ReadLine()
        {
            try
            {
                while (true)
                {
                    string currentLine = file.ReadLine();
                    if (!currentLine.StartsWith("#")) return currentLine;
                }
            }
            catch
            {
                Log.ErrorAt("FileReader.ReadLine", "No more data to load.");
                return "";
            }
        }

        public void Finish()
        {
            file.Close();
            Dispose();
        }
    }
}
