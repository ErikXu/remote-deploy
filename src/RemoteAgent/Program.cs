using RemoteAgent.Installations;
using System;
using System.Diagnostics;

namespace RemoteAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            ComandExecutor.Execute(Docker.Scripts);
        }
    }
}
