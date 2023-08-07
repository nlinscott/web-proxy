using OpenApiValidator.Model;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace OpenApiValidator.Services
{
    internal sealed class RequestValidator : IRequestValidator
    {
        public VerifyResponseErrorModel Validate(VerifyRequestModel model, OpenApiDocument document)
        {
            VerifyResponseErrorModel errorModel = new VerifyResponseErrorModel();

            IEnumerable<OpenApiPathItem> paths = document.Paths.Where(p => string.Equals(p.Key, model.Path))
                .Select(kvp => kvp.Value).ToList();

            if (!paths.Any())
            {
                errorModel.Add($"No matching paths found for: {model.Path}");
                return errorModel;
            }

            IEnumerable<OpenApiMediaType> contentTypes = paths.SelectMany(p => p.Operations.Values)
                .SelectMany(o => o.RequestBody.Content.Values);

            errorModel.SchemaName = document.Info.Title;

            foreach (OpenApiMediaType contentType in contentTypes)
            {
                OpenApiSchema schema = contentType.Schema;

                if(model.Body.ValueKind == JsonValueKind.Array)
                {
                    CheckArrayItem(errorModel, schema, model.Body, "body");
                }
                else
                {
                    IEnumerable<JsonProperty> properties = model.Body.EnumerateObject();

                    GetSchemaAndCheckProperties(errorModel, properties, schema);
                }
            }

            return errorModel;
        }

        private void CheckAllProperties(VerifyResponseErrorModel errorModel, OpenApiSchema schema, JsonProperty property)
        {
            JsonElement jsonElement = property.Value;

            if(jsonElement.ValueKind == JsonValueKind.Object)
            {
                CheckObjectProperties(errorModel, schema, property);
            }
            else if(jsonElement.ValueKind == JsonValueKind.Array)
            {
                CheckArrayProperty(errorModel, schema, property);
            }
            else
            {
                CheckSingleProperty(errorModel, schema, property);
            }
        }

        private void CheckObjectProperties(VerifyResponseErrorModel errorModel, OpenApiSchema objectSchema, JsonProperty property)
        {
            JsonElement jsonElement = property.Value;

            if (objectSchema.Properties == null || 
                !objectSchema.Properties.Any() || 
                !string.Equals(objectSchema.Type, "object"))
            {
                errorModel.Add($"Expected {objectSchema.Type} type for property '{property.Name}' but recieved an object");
                return;
            }

            IDictionary<string, JsonProperty> properties = jsonElement.EnumerateObject().ToDictionary(k => k.Name, v => v);

            foreach (string required in objectSchema.Required)
            {
                if (!properties.ContainsKey(required))
                {
                    errorModel.Add($"Invalid request. Missing required property '{required}'");
                }
            }

            GetSchemaAndCheckProperties(errorModel, properties.Values, objectSchema);
        }

        private void GetSchemaAndCheckProperties(VerifyResponseErrorModel errorModel, IEnumerable<JsonProperty> properties, OpenApiSchema objectSchema)
        {
            foreach (JsonProperty prop in properties)
            {
                OpenApiSchema propSchema;
                if (!TryGetSchema(errorModel, objectSchema, prop.Name, out propSchema))
                {
                    continue;
                }

                CheckAllProperties(errorModel, propSchema, prop);
            }
        }

        private bool TryGetSchema(VerifyResponseErrorModel errorModel, OpenApiSchema objectSchema, string propertyName, out OpenApiSchema propSchema)
        {
            if (!objectSchema.Properties.TryGetValue(propertyName, out propSchema))
            {
                propSchema = null;
                errorModel.Add($"Property '{propertyName}' was provided in the request body but is not part of the schema.");
                return false;
            }

            return true;
        }

        private void CheckSingleProperty(VerifyResponseErrorModel errorModel, OpenApiSchema schema, JsonProperty property)
        {
            CheckSingleProperty(errorModel, schema, property.Value, property.Name);
        }

        private void CheckSingleProperty(VerifyResponseErrorModel errorModel, OpenApiSchema schema, JsonElement jsonElement, string name)
        {
            if (!DoTypesMatch(jsonElement.ValueKind, schema.Type))
            {
                errorModel.Add($"Value for '{name}' does not match required type: {schema.Type}");
            }

            if (jsonElement.ValueKind == JsonValueKind.String && schema.Pattern != null)
            {
                if (!Regex.IsMatch(jsonElement.GetString(), schema.Pattern))
                {
                    errorModel.Add($"Value for '{name}' does not match required pattern: {schema.Pattern}");
                }
            }
        }

        private void CheckArrayProperty(VerifyResponseErrorModel errorModel, OpenApiSchema schema, JsonProperty property)
        {
            JsonElement jsonElement = property.Value;

            if(schema.Items == null || 
               !string.Equals(schema.Type, "array"))
            {
                errorModel.Add($"Expected {schema.Type} type for property '{property.Name}' but recieved an array");
                return;
            }

            int length = jsonElement.GetArrayLength();
            if (length > schema.MaxItems || length < schema.MinItems)
            {
                errorModel.Add($"Array '{property.Name}' is too long with {length} items. Length must be between {schema.MinItems} and {schema.MaxItems}.");
            }

            CheckArrayItem(errorModel, schema, jsonElement, property.Name);
        }

        private void CheckArrayItem(VerifyResponseErrorModel errorModel, OpenApiSchema schema, JsonElement jsonElement, string name)
        {
            foreach (JsonElement item in jsonElement.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Object)
                {
                    GetSchemaAndCheckProperties(errorModel, item.EnumerateObject(), schema.Items);
                }
                else if (item.ValueKind == JsonValueKind.Array)
                {
                    CheckArrayItem(errorModel, schema.Items, item, name);
                }
                else
                {
                    CheckSingleProperty(errorModel, schema.Items, item, name);
                }
            }
        }

        private bool DoTypesMatch(JsonValueKind propertyType, string schemaType)
        {
            if(propertyType == JsonValueKind.String)
            {
                return string.Equals(schemaType, "string", StringComparison.InvariantCultureIgnoreCase);
            }

            if(propertyType == JsonValueKind.Number)
            {
                return string.Equals(schemaType, "number", StringComparison.InvariantCultureIgnoreCase) ||
                       string.Equals(schemaType, "integer", StringComparison.InvariantCultureIgnoreCase);
            }

            if (propertyType == JsonValueKind.True || propertyType == JsonValueKind.False)
            {
                return string.Equals(schemaType, "boolean", StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }
    }
}
