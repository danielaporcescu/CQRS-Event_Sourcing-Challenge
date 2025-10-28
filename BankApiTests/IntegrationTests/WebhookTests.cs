using System.Net;
using BankApi.Aggregates;
using BankApi.Events;
using BankApi.Models;
using BankApi.Webhook;
using EventFlow.Aggregates;
using Moq;
using Moq.Protected;

namespace BankApiTests.IntegrationTests;

public class BalanceUpdatedWebHookTests
{
    [Test]
    public async Task HandleAsync_DepositEvent_SendsWebhook_WithCorrectBody()
    {
        HttpRequestMessage? capturedRequest = null;

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Callback<HttpRequestMessage, CancellationToken>((req, _) =>
            {
                capturedRequest = req;
            })
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var httpClient = new HttpClient(handlerMock.Object);
        var httpClientFactory = Mock.Of<IHttpClientFactory>(f =>
            f.CreateClient(It.IsAny<string>()) == httpClient);
        
        var subscriber = new BalanceUpdatedWebHook(httpClientFactory);

        var depositEvent = new DepositEvent(TransferId.New, 100m);
        var accountId = AccountId.New;

        var domainEventMock = new Mock<IDomainEvent<AccountAggregate, AccountId, DepositEvent>>();
        domainEventMock.SetupGet(e => e.AggregateIdentity).Returns(accountId);
        domainEventMock.SetupGet(e => e.AggregateEvent).Returns(depositEvent);
        
        await subscriber.HandleAsync(domainEventMock.Object, CancellationToken.None);
        
        Assert.That(capturedRequest!.Method, Is.EqualTo(HttpMethod.Post));
        Assert.That(capturedRequest.RequestUri!.ToString(), Is.EqualTo("https://localhost:5001/webhook"));
        
        var body = await capturedRequest.Content!.ReadAsStringAsync();
        Console.WriteLine(body); // helpful for debugging
        
        // {"AccountId":"account-65f091aa-eea8-4983-aa24-a759512dde27","Message":"Balance Updated","Timestamp":"2025-10-28T11:21:19.255133Z"}
        Assert.That(body, Does.Contain(accountId.ToString())); // crude example
    }

}