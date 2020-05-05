using System.Collections.Generic;
using System.IO;

namespace RemoteDeploy
{
    public interface IShellGenerator
    {
        GeneratorResult GenerateDockerShell(string ip);

        GeneratorResult GenerateRabbitShell(string ip, string rabbitUser, string rabbitPassword, string vhost = "/");
    }

    public class ShellGenerator : IShellGenerator
    {
        public GeneratorResult GenerateDockerShell(string ip)
        {
            var result = new GeneratorResult
            {
                FileName = "docker-install.sh"
            };

            var scripts = new List<string>
            {
                "sudo yum remove docker docker-client docker-client-latest docker-common docker-latest docker-latest-logrotate docker-logrotate docker-engine",
                "sudo yum install -y yum-utils",
                "sudo yum-config-manager --add-repo https://download.docker.com/linux/centos/docker-ce.repo",
                "sudo yum install -y docker-ce docker-ce-cli containerd.io",
                "sudo systemctl start docker",
                "docker info"
            };

            result.FilePath = SaveShell(ip, result.FileName, scripts);
            return result;
        }

        public GeneratorResult GenerateRabbitShell(string ip, string rabbitUser, string rabbitPassword, string vhost = "/")
        {
            var result = new GeneratorResult
            {
                FileName = "rabbit-install.sh"
            };

            var scripts = new List<string>
            {
                "yum install rabbitmq-server -y",
                "systemctl enable rabbitmq-server",
                "systemctl start rabbitmq-server",
                "rabbitmq-plugins enable rabbitmq_management",
                "systemctl restart rabbitmq-server",
                $"rabbitmqctl add_user {rabbitUser} {rabbitPassword}",
                $"rabbitmqctl set_user_tags {rabbitUser} administrator",
                $"rabbitmqctl set_permissions -p '{vhost}' {rabbitUser} '.*' '.*' '.*'",
                "rabbitmqctl delete_user guest"
            };

            result.FilePath = SaveShell(ip, result.FileName, scripts);
            return result;
        }

        private string SaveShell(string ip, string fileName, List<string> scripts)
        {
            var directory = Path.Combine("/shells", ip);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var filePath = Path.Combine(directory, fileName);
            File.WriteAllText(filePath, string.Join("\r\n", scripts));
            return filePath;
        }
    }

    public class GeneratorResult
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }
    }
}