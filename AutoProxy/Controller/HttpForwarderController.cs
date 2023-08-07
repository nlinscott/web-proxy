using RequestForwarding.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;

namespace RequestForwarding.Controller
{
    [Route("[controller]")]
    [ApiVersion("1")]
    [ApiController]
    public sealed class HttpForwarderController : ControllerBase
    {
        internal const string HttpForwarderClient = "httpForwarderClient";

        private const string ForwardPayloadToRoute = "/forwardPayload";
        private const string ForwardToRoute = "/forward";

        private readonly HttpClient _httpClient;

        private readonly IResponseForwarder _forwarder;
        private readonly IRequestCreator _requestCreator;

        public HttpForwarderController(IHttpClientFactory httpClientFactory, IResponseForwarder forwarder, IRequestCreator requestCreator)
        {
            _forwarder = forwarder;
            _requestCreator = requestCreator;
            _httpClient = httpClientFactory.CreateClient(HttpForwarderClient);
        }

        [HttpDelete(ForwardToRoute)]
        [HttpHead(ForwardToRoute)]
        [HttpPatch(ForwardToRoute)]
        [HttpPost(ForwardToRoute)]
        [HttpPut(ForwardToRoute)]
        [HttpGet(ForwardToRoute)]
        public Task ForwardRequest([FromQuery] string to)
        {
            return HandleRequest(to);
        }

        /// <summary>
        /// Makes a request to another system by forwarding the request along
        /// </summary>
        /// <returns>The response from the system where the original request was forwarded</returns>
        [HttpDelete(ForwardPayloadToRoute)]
        [HttpHead(ForwardPayloadToRoute)]
        [HttpPatch(ForwardPayloadToRoute)]
        [HttpPost(ForwardPayloadToRoute)]
        [HttpPut(ForwardPayloadToRoute)]
        [HttpGet(ForwardPayloadToRoute)]
        public Task ForwardRequestWithBody([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] object obj, [FromQuery] string to)
        {
            return HandleRequest(to);
        }

        private async Task HandleRequest(string forwardUrl)
        {
            if (string.IsNullOrWhiteSpace(forwardUrl) || !Uri.IsWellFormedUriString(forwardUrl, UriKind.Absolute))
            {
                await WriteNotFoundError($"Unable to forward request. Missing or malformed url to forward.");
                return;
            }

            HttpRequestMessage message = _requestCreator.CreateMessage(HttpContext.Request, forwardUrl);

            HttpResponseMessage resp = await _httpClient.SendAsync(message);

            await _forwarder.Forward(resp, HttpContext.Response);
        }

        private async Task WriteNotFoundError(string message)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;

            await HttpContext.Response.StartAsync();

            await HttpContext.Response.WriteAsync(message);

            await HttpContext.Response.CompleteAsync();
        }
    }
}