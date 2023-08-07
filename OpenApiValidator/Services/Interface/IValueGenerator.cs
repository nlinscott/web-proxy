using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;

namespace OpenApiValidator.Services
{
    internal interface IValueGenerator
    {
        JProperty FromSchema(string name, OpenApiSchema schema);
    }
}
