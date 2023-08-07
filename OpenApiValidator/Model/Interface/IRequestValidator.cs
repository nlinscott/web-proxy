using Microsoft.OpenApi.Models;

namespace OpenApiValidator.Model
{
    public interface IRequestValidator
    {
        VerifyResponseErrorModel Validate(VerifyRequestModel model, OpenApiDocument document);
    }
}
