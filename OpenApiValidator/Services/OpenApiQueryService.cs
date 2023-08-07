using OpenApiValidator.Model;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Mime;

namespace OpenApiValidator.Services
{
    internal sealed class OpenApiQueryService : IOpenApiQueryService
    {
        private readonly IResponseBuilder _responseBuilder;

        public OpenApiQueryService(IResponseBuilder responseBuilder)
        {
            _responseBuilder = responseBuilder;
        }

        public IClientVerificationResponse GetAllResponses(OpenApiDocument doc, string path)
        {
            KeyValuePair<string, OpenApiPathItem> pathItem = doc.Paths.FirstOrDefault(p => 
                string.Equals(p.Key, path, StringComparison.OrdinalIgnoreCase));

            if (pathItem.Equals(default(KeyValuePair<string, OpenApiPathItem>)))
            {
                //not found
                return null;
            }

            ClientVerificationResponse clientResponse = new ClientVerificationResponse();

            foreach (OpenApiOperation operation in pathItem.Value.Operations.Values)
            {
                foreach (KeyValuePair<string, OpenApiResponse> responses in operation.Responses)
                {
                    string httpStatus = responses.Key;

                    OpenApiResponse response = responses.Value;

                    foreach (KeyValuePair<string, OpenApiMediaType> contentTypes in response.Content
                        .Where(kvp => kvp.Key.Equals(MediaTypeNames.Application.Json)))
                    {
                        OpenApiMediaType mediaType = contentTypes.Value;

                        JObject result = _responseBuilder.BuildResponseObject(mediaType.Schema);

                        clientResponse.Responses.Add(httpStatus, result.ToString(Formatting.None));
                    }
                }
            }

            return clientResponse;
        }

    }
}
