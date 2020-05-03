using System.Collections.Generic;

namespace RemoteAgent.Installations
{
    public class Docker
    {
        public static List<string> Scripts => new List<string>
        {
            "yum update -y",
            "yum remove docker docker-client docker-client-latest docker-common docker-latest docker-latest-logrotate docker-logrotate docker-engine",
            "yum install -y yum-utils",
            "yum-config-manager --add-repo https://download.docker.com/linux/centos/docker-ce.repo",
            "yum install -y docker-ce docker-ce-cli containerd.io",
            "systemctl start docker",
            "docker info"
        };
    }
}
