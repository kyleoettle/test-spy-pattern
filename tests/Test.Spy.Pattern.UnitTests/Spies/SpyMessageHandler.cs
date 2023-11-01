namespace Test.Spy.Pattern.UnitTests.Spies
{
    public class SpyHttpMessageHandler : HttpMessageHandler
    {
        internal Func<HttpRequestMessage, HttpResponseMessage> _sendAsync = null;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_sendAsync == null)
                throw new NotImplementedException(nameof(_sendAsync));
            return Task.FromResult(_sendAsync(request));
        }
    }
}
