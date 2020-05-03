using System.Collections.Generic;

namespace RemoteAgent.Installations
{
    public class AliyunYum
    {
        public static List<string> Scripts => new List<string>
        {
            "yum install wget -y",
            "mkdir -p /etc/yum.repos.d/repo_bak",
            "mv /etc/yum.repos.d/*.repo /etc/yum.repos.d/repo_bak/",
            "wget -O /etc/yum.repos.d/CentOS-Base.repo http://mirrors.aliyun.com/repo/Centos-7.repo",
            "yum clean all",
            "yum makecache",
            "yum update -y"
        };
    }
}
