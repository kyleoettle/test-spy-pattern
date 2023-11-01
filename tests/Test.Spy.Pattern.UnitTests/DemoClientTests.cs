using FakeItEasy;
using Newtonsoft.Json;
using System.Net;
using Test.Spy.Pattern.UnitTests.Spies;

namespace Test.Spy.Pattern.UnitTests
{
    public class DemoClientTests
    {
        private readonly DemoClient sut;
        private readonly SpyLogger<DemoClient> logger;
        private readonly SpyHttpMessageHandler messageHandler;
        private readonly IHttpClientFactory httpClientFactory; 
        
        public DemoClientTests()
        {
            //setting up of spies and fakes so I don't have to do it in every test
            logger = new SpyLogger<DemoClient>();
            messageHandler = new SpyHttpMessageHandler();
            httpClientFactory = A.Fake<IHttpClientFactory>();
            var httpClient = new HttpClient(messageHandler);

            A.CallTo(() => httpClientFactory.CreateClient("DemoClient"))
                .Returns(httpClient);

            sut = new DemoClient(logger, httpClientFactory);
        }


        [Fact(DisplayName = "When getting WorldTimeByIP and the correct value is returned")]
        public async Task GetWorldTime()
        {
            //setup
            var response = new WorldTime() { datetime = DateTime.Now, client_ip = "mockClientIp" };
            var responseMessage = new HttpResponseMessage();
            responseMessage.Content = new StringContent(JsonConvert.SerializeObject(response));
            responseMessage.StatusCode = HttpStatusCode.OK;

            messageHandler._sendAsync = (request) =>
            {
                //assert request properties
                Assert.Equal(HttpMethod.Get, request.Method);
                Assert.Equal("http://worldtimeapi.org/api/ip", request.RequestUri.AbsoluteUri);
                
                //return responseMessage
                return responseMessage;
            };

            //act
            var worldTime = await sut.GetWorldTimeFromIP();

            //assert
            Assert.Equal(response.datetime, worldTime.datetime);
            Assert.Equal(response.client_ip, worldTime.client_ip);
        }

        [Fact(DisplayName = "When getting WorldTimeByIP and a failed status code is returned")]
        public async Task GetWorldTime_fail()
        {
            //setup
            var response = "mock response";
            var responseMessage = new HttpResponseMessage();
            responseMessage.Content = new StringContent(response);
            responseMessage.StatusCode = HttpStatusCode.Conflict;

            messageHandler._sendAsync = (request) =>
            {
                //assert request properties
                Assert.Equal(HttpMethod.Get, request.Method);
                Assert.Equal("http://worldtimeapi.org/api/ip", request.RequestUri.AbsoluteUri);

                //return responseMessage
                return responseMessage;
            };

            var logged = false;
            logger._logInvoked = (logLevel, eventId, state, exception) =>
            {
                logged = true;
                Assert.Equal($"Failed to get WorldTime from IP. {responseMessage.StatusCode}: {response}", state.ToString());
            };

            //act
            var worldTime = await sut.GetWorldTimeFromIP();

            //assert
            Assert.Null(worldTime);
            Assert.True(logged);
        }
    }
}