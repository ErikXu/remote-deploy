using System;
using Microsoft.Extensions.Configuration;

namespace RemoteCommon.Connector.Minio
{
    public class MinioOption
    {
        public MinioOption(IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var section = config.GetSection("minio");
            section.Bind(this);
        }

        public string Endpoint { get; set; }

        public string AccessKey { get; set; }

        public string SecretKey { get; set; }

        public bool Secure { get; set; }
    }
}