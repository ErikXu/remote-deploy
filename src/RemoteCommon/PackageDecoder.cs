using System.Buffers;
using System.Text;
using SuperSocket.ProtoBase;

namespace RemoteCommon
{
    public class PackageDecoder : IPackageDecoder<PackageInfo>
    {
        public PackageInfo Decode(ref ReadOnlySequence<byte> buffer, object context)
        {
            var text = buffer.GetString(new UTF8Encoding(false));

            var index = text.IndexOf(' ');

            var package = new PackageInfo
            {
                Raw = text
            };

            if (index == -1)
            {
                package.Key = text;
            }
            else
            {
                package.Key = text.Substring(0, index);
                package.Content = text.Substring(index + 1, text.Length - index - 1);
            }

            return package;
        }
    }
}