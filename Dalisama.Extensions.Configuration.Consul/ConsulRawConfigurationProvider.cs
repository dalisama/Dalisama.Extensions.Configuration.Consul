using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Dalisama.Extensions.Configuration.Consul
{
    class ConsulRawConfigurationProvider : ConfigurationProvider
    {
        public ConsulRawConfigurationSource Source { get; }

        public ConsulRawConfigurationProvider(ConsulRawConfigurationSource source)
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
                var tmp = tokens.Children().First().Value<string>("Value");
                var valuetmp = Convert.FromBase64String(tmp);
                var value = Encoding.UTF8.GetString(valuetmp);
                var jtoken = (JToken)JsonConvert.DeserializeObject(value);
                var dictionary = new Dictionary<string, string>();
                GetJTokens(jtoken, dictionary);
                foreach (var tk in dictionary)
                {
                    Set(tk.Key, tk.Value);
                }
            }

        }


        private void GetJTokens(JToken jToken, Dictionary<string, string> dico)
        {

            if (jToken.Count() > 0)
            {

                foreach (var token in jToken.Children())
                {
                    GetJTokens(token, dico);
                }
            }
            else
            {
                dico.Add(jToken.Path.Replace(".", ":"), jToken.Value<string>());
            }

        }
    }
}
