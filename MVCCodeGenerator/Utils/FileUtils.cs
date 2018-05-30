using MVCCodeGenerator.Model;
using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace MVCCodeGenerator.Utils
{
    public class FileUtils
    {
        private static string _logFilePath = ConfigurationManager.AppSettings["logpath"] + "\\" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";

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

        public static CreateFileResult CreateFileOrOverwrite(string path, string content)
        {
            var result = new CreateFileResult
            {
                FileExisted = File.Exists(path),
                Success = true
            };

            try
            {
                if (result.FileExisted)
                {
                    File.Delete(path);
                }
                else
                {
                    //let's make sure directory exist first
                    var dir = new FileInfo(path).Directory.FullName;
                    Directory.CreateDirectory(dir);
                }

                // Create the file.
                using (FileStream fs = File.Create(path))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes(content);
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                AppendToFile(_logFilePath, ex.Message);
            }

            return result;
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

        public static bool AppendToLogFile(string content)
        {
            return AppendToFile(_logFilePath, content);
        }
    }
}
