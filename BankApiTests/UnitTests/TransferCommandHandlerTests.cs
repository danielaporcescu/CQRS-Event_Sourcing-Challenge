using BankApi.Aggregates;
using BankApi.Commands;
using BankApi.Models;
using EventFlow.Aggregates.ExecutionResults;
using FluentAssertions;

namespace BankApiTests.UnitTests
{
    [TestFixture]
    public class TransferCommandHandlerTests
    {
        private TransferCommandHandler _handler = null!;
        private AccountAggregate _fromAccount = null!;
        private AccountAggregate _toAccount = null!;

        [SetUp]
        public void SetUp()
        {
            _handler = new TransferCommandHandler();
            _fromAccount = new AccountAggregate(AccountId.New);
            _toAccount = new AccountAggregate(AccountId.New);
            _fromAccount.Deposit(new TransferId($"transfer-{Guid.NewGuid()}"), 500);
        }

        [Test]
        public async Task ExecuteCommandAsync_Should_Succeed_With_ValidTransfer()
        {
            var transferId = new TransferId($"transfer-{Guid.NewGuid()}");
            var command = new TransferCommand(transferId, _fromAccount.Id, _toAccount.Id, 100);

            var result = await _handler.ExecuteCommandAsync(_fromAccount, command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue("transfer with valid amount and balance should succeed");
        }

        [Test]
        public async Task ExecuteCommandAsync_Should_Fail_When_Amount_IsZero()
        {
            var command = new TransferCommand(
                new TransferId($"transfer-{Guid.NewGuid()}"),
                _fromAccount.Id,
                _toAccount.Id,
                0);

            var result = await _handler.ExecuteCommandAsync(_fromAccount, command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse("transfer with zero amount should fail");
            (result as FailedExecutionResult)?.Errors.FirstOrDefault().Should().Be("Amount cannot be negative or zero");
        }

        [Test]
        public async Task ExecuteCommandAsync_Should_Fail_When_Amount_IsNegative()
        {
            var command = new TransferCommand(
                new TransferId($"transfer-{Guid.NewGuid()}"),
                _fromAccount.Id,
                _toAccount.Id,
                -50);

            var result = await _handler.ExecuteCommandAsync(_fromAccount, command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            (result as FailedExecutionResult)?.Errors.FirstOrDefault().Should().Be("Amount cannot be negative or zero");
        }

        [Test]
        public async Task ExecuteCommandAsync_Should_Fail_When_InsufficientFunds()
        {
            var command = new TransferCommand(
                new TransferId($"transfer-{Guid.NewGuid()}"),
                _fromAccount.Id,
                _toAccount.Id,
                1000);

            var result = await _handler.ExecuteCommandAsync(_fromAccount, command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse("cannot transfer more than balance");
            (result as FailedExecutionResult)?.Errors.FirstOrDefault().Should().Be("Insufficient funds for transfer operation");
        }

        [Test]
        public async Task ExecuteCommandAsync_Should_Handle_Large_Transfer()
        {
            _fromAccount.Deposit(new TransferId($"transfer-{Guid.NewGuid()}"), 10_000_000);
            var command = new TransferCommand(
                new TransferId($"transfer-{Guid.NewGuid()}"),
                _fromAccount.Id,
                _toAccount.Id,
                1_000_000);

            var result = await _handler.ExecuteCommandAsync(_fromAccount, command, CancellationToken.None);
            (result as FailedExecutionResult)?.Errors.FirstOrDefault().Should()
                .Be("Insufficient funds for withdrawal operation");
        }
    }
}