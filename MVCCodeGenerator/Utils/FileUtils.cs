using System;
using System.IO;
using System.Text;

namespace MVCCodeGenerator.Utils
{
    public class FileUtils
    {
        public static string ReadStringFromFile(string fileName)
        {
            string line;
            var sbResult = new StringBuilder();

            // Read the file and display it line by line.  
            var file = new StreamReader(fileName);

            while ((line = file.ReadLine()) != null)
            {
                sbResult.AppendLine(line);
            }

            file.Close();

            return sbResult.ToString();
        }

        public static bool CreateFileOrOverwrite(string path, string content)
        {
            if (File.Exists(path))
            {              
                File.Delete(path);
            }

            // Create the file.
            using (FileStream fs = File.Create(path))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(content);
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }
            return true;
        }

        public static bool AppendToFile(string path, string content)
        {
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(content);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(content);
                }
            }

            return true;
        }
    }
}
