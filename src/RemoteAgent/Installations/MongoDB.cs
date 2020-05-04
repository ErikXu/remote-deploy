using System.Collections.Generic;

namespace RemoteAgent.Installations
{
    public class Mongodb
    {
        public static List<string> InstallScripts => new List<string>
        {
             "yum update -y",
             "cat <<EOF > /etc/yum.repos.d/mongodb-org-4.2.repo \n[mongodb-org-4.2]\nname=MongoDB Repository\nbaseurl=https://repo.mongodb.org/yum/redhat/\\$releasever/mongodb-org/4.2/x86_64/\ngpgcheck=1\nenabled=1\ngpgkey=https://www.mongodb.org/static/pgp/server-4.2.asc\nEOF",
             "yum install -y mongodb-org",
             "sed -i 's/127.0.0.1/0.0.0.0/g' /etc/mongod.conf",
             "systemctl enable mongod",
             "systemctl start mongod"
        };

        public static List<string> RemoveScripts => new List<string>
        {
            "systemctl disable mongod",
            "systemctl stop mongod",
             "yum erase $(rpm -qa | grep mongodb-org) -y",
             "rm -rf /var/log/mongodb",
             "rm -rf /var/lib/mongo"
        };
    }
}
