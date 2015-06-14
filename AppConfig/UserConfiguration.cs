using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Serialization;
using log4net;

namespace AppConfig
{
    public class UserConfiguration
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UserConfiguration));
        public const string KeyUserConfigPath = "user.config.path";
        private UserConfigurationXml _userConfigurationXml;

        public ConfigurationKey[] ConfigurationKeys
        {
            get { return _userConfigurationXml.ConfigurationKeys; }
            set { _userConfigurationXml.ConfigurationKeys = value; }
        }

        public Library[] Libraries
        {
            get { return _userConfigurationXml.Libraries; }
            set { _userConfigurationXml.Libraries = value; }
        }

        public Emulator[] Emulators
        {
            get { return _userConfigurationXml.Emulators; }
            set { _userConfigurationXml.Emulators = value; }
        }

        public void Load()
        {
            var serializer = new XmlSerializer(typeof(UserConfigurationXml));
            try
            {
                using (var fileStream = new FileStream(ConfigPath, FileMode.Open))
                {
                    _userConfigurationXml = serializer.Deserialize(fileStream) as UserConfigurationXml;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, e);
                _userConfigurationXml = new UserConfigurationXml();
            }
        }

        private string ConfigPath
        {
            get { return Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings[KeyUserConfigPath]); }
        }

        public Library FindLibraryByKey(string key)
        {
            return _userConfigurationXml.Libraries.SingleOrDefault(l => l.Name == key);
        }
    }

    [Serializable]
    [XmlRoot(IsNullable = false, ElementName = "user_configuration")]
    public class UserConfigurationXml
    {
        public const string KeyUserConfigPath = "user.config.path";


        [XmlElement("emulator")]
        public Emulator[] Emulators { get; set; }

        [XmlElement("library")]
        public Library[] Libraries { get; set; }

        [XmlElement("configuration_key")]
        public ConfigurationKey[] ConfigurationKeys { get; set; }
    }

    public class Emulator
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlElement("path")]
        public string Path { get; set; }
        
    }

    public class Library
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlElement("path")]
        public string[] Path { get; set; }
    }

    public class ConfigurationKey
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlText]
        public string Value { get; set; }
    }

    
}