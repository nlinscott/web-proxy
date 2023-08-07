using OpenApiValidator.Model;
using Microsoft.OpenApi.Models;

namespace OpenApiValidator.Services
{
    public interface IOpenApiQueryService
    {
        IClientVerificationResponse GetAllResponses(OpenApiDocument doc, string path);
    }
}
