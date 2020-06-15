using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace RemoteCommon.Connector.Minio
{
    public static class MinioServiceCollectionExtensions
    {
        public static IServiceCollection AddMinio(this IServiceCollection services, IConfiguration config)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            services.AddOptions();

            var option = new MinioOption(config);

            var minio = option.Secure ? new MinioClient(option.Endpoint, option.AccessKey, option.SecretKey).WithSSL()
                                      : new MinioClient(option.Endpoint, option.AccessKey, option.SecretKey);

            services.Add(ServiceDescriptor.Singleton(minio));

            return services;
        }
    }
}