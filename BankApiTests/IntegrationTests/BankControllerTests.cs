using System.Text;
using System.Text.Json;
using BankApi.Models;
using BankApiTests.Fixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using RestSharp;

namespace BankApiTests.IntegrationTests;

[TestFixture]
[Category("IntegrationTest")]
public class BankControllerTests
{
    private IntegrationTestFixture _integrationTestFixture = null!;
    private HttpClient _client = null!;
    private WebApplicationFactory<Program> _factory = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _integrationTestFixture = new IntegrationTestFixture();
        // _integrationTestFixture.ClearEnvironment();

        await _integrationTestFixture.WaitForContainers();
        
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }
    
    [Test]
    public async Task Test()
    {
        var requestUri = "api/accounts";
        var account = new Account(firstName: "Alice", lastName: "Doe", amount: 500, currency: "DKK");
        var json = JsonSerializer.Serialize(account);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var restResponse = await _client.PostAsync(requestUri, content);
        restResponse.IsSuccessStatusCode.Should().BeTrue();
        
        var accountDetails = await _client.GetAsync($"accounts/{account.AccountId}/balance");
        accountDetails.IsSuccessStatusCode.Should().BeTrue();
        
        accountDetails.Content.ReadAsStringAsync().Result.Should().BeEquivalentTo(json);        
        // JToken actualJsonToken= JToken.Parse(restResponse.Content);
        // JToken expectedJsonToken= JToken.Parse("[]");
            
        // actualJsonToken.Should().BeEquivalentTo(expectedJsonToken);
    }
    
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _client.Dispose();
        _integrationTestFixture.Dispose();
        _factory.Dispose();
    }
}