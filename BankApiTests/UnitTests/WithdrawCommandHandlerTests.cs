using BankApi.Aggregates;
using BankApi.Commands;
using BankApi.Events;
using BankApi.Models;
using EventFlow.Aggregates.ExecutionResults;
using FluentAssertions;

namespace BankApiTests.UnitTests
{
    [TestFixture]
    public class WithdrawCommandHandlerTests
    {
        private WithdrawCommandHandler _handler = null!;
        private AccountAggregate _aggregate = null!;

        [SetUp]
        public void SetUp()
        {
            _handler = new WithdrawCommandHandler();
            _aggregate = new AccountAggregate(AccountId.New);
            _aggregate.Deposit(new TransferId($"transfer-{Guid.NewGuid()}"), 500);
        }

        [Test]
        public async Task ExecuteCommandAsync_Should_Succeed_With_ValidAmount()
        {
            var transferId = new TransferId($"transfer-{Guid.NewGuid()}");
            var command = new WithdrawCommand(_aggregate.Id, transferId, 100m);

            var result = await _handler.ExecuteCommandAsync(_aggregate, command, CancellationToken.None);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue("valid withdrawal within balance should succeed");
        }

        [Test]
        public async Task ExecuteCommandAsync_Should_Fail_When_Amount_IsZero()
        {
            var command = new WithdrawCommand(
                _aggregate.Id,
                new TransferId($"transfer-{Guid.NewGuid()}"),
                0m);

            var result = await _handler.ExecuteCommandAsync(_aggregate, command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            (result as FailedExecutionResult)?.Errors.FirstOrDefault().Should().Be("Amount cannot be negative or zero");
        }

        [Test]
        public async Task ExecuteCommandAsync_Should_Fail_When_Amount_IsNegative()
        {
            var command = new WithdrawCommand(
                _aggregate.Id,
                new TransferId($"transfer-{Guid.NewGuid()}"),
                -50m);

            var result = await _handler.ExecuteCommandAsync(_aggregate, command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            (result as FailedExecutionResult)?.Errors.FirstOrDefault().Should().Be("Amount cannot be negative or zero");
        }

        [Test]
        public async Task ExecuteCommandAsync_Should_Fail_When_InsufficientFunds()
        {
            var command = new WithdrawCommand(
                _aggregate.Id,
                new TransferId($"transfer-{Guid.NewGuid()}"),
                1000m);

            var result = await _handler.ExecuteCommandAsync(_aggregate, command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            (result as FailedExecutionResult)?.Errors.FirstOrDefault().Should().Be("Insufficient funds for withdrawal operation");
        }

        [Test]
        public async Task ExecuteCommandAsync_Should_Handle_Large_Valid_Withdrawal()
        {
            _aggregate.Deposit(new TransferId($"transfer-{Guid.NewGuid()}"), 1_000_000m);
            var command = new WithdrawCommand(_aggregate.Id, new TransferId($"transfer-{Guid.NewGuid()}"), 999_999m);

            var result = await _handler.ExecuteCommandAsync(_aggregate, command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue("large valid withdrawal should succeed");
        }
    }
}
