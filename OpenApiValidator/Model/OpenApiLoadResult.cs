using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace OpenApiValidator.Model
{
    internal sealed class OpenApiLoadResult : IOpenApiLoadResult
    {
        public bool Success
        {
            get;
            set;
        } = false;

        public string ErrorMessage
        {
            get;
            set;
        } = string.Empty;

        public OpenApiDocument Document
        {
            get;
            set;
        }

        public OpenApiDiagnostic Diagnostics
        {
            get;
            set;
        }
    }
}
