using Microsoft.AspNetCore.Http;

namespace RequestForwarding.Services
{
    public interface IRequestCreator
    {
        HttpRequestMessage CreateMessage(HttpRequest req, string forwardUrl);
    }
}
