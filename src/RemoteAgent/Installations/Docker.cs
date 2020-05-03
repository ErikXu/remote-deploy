using System.Collections.Generic;

namespace RemoteAgent.Installations
{
    public class Docker
    {
        public static List<string> Scripts => new List<string>
        {
            "sudo yum update -y",
            "sudo yum remove docker docker-client docker-client-latest docker-common docker-latest docker-latest-logrotate docker-logrotate docker-engine",
            "sudo yum install -y yum-utils",
            "sudo yum-config-manager --add-repo https://download.docker.com/linux/centos/docker-ce.repo",
            "sudo yum install -y docker-ce docker-ce-cli containerd.io",
            "sudo systemctl start docker",
            "docker info"
        };
    }
}
