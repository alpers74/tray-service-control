using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TrayServiceControl
{
    public interface IVisibilitySettings
    {
        bool this[string serviceName] { get; set; }
        void Save();
    }

    class Settings : IVisibilitySettings
    {
        private Dictionary<string, bool> _settings = new Dictionary<string, bool>();
        private const string _filename = "ServiceTrayControlConfig.json";
        public Settings()
        {
            try
            {
                var fn = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + _filename;
                if (File.Exists(fn))
                {
                    using (var fs = File.OpenRead(fn))
                    {
                        var sr = new StreamReader(fs);
                        var json = sr.ReadToEnd();
                        var settings = JsonConvert.DeserializeObject<Dictionary<string, bool>>(json);
                        if (settings != null)
                            _settings = settings;
                    }
                }
            }
            catch (Exception)
            {
            }

        }

        public bool this[string key]
        {
            get { return GetServiceVisibility(key); }
            set { SetServiceVisibility(key, value); }
        }

        public void SetServiceVisibility(string name, bool visibility)
        {
            _settings[name] = visibility;
        }

        public bool GetServiceVisibility(string name) 
        {
            return _settings.ContainsKey(name) && _settings[name];
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(_settings);
            var fn = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + _filename;

            using (var fs = File.Create(fn))
            {
                using(var sw = new StreamWriter(fs))
                    sw.Write(json);
            }
        }
    }
}
