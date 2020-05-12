using System.Text;
using SuperSocket.ProtoBase;

namespace RemoteCommon
{
    public class CommandLinePipelineFilter : TerminatorPipelineFilter<PackageInfo>
    {
        public CommandLinePipelineFilter()
            : base(Encoding.UTF8.GetBytes(Package.Terminator))
        {

        }
    }
}