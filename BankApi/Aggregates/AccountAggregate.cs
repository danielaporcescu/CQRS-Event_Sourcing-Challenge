using BankApi.Events;
using BankApi.Models;
using EventFlow.Aggregates;
using EventFlow.Aggregates.ExecutionResults;

namespace BankApi.Aggregates;

public class AccountAggregate : AggregateRoot<AccountAggregate, AccountId>, 
    IEmit<AccountCreatedEvent>
{
    public string AccountId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    
    public AccountAggregate(AccountId id)
        : base(id)
    {
    }
    
    public virtual IExecutionResult CreateAccount(string firstName, string lastName, decimal amount, string currency)
    {
        if(string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            return ExecutionResult.Failed("First and Last names are required");

        if (amount < 0)
            return ExecutionResult.Failed("Initial deposit cannot be negative");
        
        var accountCreatedEvent = new AccountCreatedEvent(firstName, lastName, amount, currency);
        Emit(accountCreatedEvent);
        
        return ExecutionResult.Success();
    }

    public virtual IExecutionResult Deposit(TransferId transferId, decimal amount)
    {
        if (amount <= 0)
            return ExecutionResult.Failed("Amount cannot be negative or zero");
        
        Emit(new DepositEvent(transferId, amount));

        return ExecutionResult.Success();
    }
    
    public virtual IExecutionResult Transfer(AccountId toAccountId, TransferId  transferId, decimal amount)
    {
        if (amount <= 0)
            return ExecutionResult.Failed("Amount cannot be negative or zero");
        
        if (Amount < amount)
            return ExecutionResult.Failed("Insufficient funds for transfer operation");
        
        Emit(new TransferEvent(Id, toAccountId, transferId, amount));

        return ExecutionResult.Success();
    }
    
    public virtual IExecutionResult WithdrawEvent(TransferId transferId, decimal amount)
    {
        if (amount <= 0)
            return ExecutionResult.Failed("Amount cannot be negative or zero");
        
        if (Amount - amount < 0)
            return ExecutionResult.Failed("Insufficient funds for withdrawal operation");
        
        Emit(new WithdrawEvent(transferId, amount));

        return ExecutionResult.Success();
    }

    public void Apply(AccountCreatedEvent accountCreatedEvent)
    {
        FirstName = accountCreatedEvent.FirstName;
        LastName = accountCreatedEvent.LastName;
        Amount = accountCreatedEvent.Amount;
        Currency = accountCreatedEvent.Currency;
    }
    
    public void Apply(DepositEvent depositEvent)
    {
        Amount += depositEvent.Amount;
    }
    
    public void Apply(TransferEvent transferEvent)
    {
        Amount -= transferEvent.Amount;
    }
    
    public void Apply(WithdrawEvent withdraw)
    {
        Amount -= withdraw.Amount;
    }
}