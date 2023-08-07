using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace OpenApiValidator.Model
{
    public sealed class VerifyRequestModel
    {
        [Required]
        public string OpenAPIUrl
        {
            get;
            set;
        }

        [Required]
        public string Path
        {
            get;
            set;
        }

        public JsonElement Body
        {
            get;
            set;
        }
    }
}
