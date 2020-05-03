using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RemoteAgent
{
    public class ComandExecutor
    {
        public static void Execute(List<string> scripts)
        {
            foreach (var script in scripts)
            {
                Execute(script);
            }
        }

        private static void Execute(string script)
        {
            Console.WriteLine(script);

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

                process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
                process.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);
                
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class ComandResult
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }
    }
}
