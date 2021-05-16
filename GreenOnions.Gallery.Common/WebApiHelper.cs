using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GreenOnions.Gallery.Common
{
    public static class WebApiHelper
    {
        public static async Task<string> InvokeApiGetAsync(string url, IDictionary<string, string> headerParams = null)
        {
            using HttpClient httpClient = new();
            HttpRequestMessage message = new();

            if (headerParams != null)
            {
                foreach (KeyValuePair<string, string> header in headerParams)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
            message.Method = HttpMethod.Get;
            message.RequestUri = new Uri(url);
            var response = await httpClient.SendAsync(message);
            string responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }

        public static async Task<string> InvokeApiPostAsync(string url, object bodyParams, IDictionary<string, string> headers = null)
        {
            using HttpClient httpClient = new();
            HttpRequestMessage message = new();

            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    if (!httpClient.DefaultRequestHeaders.Contains(header.Key))
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
            message.Method = HttpMethod.Post;
            message.RequestUri = new Uri(url);

            StringContent strcontent = new(JsonConvert.SerializeObject(bodyParams), Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(url, strcontent).Result;

            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }
    }
}
