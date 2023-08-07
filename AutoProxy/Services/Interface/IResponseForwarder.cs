using Microsoft.AspNetCore.Http;

namespace RequestForwarding.Services
{
    public interface IResponseForwarder
    {
        Task Forward(HttpResponseMessage toForward, HttpResponse response);
    }
}
