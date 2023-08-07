using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;

namespace OpenApiValidator.Services
{
    internal sealed class ResponseBuilder : IResponseBuilder
    {
        private readonly IValueGenerator _valueGenerator;

        public ResponseBuilder(IValueGenerator valueGenerator)
        {
            _valueGenerator = valueGenerator;
        }

        public JObject BuildResponseObject(OpenApiSchema schema)
        {
            JObject obj = new JObject();

            foreach (KeyValuePair<string, OpenApiSchema> property in schema.Properties)
            {
                JProperty jsonProperty = BuildFromProperty(property.Key, property.Value);
                obj.Add(jsonProperty.Name, jsonProperty.Value);
            }

            return obj;
        }

        private JObject BuildProperty(string name, OpenApiSchema schema)
        {
            JObject obj = new JObject();

            JProperty prop;

            if (CanUseValue(schema.Example))
            {
                prop = BuildFromExample(name, schema.Example);
            }
            else
            {
                prop = _valueGenerator.FromSchema(name, schema);
            }

            obj.Add(prop.Name, prop.Value);

            return obj;
        }

        private bool CanUseValue(IOpenApiAny any)
        {
            return any != null &&
                any.AnyType == AnyType.Primitive &&
                any is IOpenApiPrimitive;
        }

        private bool IsObject(OpenApiSchema schema)
        {
            return schema.Type == "object" && schema.Properties.Count != 0;
        }

        private bool IsArray(OpenApiSchema schema)
        {
            return schema.Type == "array" && schema.Items != null && schema.Properties.Count == 0;
        }

        private JProperty BuildFromExample(string name, IOpenApiAny example)
        {
            JObject obj = new JObject();

            if (example.AnyType == AnyType.Primitive && example is IOpenApiPrimitive)
            {
                ExampleTextWriter writer = new ExampleTextWriter();

                //second parameter is arbitrary since the writer is just capturing the value of the example
                example.Write(writer, OpenApiSpecVersion.OpenApi3_0);

                obj[name] = JToken.FromObject(writer.Value);
            }

            return obj.Properties().FirstOrDefault();
        }

        private JProperty BuildFromProperty(string name, OpenApiSchema schema)
        {
            JObject jsonObject = new JObject();
            if (IsArray(schema))
            {
                JArray jsonArray = new JArray();

                if (IsObject(schema.Items))
                {
                    jsonArray.Add(BuildResponseObject(schema.Items));
                }
                else
                {
                    JProperty prop = _valueGenerator.FromSchema(string.Empty, schema.Items);

                    jsonArray.Add(prop.Value);
                }

                jsonObject[name] = jsonArray;

            }
            else if (IsObject(schema))
            {
                jsonObject[name] = BuildResponseObject(schema);
            }
            else
            {
                jsonObject = BuildProperty(name, schema);
            }

            return jsonObject.Properties().FirstOrDefault();
        }
    }
}
