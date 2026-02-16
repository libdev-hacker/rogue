using System.Web;
using RestSharp;

namespace Rogue.Utils
{
    public class WebClient
    {
        
        public Uri Uri { get; }

        public static readonly string BlankPage = "about:blank";

        private RestClient _client;

        public WebClient(string url)
        {
            try
            {
                this.Uri = new (url);
                _client = new (this.Uri);
            }
            catch (UriFormatException)
            {
                this.Uri = new (WebClient.BlankPage);
                _client = new (this.Uri);
            }
        }

        private RestRequest PrepareRequest(Uri info, Dictionary<string, string>? headers)
        {
            RestRequest req = new (info.AbsolutePath);

            string query = Uri.Query;

            // Adding headers to request
            if (headers is not null)
            {
                req.AddHeaders(headers);
            }

            // Adding URL query
            if (query != "")
            {
                if (query.StartsWith("?")) query = query.TrimStart('?');
                var parsedQuery = HttpUtility.ParseQueryString(query);

                foreach (var param in parsedQuery.AllKeys)
                {
                    if (param is not null)
                    {
                        req.AddQueryParameter(param, parsedQuery[param]);
                    }
                }
            }

            return req;
        }

        public string? GetResource(string path, Dictionary<string, string>? headers) => _client.Get(PrepareRequest(new (this.Uri, path), headers)).Content;
        
        public async Task<string?> GetResourceAsync(string path, Dictionary<string, string>? headers) => (await _client.GetAsync(PrepareRequest(new (this.Uri, path), headers))).Content;
        
        public async Task<string?> PostResource(string path, Dictionary<string, string>? headers) => (await _client.PostAsync(PrepareRequest(new (this.Uri, path), headers))).Content;
        
        public async Task<string?> PutResource(string path, Dictionary<string, string>? headers) => (await _client.PutAsync(PrepareRequest(new (this.Uri, path), headers))).Content;
        
        public async Task<string?> DeleteResource(string path, Dictionary<string, string>? headers) => (await _client.DeleteAsync(PrepareRequest(new (this.Uri, path), headers))).Content;

        public async Task<Stream?> GetFile(string path, Dictionary<string, string>? headers) => (await _client.DownloadStreamAsync(PrepareRequest(new (this.Uri, path), headers)));
    }
}