using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;

namespace OpenApiValidator.Services
{
    internal sealed class ValueGenerator : IValueGenerator
    {
        public JProperty FromSchema(string name, OpenApiSchema schema)
        {
            JObject obj = new JObject();

            Type t = schema.MapOpenApiPrimitiveTypeToSimpleType();

            if (IsNumericType(t))
            {
                obj[name] = 1;
            }
            else if (IsBooleanType(t))
            {
                obj[name] = true;
            }
            else if (IsDateType(t))
            {
                obj[name] = DateTimeOffset.UtcNow.ToString();
            }
            else
            {
                obj[name] = t.Name;
            }

            return obj.Properties().FirstOrDefault();
        }

        private static bool IsDateType(Type type)
        {
            if(type.Equals(typeof(DateTimeOffset)))
            {
                return true;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.DateTime:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsBooleanType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsNumericType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}
