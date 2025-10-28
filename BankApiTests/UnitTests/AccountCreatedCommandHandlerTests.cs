using BankApi.Aggregates;
using BankApi.Commands;
using BankApi.Models;
using EventFlow.Aggregates.ExecutionResults;
using FluentAssertions;

namespace BankApiTests.UnitTests
{
    [TestFixture]
    public class AccountCreatedCommandHandlerTests
    {
        private AccountCreatedCommandHandler _handler = null!;
        private AccountAggregate _aggregate = null!;

        [SetUp]
        public void SetUp()
        {
            _handler = new AccountCreatedCommandHandler();
            _aggregate = new AccountAggregate(AccountId.New);
        }

        [Test]
        public async Task ExecuteCommandAsync_Should_CreateAccount_With_ValidData()
        {
            var command = new AccountCreatedCommand(
                _aggregate.Id,
                new Account(firstName: "Alice", lastName: "Doe", amount: 500, currency: "USD"));

            var result = await _handler.ExecuteCommandAsync(_aggregate, command, CancellationToken.None);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Test]
        public async Task ExecuteCommandAsync_Should_Fail_When_Balance_IsNegative()
        {
            var command = new AccountCreatedCommand(
                _aggregate.Id,
                new Account(firstName: "Bob", lastName: "Smith", amount: -100, currency: "USD"));

            var result = await _handler.ExecuteCommandAsync(_aggregate, command, CancellationToken.None);

            result.Should().NotBeNull();
            (result as FailedExecutionResult)?.Errors.FirstOrDefault().Should().Be("Initial deposit cannot be negative");
        }

        [Test]
        public async Task ExecuteCommandAsync_Should_Fail_When_FirstName_IsMissing()
        {
            var command = new AccountCreatedCommand(
                _aggregate.Id,
                new Account(firstName: "", lastName: "Smith", amount: 100, currency: "EUR"));

            var result = await _handler.ExecuteCommandAsync(_aggregate, command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            (result as FailedExecutionResult)?.Errors.FirstOrDefault().Should().Be("First and Last names are required");
        }

        [Test]
        public async Task ExecuteCommandAsync_Should_Fail_When_LastName_IsMissing()
        {
            var command = new AccountCreatedCommand(
                _aggregate.Id,
                new Account(firstName: "John", lastName: "", amount: 100, currency: "EUR"));

            var result = await _handler.ExecuteCommandAsync(_aggregate, command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse("last name is required");
        }

        [Test]
        public async Task ExecuteCommandAsync_Should_Handle_Large_Balance()
        {
            var command = new AccountCreatedCommand(
                _aggregate.Id,
                new Account(firstName: "Richie", lastName: "Rich", amount: 1_000_000_000, currency: "USD"));

            var result = await _handler.ExecuteCommandAsync(_aggregate, command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
        }

        [Test]
        public async Task ExecuteCommandAsync_Should_Handle_Multiple_Parallel_Creations()
        {
            var command = new AccountCreatedCommand(
                _aggregate.Id,
                new Account(firstName: "Parallel", lastName: "User", amount: 50, currency: "GBP"));

            var tasks = new[]
            {
                _handler.ExecuteCommandAsync(new AccountAggregate(AccountId.New), command, CancellationToken.None),
                _handler.ExecuteCommandAsync(new AccountAggregate(AccountId.New), command, CancellationToken.None),
                _handler.ExecuteCommandAsync(new AccountAggregate(AccountId.New), command, CancellationToken.None)
            };

            var results = await Task.WhenAll(tasks);

            results.Should().OnlyContain(r => r.IsSuccess, "parallel executions should not conflict");
        }
    }
}
