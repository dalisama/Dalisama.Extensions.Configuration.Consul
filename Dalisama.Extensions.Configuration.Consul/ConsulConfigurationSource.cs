using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text;

namespace Dalisama.Extensions.Configuration.Consul
{
    public class ConsulConfigurationSource : IConfigurationSource
    {

        public ConsulConfigurationOptions ApiOption;

        public ConsulConfigurationSource(ConsulConfigurationOptions apiOption)
        {
            ApiOption = apiOption;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ConsulConfigurationProvider(this);
        }
    }


}
