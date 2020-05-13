using System;
using System.IO;
using System.Text;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace RemoteAgent
{
    public interface IConfigService
    {
        void Set(string serverIp, int serverPort);

        Config Get();
    }

    public class ConfigService: IConfigService
    {
        private readonly IConsole _console;
        private readonly string _directoryName;
        private readonly string _fileName;

        public ConfigService(IConsole console)
        {
            _console = console;
            _directoryName = ".remote-agent";
            _fileName = "config.json";
        }

        public void Set(string serverIp, int serverPort)
        {
            var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), _directoryName);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var config = new Config
            {
                ServerIp = serverIp,
                ServerPort = serverPort
            };

            var filePath = Path.Combine(directory, _fileName);

            using (var outputFile = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                outputFile.WriteLine(JsonConvert.SerializeObject(config, Formatting.Indented));
            }
            _console.WriteLine($"Config saved in {filePath}.");
        }

        public Config Get()
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), _directoryName, _fileName);

            if (File.Exists(filePath))
            {
                var content = File.ReadAllText(filePath);
                try
                {
                    var config = JsonConvert.DeserializeObject<Config>(content);
                    return config;
                }
                catch
                {
                    _console.WriteLine("The config is invalid, please use 'config set' command to reset one.");
                }
            }
            else
            {
                _console.WriteLine("Config is not existed, please use 'config set' command to set one.");
            }

            return null;
        }
    }

    public class Config
    {
        public string ServerIp { get; set; }

        public int ServerPort { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}