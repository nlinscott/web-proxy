using OpenApiValidator.Model;
using Microsoft.OpenApi.Readers;

namespace OpenApiValidator.Services
{
    internal sealed class OpenApiLoader : IOpenApiLoader
    {
        internal const string OpenApiLoaderClient = "openApiLoaderClient";

        private readonly HttpClient _httpClient;

        public OpenApiLoader(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(OpenApiLoaderClient);
        }

        public async Task<IOpenApiLoadResult> Load(string url)
        {
            OpenApiLoadResult result = new OpenApiLoadResult();
            try
            {
                using (HttpResponseMessage message = await _httpClient.GetAsync(url))
                {
                    OpenApiStreamReader reader = new OpenApiStreamReader();

                    OpenApiDiagnostic diagnostic;

                    result.Document = reader.Read(message.Content.ReadAsStream(), out diagnostic);
                    result.Diagnostics = diagnostic;
                    result.Success = true;
                }
            }
            catch(Exception e)
            {
                result.ErrorMessage = e.Message;
            }

            return result;
        }
    }
}