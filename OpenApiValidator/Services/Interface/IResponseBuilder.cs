using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;

namespace OpenApiValidator.Services
{
    internal interface IResponseBuilder
    {
        JObject BuildResponseObject(OpenApiSchema schema);
    }
}
