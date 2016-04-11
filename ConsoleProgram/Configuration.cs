using Delta.IO;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleProgram
{
    class Configuration
    {
        public class Info
        {
            public string Description { get; set; }
            public int Timeout { get; set; }
            public List<string> KillProcessList { get; set; }
        }

        private static Configuration _instance;

        private Configuration(string configPath)
        {
            ConfigPath = configPath;
            ConfigMap = Parse();
        }

        public static Configuration Instance(string configPath)
        {
            if (_instance == null)
                _instance = new Configuration(configPath);

            return _instance;

        }

        private string ConfigPath { get; set; }
        public Dictionary<string, Info> ConfigMap { get; private set; }
        
        private Dictionary<string, Info> Parse()
        {
            if (!System.IO.File.Exists(ConfigPath))
                return null;

            Ini configIni = new Ini(ConfigPath);
            string [] sectionAry = configIni.GetAllSectionNames();
            Dictionary<string, Info> configMap = new Dictionary<string, Info>();

            for (int i = 0; i < sectionAry.Length; i++)
            {
                Info info = new Info();

                info.Description = configIni.ReadString(sectionAry[i], "Description", "");
                info.Timeout = configIni.ReadInteger(sectionAry[i], "Timeout", 60);

                string temp = configIni.ReadString(sectionAry[i], "KillProcess ", "");
                info.KillProcessList = temp.Trim().Split(',').ToList();

                configMap.Add(sectionAry[i], info);
            }

            return configMap;
        }
    }
}
