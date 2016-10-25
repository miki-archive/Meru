using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace IA.Rest
{
    class RestClient
    {
        HttpClient client;

        string baseUrl;

        public RestClient(string base_url)
        {
            baseUrl = base_url;
            client = new HttpClient();
            client.BaseAddress = new Uri(base_url);
        }

        public RestClient AddHeader(string name, string value)
        {
            client.DefaultRequestHeaders.Add(name, value);
            return this;
        }

        public RestClient SetAuthorisation(string scheme, string value)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, value);
            return this;
        }

        public RestClient AsJson()
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return this;
        }

        public async Task<RestResponse<string>> PostAsync()
        {
            HttpResponseMessage response = await client.PostAsync("", null);
            RestResponse<string> r = new RestResponse<string>();
            r.Success = response.IsSuccessStatusCode;
            r.Data = await response.Content.ReadAsAsync<string>();
            return r;
        }
        
        public async Task<RestResponse<T>> PostAsAsync<T>()
        {
            HttpResponseMessage response = await client.PostAsync("", null);
            RestResponse<T> r = new RestResponse<T>();
            r.Success = response.IsSuccessStatusCode;
            r.Data = await response.Content.ReadAsAsync<T>();
            return r;
        }
    }
}
