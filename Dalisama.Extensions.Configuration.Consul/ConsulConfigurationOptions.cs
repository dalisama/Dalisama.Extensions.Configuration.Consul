using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Dalisama.Extensions.Configuration.Consul
{
    public class ConsulConfigurationOptions
    {
        public string ApiUri { get; set; }
        public string KeyPath { get; set; }
        public Boolean IgnoreSSL { get; set; } = false;
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public bool ReloadOnChange { get; set; } = true;
        public int ReloadDelay { get; set; } = 5000;
        public Func<Dictionary<string, string>, bool, HttpClient> HttpClientFactory { get; set; } = (headers, ignoreSSL) =>
           {
               var httpClientHandler = new HttpClientHandler
               {
                   AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
               };
               if (ignoreSSL)
               {
                   httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
               }

               var httpClient = new HttpClient(httpClientHandler);

               foreach (var header in headers)
               {
                   httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
               }
               return httpClient;
           };

    }


}
