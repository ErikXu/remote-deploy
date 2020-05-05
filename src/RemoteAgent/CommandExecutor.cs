using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RemoteAgent
{
    public interface ICommandExecutor
    {
        void Execute(List<string> scripts);
    }

    public class CommandExecutor: ICommandExecutor
    {
        private readonly IConsole _console;

        public CommandExecutor(IConsole console)
        {
            _console = console;
        }

        public void Execute(List<string> scripts)
        {
            foreach (var script in scripts)
            {
                Execute(script);
            }
        }

        private void Execute(string script)
        {
            _console.WriteLine(script);

            try
            {
                var escapedArgs = script.Replace("\"", "\\\"");
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

                process.OutputDataReceived += (sender, args) => _console.WriteLine(args.Data);
                process.ErrorDataReceived += (sender, args) => _console.WriteLine(args.Data);
                
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                _console.WriteLine(ex.Message);
            }
        }
    }

    public class ComandResult
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }
    }
}
