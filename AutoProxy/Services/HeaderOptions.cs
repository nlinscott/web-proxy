using HttpHeaders = Microsoft.Net.Http.Headers;

namespace RequestForwarding.Services
{
    internal sealed class HeaderOptions : IHeaderOptions
    {
        /// <summary>
        /// "content" headers in System.Net.Http must be associated with HttpContent objects, so set these names aside to make sure they arent added
        /// in places they shouldnt be
        /// </summary>
        private static readonly ISet<string> _contentHeaders = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            HttpHeaders.HeaderNames.ContentType,
            HttpHeaders.HeaderNames.ContentDisposition,
            HttpHeaders.HeaderNames.ContentEncoding,
            HttpHeaders.HeaderNames.ContentLanguage,
            HttpHeaders.HeaderNames.ContentLocation,
            HttpHeaders.HeaderNames.ContentMD5,
            HttpHeaders.HeaderNames.ContentRange,
            HttpHeaders.HeaderNames.ContentSecurityPolicy,
            HttpHeaders.HeaderNames.ContentSecurityPolicyReportOnly,
        };

        private static readonly ISet<string> _ignoreHeaders = new HashSet<string>(_contentHeaders, StringComparer.InvariantCultureIgnoreCase)
        {
            HttpHeaders.HeaderNames.Host,
            HttpHeaders.HeaderNames.AcceptEncoding,
            HttpHeaders.HeaderNames.ContentLength,
        };

        /// <summary>
        /// When forwarding a response, ignore these headers
        /// </summary>
        private static readonly ISet<string> _excludeFromResponse = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            HttpHeaders.HeaderNames.TransferEncoding
        };

        public ISet<string> Ignore => _ignoreHeaders;

        public ISet<string> ContentHeaders => _contentHeaders;

        public ISet<string> ExcludeFromResponse => _excludeFromResponse;
    }
}
