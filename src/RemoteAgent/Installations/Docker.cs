using System.Collections.Generic;

namespace RemoteAgent.Installations
{
    public class Docker
    {
        public static List<string> InstallScripts => new List<string>
        {
            "yum update -y",
            "yum remove docker docker-client docker-client-latest docker-common docker-latest docker-latest-logrotate docker-logrotate docker-engine",
            "yum install -y yum-utils",
            "yum-config-manager --add-repo https://download.docker.com/linux/centos/docker-ce.repo",
            "yum install -y docker-ce docker-ce-cli containerd.io",
            "systemctl enable docker",
            "systemctl start docker",
            "docker info"
        };

        public static List<string> RemoveScripts => new List<string>
        {
            "systemctl disable docker",
            "systemctl stop docker",
            "yum remove docker-ce docker-ce-cli containerd.io",
            "rm -rf /var/lib/docker"
        };
    }
}
