using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace RequestForwarding.Services
{
    internal sealed class RequestCreator : IRequestCreator
    {
        private readonly IHeaderOptions _headerOptions;

        public RequestCreator(IHeaderOptions headerOptions)
        {
            _headerOptions = headerOptions;
        }

        public HttpRequestMessage CreateMessage(HttpRequest req, string forwardUrl)
        {
            HttpRequestMessage message = new HttpRequestMessage();
            message.Method = new HttpMethod(req.Method);

            //exclude content headers since those must go into the Content object below
            foreach (KeyValuePair<string, StringValues> header in req.Headers.Where(kvp => !_headerOptions.Ignore.Contains(kvp.Key)))
            {
                string value = header.Value;

                message.Headers.Add(header.Key, value.AsEnumerable());
            }

            message.Content = new StreamContent(req.Body);

            IEnumerable<string> contentHeadersInRequest = _headerOptions.ContentHeaders.Where(h => req.Headers.ContainsKey(h));

            foreach (string headerKey in contentHeadersInRequest)
            {
                string headerValue = req.Headers[headerKey];
                message.Content.Headers.Add(headerKey, headerValue);
            }

            message.RequestUri = new Uri(forwardUrl);

            return message;
        }
    }
}
