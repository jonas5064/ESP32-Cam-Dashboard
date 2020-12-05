using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCamera
{
    class Files
    {

        // Write Data to File
        public static bool write(String data, String path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.AppendAllText(path, data);
                    return true;
                }
                else
                {
                    File.Create(path);
                    File.WriteAllText(path, data);
                    return true;
                } 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // Read Data from File
        public static String read(String path)
        {
            try
            {
                if (File.Exists(path))
                {
                    string s = "";
                    using (StreamReader sr = File.OpenText(path))
                    {
                        while ((s = sr.ReadLine()) != null)
                        {
                            Console.WriteLine(s);
                        }
                    }
                    return s;
                }
                else
                {
                    return "false";
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "false";
            }
        }

    }
}
