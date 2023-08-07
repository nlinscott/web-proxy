using OpenApiValidator.Model;
using OpenApiValidator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OpenApiValidator.Controller
{
    [Route("[controller]")]
    [ApiVersion("1")]
    [ApiController]
    public class OpenAPIController : ControllerBase
    {
        private readonly IOpenApiLoader _loader;
        private readonly IOpenApiQueryService _queryService;
        private readonly IRequestValidator _requestValidator;

        public OpenAPIController(IOpenApiLoader loader, IOpenApiQueryService queryService, IRequestValidator requestValidator)
        {
            _loader = loader;
            _queryService = queryService;
            _requestValidator = requestValidator;
        }

        [HttpPost("/verify")]
        [ProducesResponseType(typeof(ClientVerificationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(VerifyResponseErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(VerifyResponseErrorModel), StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> VerifyRequestAsync([FromBody] VerifyRequestModel requestModel)
        {
            IOpenApiLoadResult result = await _loader.Load(requestModel.OpenAPIUrl);

            if (!result.Success)
            {
                return HandleError(result);
            }

            VerifyResponseErrorModel errors = _requestValidator.Validate(requestModel, result.Document);

            if (errors.Errors.Any())
            {
                return BadRequest(errors);
            }

            IClientVerificationResponse response = _queryService.GetAllResponses(result.Document, requestModel.Path);

            if (response == null)
            {
                VerifyResponseErrorModel model = new VerifyResponseErrorModel();
                model.Errors.Add("Path provided does not match any path defined in OpenApi docuemnt.");
                return BadRequest(model);
            }

            return Ok(response);
        }

        private IActionResult HandleError(IOpenApiLoadResult result)
        {
            VerifyResponseErrorModel errors = new VerifyResponseErrorModel();

            if (result.Diagnostics == null)
            {
                errors.Errors.Add(result.ErrorMessage);

                return NotFound(errors);
            }

            result.Diagnostics.Errors.Action(s =>
            {
                errors.Errors.Add(s.Message);
            });

            result.Diagnostics.Warnings.Action(s =>
            {
                errors.Errors.Add(s.Message);
            });

            return BadRequest(result);
        }
    }
}
