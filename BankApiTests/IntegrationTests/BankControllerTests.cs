using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BankApi.Models;
using BankApiTests.Fixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BankApiTests.IntegrationTests;

[TestFixture]
[Category("IntegrationTest")]
public class BankControllerTests
{
    private IntegrationTestFixture _integrationTestFixture = null!;
    private HttpClient _client = null!;
    private WebApplicationFactory<Program> _factory = null!;

    private const string CreateAccount = "api/accounts";
    private const string GetBalance = "api/accounts/{0}/balance";
    private const string GetAccount = "api/accounts/{0}/info";
    private const string GetHistory = "api/accounts/{0}/history";
    private const string Deposit = "api/accounts/deposit";
    private const string Withdraw = "api/accounts/withdraw";
    private const string Transfer = "api/accounts/transfer";

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _integrationTestFixture = new IntegrationTestFixture();
        await _integrationTestFixture.WaitForContainers();

        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [Test]
    public async Task CreateAccount_And_GetBalance_ShouldReturnCorrectAmount()
    {
        _integrationTestFixture.ClearEnvironment();

        var accountId = AccountId.New;
        var account = new Account(accountId.ToString(), "Alice", "Doe", 500m,"DKK");
        var content = GetContent(account);

        var createResponse = await _client.PostAsync(CreateAccount, content);
        createResponse.IsSuccessStatusCode.Should().BeTrue();

        var balanceResponse = await _client.GetAsync(string.Format(GetBalance, account.Id));
        balanceResponse.IsSuccessStatusCode.Should().BeTrue();

        var balanceResponseAsDecimal = JsonSerializer.Deserialize<decimal>(await balanceResponse.Content.ReadAsStringAsync());
        balanceResponseAsDecimal.Should().Be(account.Amount);
    }

    [Test]
    public async Task Deposit_ShouldIncreaseBalance()
    {
        _integrationTestFixture.ClearEnvironment();
        var accountId = AccountId.New;
        var account = new Account(accountId.ToString(),"Bob", "Smith", 100m, "EUR");
        await _client.PostAsync(CreateAccount, GetContent(account));

        var deposit = new Account(accountId.ToString(), account.FirstName, account.LastName, 50m, account.Currency);
        var depositResponse = await _client.PostAsync(Deposit, GetContent(deposit));
        depositResponse.IsSuccessStatusCode.Should().BeTrue();

        var balanceResponse = await _client.GetAsync(string.Format(GetBalance, account.Id));
        balanceResponse.IsSuccessStatusCode.Should().BeTrue();

        var balance = JsonSerializer.Deserialize<decimal>(await balanceResponse.Content.ReadAsStringAsync());
        balance.Should().Be(150m);
    }

    [Test]
    public async Task Withdraw_ShouldDecreaseBalance()
    {
        _integrationTestFixture.ClearEnvironment();
        var accountId = AccountId.New;
        var account = new Account(accountId.ToString(), "Charlie", "Brown", 200m, "USD");
        await _client.PostAsync(CreateAccount, GetContent(account));

        var withdraw = new Account(accountId.ToString(), account.FirstName, account.LastName, 50m, account.Currency);
        var withdrawResponse = await _client.PostAsync(Withdraw, GetContent(withdraw));
        withdrawResponse.IsSuccessStatusCode.Should().BeTrue();

        var balanceResponse = await _client.GetAsync(string.Format(GetBalance, account.Id));
        balanceResponse.IsSuccessStatusCode.Should().BeTrue();

        var balance = JsonSerializer.Deserialize<decimal>(await balanceResponse.Content.ReadAsStringAsync());
        balance.Should().Be(150m);
    }

    [Test]
    public async Task Transfer_ShouldMoveFundsBetweenAccounts()
    {
        _integrationTestFixture.ClearEnvironment();
        var accountIdSource = AccountId.New;
        var accountIdTarget = AccountId.New;

        var source = new Account(accountIdSource.ToString(),"Dan", "Miller", 300m, "USD");
        var target = new Account(accountIdTarget.ToString(), "Eva", "Taylor", 100m, "USD");

        await _client.PostAsync(CreateAccount, GetContent(source));
        await _client.PostAsync(CreateAccount, GetContent(target));

        var transfer = new Transfer(source.Id, target.Id, 50m);
        var transferResponse = await _client.PostAsync(Transfer, GetContent(transfer));
        transferResponse.IsSuccessStatusCode.Should().BeTrue();

        // Check balances
        var sourceBalanceResp = await _client.GetAsync(string.Format(GetBalance, source.Id));
        var targetBalanceResp = await _client.GetAsync(string.Format(GetBalance, target.Id));

        var sourceBalance = JsonSerializer.Deserialize<decimal>(await sourceBalanceResp.Content.ReadAsStringAsync());
        var targetBalance = JsonSerializer.Deserialize<decimal>(await targetBalanceResp.Content.ReadAsStringAsync());

        sourceBalance.Should().Be(250m);
        targetBalance.Should().Be(150m);
    }
    
    [Test]
    public async Task GetHistory_ShouldReturnTransactionList()
    {
        _integrationTestFixture.ClearEnvironment();
        var accountId = AccountId.New;
        var account = new Account(accountId.ToString(), "George", "Hall", 400m, "EUR");
        await _client.PostAsync(CreateAccount, GetContent(account));
        
        await _client.PostAsync(Deposit, GetContent(new Account(account.Id, account.FirstName, account.LastName, 100m, account.Currency)));
        await _client.PostAsync(Withdraw, GetContent(new Account(account.Id, account.FirstName, account.LastName, 50m, account.Currency)));
    
        var response = await _client.GetAsync(string.Format(GetHistory, account.Id));
        response.IsSuccessStatusCode.Should().BeTrue();
    
        // var transactions = await response.Content.ReadFromJsonAsync<List<Transaction>>();
        //
        // var json = await response.Content.ReadAsStringAsync();
    }

    private StringContent GetContent(object data)
    {
        var json = JsonSerializer.Serialize(data);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _client.Dispose();
        _integrationTestFixture.Dispose();
        _factory.Dispose();
    }
}
