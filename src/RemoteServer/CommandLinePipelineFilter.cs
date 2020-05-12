using SuperSocket.ProtoBase;

namespace RemoteServer
{
    public class CommandLinePipelineFilter : TerminatorPipelineFilter<PackageInfo>
    {
        public CommandLinePipelineFilter()
            : base(new[] { (byte)'\r', (byte)'\n' })
        {

        }
    }
}