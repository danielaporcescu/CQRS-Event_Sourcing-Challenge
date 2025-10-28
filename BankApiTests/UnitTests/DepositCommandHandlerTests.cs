using BankApi.Aggregates;
using BankApi.Commands;
using BankApi.Events;
using BankApi.Models;
using EventFlow.Aggregates.ExecutionResults;
using FluentAssertions;

namespace BankApiTests.UnitTests
{
    [TestFixture]
    public class DepositCommandHandlerTests
    {
        private DepositCommandHandler _handler = null!;
        private AccountAggregate _aggregate = null!;

        [SetUp]
        public void SetUp()
        {
            _handler = new DepositCommandHandler();
            _aggregate = new AccountAggregate(AccountId.New);
        }

        [Test]
        public async Task ExecuteCommandAsync_Should_Succeed_With_ValidAmount()
        {
            var command = new DepositCommand(
                _aggregate.Id,
                new TransferId($"transfer-{Guid.NewGuid()}"),
                100);

            var result = await _handler.ExecuteCommandAsync(_aggregate, command, CancellationToken.None);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Test]
        public async Task ExecuteCommandAsync_Should_Fail_When_Amount_IsZero()
        {
            var command = new DepositCommand(
                _aggregate.Id,
                new TransferId($"transfer-{Guid.NewGuid()}"),
                0);

            var result = await _handler.ExecuteCommandAsync(_aggregate, command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            (result as FailedExecutionResult)?.Errors.FirstOrDefault().Should().Be("Amount cannot be negative or zero");
        }

        [Test]
        public async Task ExecuteCommandAsync_Should_Fail_When_Amount_IsNegative()
        {
            var command = new DepositCommand(
                _aggregate.Id,
                new TransferId($"transfer-{Guid.NewGuid()}"),
                -50);

            var result = await _handler.ExecuteCommandAsync(_aggregate, command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            (result as FailedExecutionResult)?.Errors.FirstOrDefault().Should().Be("Amount cannot be negative or zero");
        }

        [Test]
        public async Task ExecuteCommandAsync_Should_Handle_Large_Deposit_Amounts()
        {
            const decimal largeAmount = 1_000_000_000m;
            var command = new DepositCommand(
                _aggregate.Id,
                new TransferId($"transfer-{Guid.NewGuid()}"),
                largeAmount);

            var result = await _handler.ExecuteCommandAsync(_aggregate, command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
        }
    }
}
