using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text;

namespace Dalisama.Extensions.Configuration.Consul
{
    public class ConsulRawConfigurationSource : IConfigurationSource
    {

        public ConsulConfigurationOptions ApiOption;

        public ConsulRawConfigurationSource(ConsulConfigurationOptions apiOption)
        {
            ApiOption = apiOption;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ConsulRawConfigurationProvider(this);
        }
    }


}
