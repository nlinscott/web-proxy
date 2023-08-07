using OpenApiValidator.Model;

namespace OpenApiValidator.Services
{
    public interface IOpenApiLoader
    {
        Task<IOpenApiLoadResult> Load(string url);
    }
}
