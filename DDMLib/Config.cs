using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DDMLib
{
    public static class Config
    {
        private static readonly string IniFilePath = "config.ini"; 
        public static string ConnectionString { get; private set; }
        static Config()
        {
            ConnectionString = ReadIniValue("Database", "ConnectionString", "server=localhost;user=root;password=vertrigo;database=pc_store;");
        }
        private static string ReadIniValue(string section, string key, string defaultValue)
        {
            const int bufferSize = 1024;
            StringBuilder buffer = new StringBuilder(bufferSize);
            GetPrivateProfileString(section, key, defaultValue, buffer, bufferSize, IniFilePath);
            return buffer.ToString();
        }
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(
            string section, string key, string defaultValue, StringBuilder retVal, int size, string filePath);
    }
}
