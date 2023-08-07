using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace OpenApiValidator.Model
{
    public interface IOpenApiLoadResult
    {
        bool Success
        {
            get;
        }

        string ErrorMessage
        {
            get;
        }

        OpenApiDocument Document
        {
            get;
        }

        OpenApiDiagnostic Diagnostics
        {
            get;
        }
    }
}
