using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Timers;

namespace Dalisama.Extensions.Configuration.Consul
{
    class ConsulConfigurationProvider : ConfigurationProvider
    {
        public ConsulConfigurationSource Source { get; }

        public ConsulConfigurationProvider(ConsulConfigurationSource source)
        {
            Source = source;
            if (source.ApiOption.ReloadOnChange)
            {
                var aTimer = new Timer(source.ApiOption.ReloadDelay);
                aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                aTimer.Start();
            }
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            Load();
        }

        public override void Load()
        {

            using (var client = Source.ApiOption.HttpClientFactory.Invoke(Source.ApiOption.Headers, Source.ApiOption.IgnoreSSL))
            {
                var result = client.GetAsync($"{ Source.ApiOption.ApiUri}{Source.ApiOption.KeyPath}?recurse=true").GetAwaiter().GetResult();
                result.EnsureSuccessStatusCode();

                Data.Clear();
                var tokens = JToken.Parse(result.Content.ReadAsStringAsync().Result);


                foreach (var tk in tokens.Children())
                {
                    if (!string.IsNullOrEmpty(tk.Value<string>("Value")))
                    {
                        Set(
                                       tk.Value<string>("Key").Replace($"{Source.ApiOption.KeyPath}/", string.Empty).Replace("/", ":")
                                       , Encoding.UTF8.GetString(Convert.FromBase64String(tk.Value<string>("Value")))
                                       );
                    }
                }
            }

        }
    }
}
