using Microsoft.Extensions.Configuration;
using System;

namespace Dalisama.Extensions.Configuration.Consul
{
    public static class ConfigurationExtension
    {
        public static IConfigurationBuilder AddApiConfiguration(this IConfigurationBuilder configuration, Action<ConsulConfigurationOptions> options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));
            var apiConfigurationOptions = new ConsulConfigurationOptions();
            options(apiConfigurationOptions);
            configuration.Add(new ConsulConfigurationSource(apiConfigurationOptions));
            return configuration;
        }
    }
}
