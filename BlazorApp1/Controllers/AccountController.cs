using BlazorApp1.Database;
using BlazorApp1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp1.Controllers;

[Route("api")]
[ApiController]
[AllowAnonymous]
public class AccountController(AccountRepository accountRepository) : ControllerBase
{
    private AccountRepository _accountRepository = accountRepository;

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateAccount()
    {
        var account = new Account(id: Guid.NewGuid(), firstName: "John", lastName: "Doe",
            email: "john.doe@gmail.com", password: "123456");
        await _accountRepository.AddAccount(account);
        return new OkResult();
    }
    
    [HttpGet]
    [Route("account")]
    public async Task<List<Account>> GetAccount()
    {
        var account = await _accountRepository.GetAccounts();
        return account;
    }
    
    [HttpPost]
    [Route("transfer")]
    public IActionResult TransferMoney()
    {
        return new ViewResult();
    }
    
    [HttpPost]
    [Route("deposit")]
    public async Task<IActionResult> DepositMoney([FromBody] decimal amount)
    {
        var account = new Account(firstName: "John", lastName: "Doe",
            email: "john.doe@gmail.com", balance: 123) {Id = new Guid("85b6ad92-201b-4fe8-87c6-7db589db18a8")};
        
        var balance = await _accountRepository.GetBalance(account);
        var updatedBalance = balance + amount;
        
        await _accountRepository.UpdateBalance(account, updatedBalance);
        return new OkResult();
    }

    [HttpPost]
    [Route("withdraw")]
    public async Task<IActionResult> WithdrawMoney(decimal amount)
    {
        var account = new Account(firstName: "John", lastName: "Doe",
            email: "john.doe@gmail.com", balance: 123) {Id = new Guid("85b6ad92-201b-4fe8-87c6-7db589db18a8")};

        var balance = await _accountRepository.GetBalance(account);
        var updatedBalance = balance - amount;

        await _accountRepository.UpdateBalance(account, updatedBalance);
        return new OkResult();
    }
}