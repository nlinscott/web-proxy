namespace OpenApiValidator.Model
{
    internal sealed class ClientVerificationResponse : IClientVerificationResponse
    {
        public Dictionary<string, string> Responses
        {
            get;
            set;
        } = new Dictionary<string, string>();

        IDictionary<string, string> IClientVerificationResponse.Responses => Responses;
    }
}
