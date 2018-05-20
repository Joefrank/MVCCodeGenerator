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
            //using (var fileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            //{
            //    using (StreamWriter sw = new StreamWriter(path, true))
            //    {
            //        sw.WriteLine(content);
            //    }
            //}
            //return true;


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
    }
}
