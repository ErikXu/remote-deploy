using SuperSocket.ProtoBase;

namespace RemoteServer
{
    public class PackageInfo : IKeyedPackageInfo<string>
    {
        public string Key { get; set; }

        public string Content { get; set; }

        public string Raw { get; set; }
    }
}