using Microsoft.OpenApi.Writers;
using SharpYaml.Tokens;
using System.Text;

namespace OpenApiValidator.Services
{
    internal sealed class ExampleTextWriter : IOpenApiWriter
    {
        public object Value
        {
            get;
            private set;
        }

        public void Flush()
        {
        }

        public void WriteEndArray()
        {
        }

        public void WriteEndObject()
        {
        }

        public void WriteNull()
        {
            Value = null;
        }

        public void WritePropertyName(string name)
        {
        }

        public void WriteRaw(string value)
        {
            Value = value;
        }

        public void WriteStartArray()
        {
        }

        public void WriteStartObject()
        {
        }

        public void WriteValue(string value)
        {
            Value = value;
        }

        public void WriteValue(decimal value)
        {
            if((value % 1) == 0)
            {
                Value = (long)value;
            }
            else
            {
                Value = value;
            }
        }

        public void WriteValue(int value)
        {
            Value = value;
        }

        public void WriteValue(bool value)
        {
            Value = value;
        }

        public void WriteValue(object value)
        {
        }
    }
}
