using System.Collections.Generic;

namespace RemoteAgent.Installations
{
    public class Rabbitmq
    {
        public static List<string> InstallScripts => new List<string>
        {
             "yum update -y",
             "yum install rabbitmq-server -y",
             "systemctl enable rabbitmq-server",
             "systemctl start rabbitmq-server",
             "rabbitmq-plugins enable rabbitmq_management",
             "systemctl restart rabbitmq-server",
             "rabbitmqctl add_user admin admin",
             "rabbitmqctl set_user_tags admin administrator",
             "rabbitmqctl set_permissions -p '/' admin '.*' '.*' '.*'",
             "rabbitmqctl delete_user guest"
        };

        public static List<string> RemoveScripts => new List<string>
        {
            "systemctl disable rabbitmq-server",
             "systemctl stop rabbitmq-server",
            "yum remove -y rabbitmq-server",
            "rm -rf /var/lib/rabbitmq"
        };
    }
}
