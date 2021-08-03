using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Fance.TurnoFixer.Storage.Interfaces;

namespace Fance.TurnoFixer.Storage
{
    public class OracleObjectStorage : IObjectStorage
    {
        private readonly string _baseUri;

        public OracleObjectStorage()
        {
            _baseUri = Environment.GetEnvironmentVariable("ORACLE_STORAGE_BASE_URI");
        }

        public async Task PutObjectAsync(byte[] objectToStore, string fileName)
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("PUT"), _baseUri + fileName))
                {
                    request.Content = new ByteArrayContent(objectToStore);
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded"); 

                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                }
            }
        }

        public async Task<byte[]> GetObjectAsync(string fileName)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(_baseUri + fileName);
                return await response.Content.ReadAsByteArrayAsync();
            }
        }

        public Task<string> GetObjectLocationAsync(string fileName)
        {
            return Task.FromResult(_baseUri + fileName);
        }
    }
}