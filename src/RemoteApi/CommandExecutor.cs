using System.Collections.Generic;
using System.Diagnostics;

namespace RemoteApi
{
    public interface ICommandExecutor
    {
        void AddSSHKey(string ip, string rootUser, string rootPassword);

        void RemoveSSHKey(string ip, string rootUser);

        void Scp(string ip, string rootUser, string source, string target);

        string ExecuteCommandSSH(string ip, string rootUser, string command);

        string ExecuteCommand(string command);
    }

    public class CommandExecutor : ICommandExecutor
    {
        public void AddSSHKey(string ip, string rootUser, string rootPassword)
        {
            var scripts = new List<string>
            {
                $"rm -rf /keys/{ip}/sshkey || true",
                $"mkdir -p /keys/{ip}/sshkey",
                $"ssh-keygen -t rsa -b 4096 -f /keys/{ip}/sshkey/id_rsa -P ''",
                $"sshpass -p {rootPassword} ssh-copy-id -o \"StrictHostKeyChecking = no\" -o \"UserKnownHostsFile=/dev/null\" -i /keys/{ip}/sshkey/id_rsa.pub {rootUser}@{ip}"
            };

            ExecuteCommands(scripts);
        }

        public void RemoveSSHKey(string ip, string rootUser)
        {
            var script = $"ssh -q -o \"StrictHostKeyChecking no\" -o \"UserKnownHostsFile=/dev/null\" -i /keys/{ip}/sshkey/id_rsa \"{rootUser}@{ip}\" \"rm -f .ssh/authorized_keys\"";
            ExecuteCommand(script);
        }

        public void Scp(string ip, string rootUser, string source, string target)
        {
            var script = $"scp -q -o \"StrictHostKeyChecking no\" -o \"UserKnownHostsFile=/dev/null\" -i /keys/{ip}/sshkey/id_rsa -r \"{source}\" \"{rootUser}@{ip}:{target}\"";
            ExecuteCommand(script);
        }

        public string ExecuteCommandSSH(string ip, string rootUser, string command)
        {
            var script = $"ssh -q -o \"StrictHostKeyChecking no\" -o \"UserKnownHostsFile=/dev/null\" -i /keys/{ip}/sshkey/id_rsa \"{rootUser}@{ip}\" \"{command}\"";
            return ExecuteCommand(script);
        }

        private void ExecuteCommands(List<string> commands)
        {
            foreach (var command in commands)
            {
                ExecuteCommand(command);
            }
        }

        public string ExecuteCommand(string command)
        {
            var escapedArgs = command.Replace("\"", "\\\"");
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();

            var message = process.StandardOutput.ReadToEnd();
            message += process.StandardOutput.ReadToEnd();

            return message;
        }
    }
}