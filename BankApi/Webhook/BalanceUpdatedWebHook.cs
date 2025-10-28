using System.Text;
using System.Text.Json;
using BankApi.Aggregates;
using BankApi.Events;
using BankApi.Models;
using BankApi.Services;
using EventFlow.Aggregates;
using EventFlow.Subscribers;

namespace BankApi.Webhook;

public class BalanceUpdatedWebHook(IHttpClientFactory httpClientFactory) :
    ISubscribeSynchronousTo<AccountAggregate, AccountId, DepositEvent>,
    ISubscribeSynchronousTo<AccountAggregate, AccountId, WithdrawEvent>
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

    public async Task HandleAsync(IDomainEvent<AccountAggregate, AccountId, DepositEvent> domainEvent, CancellationToken cancellationToken)
        => await SendWebHook(domainEvent.AggregateIdentity.Value, cancellationToken);

    public async Task HandleAsync(IDomainEvent<AccountAggregate, AccountId, WithdrawEvent> domainEvent, CancellationToken cancellationToken)
        => await SendWebHook(domainEvent.AggregateIdentity.Value, cancellationToken);

    private async Task SendWebHook(string accountId, CancellationToken cancellationToken)
    {
        var payload = new
        {
            AccountId = accountId,
            Message = "Balance Updated",
            Timestamp = DateTime.UtcNow,
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync("https://localhost:5001/webhook", content, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Webhook] Failed to send for account {accountId}: {ex.Message}");
        }
    }
}
