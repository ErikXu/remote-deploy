using SuperSocket.ProtoBase;

namespace RemoteCommon
{
    public class CommandLinePipelineFilter : TerminatorPipelineFilter<PackageInfo>
    {
        public CommandLinePipelineFilter()
            : base(new[] { (byte)'\r', (byte)'\n' })
        {

        }
    }
}