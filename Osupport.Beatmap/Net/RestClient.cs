using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Osupport.Beatmap.Net
{
    internal sealed class RestClient
    {
        public string BaseAddress { get; set; }

        private readonly HttpClient _client;
        private readonly JsonSerializer _serializer;

        public RestClient()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.ConnectionClose = false;

            _serializer = JsonSerializer.CreateDefault();
        }

        private string ResolveUrl(string url)
        {
            if (string.IsNullOrEmpty(BaseAddress))
                return url;

            return $"{BaseAddress}{url}";
        }

        public Task<T> GetJsonAsync<T>(string url, object query)
        {
            if (query == null)
                return GetJsonAsync<T>(url);

            return GetJsonAsync<T>($"{url}?{query}");
        }

        public async Task<T> GetJsonAsync<T>(string url)
        {
            var response = await _client.GetStreamAsync(ResolveUrl(url));
            using var streamReader = new StreamReader(response);
            using var jsonReader = new JsonTextReader(streamReader);

            return _serializer.Deserialize<T>(jsonReader);
        }
    }
}
