using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace RequestForwarding.Services
{
    internal sealed class ResponseForwarder : IResponseForwarder
    {
        private readonly IHeaderOptions _headerOptions;

        public ResponseForwarder(IHeaderOptions headerOptions)
        {
            _headerOptions = headerOptions;
        }

        public async Task Forward(HttpResponseMessage toForward, HttpResponse response)
        {
            response.Clear();
            response.Headers.Clear();

            toForward.Headers.Where(h => !_headerOptions.ExcludeFromResponse.Contains(h.Key))
                .Select(h => new KeyValuePair<string, StringValues>(h.Key, h.Value.ToArray()))
                .Action(kvp =>
                {
                    response.Headers.Add(kvp);
                });

            response.StatusCode = (int)toForward.StatusCode;
            response.ContentType = toForward.Content.Headers.ContentType.MediaType;

            await response.StartAsync();

            Stream responseToForward = await toForward.Content.ReadAsStreamAsync();
            await responseToForward.CopyToAsync(response.Body);

            await response.CompleteAsync();
        }
    }
}
