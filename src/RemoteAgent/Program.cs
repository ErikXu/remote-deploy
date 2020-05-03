using RemoteAgent.Installations;

namespace RemoteAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            ComandExecutor.Execute(Mongodb.Scripts);
        }
    }
}
