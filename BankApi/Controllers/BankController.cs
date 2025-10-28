using BankApi.Models;
using BankApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers;

[Route("api")]
[ApiController]
public class BankController(CommandService commandService, QueryService queryService) : ControllerBase
{
    [HttpPost]
    [Route("accounts")]
    public async Task<ActionResult> CreateAccount([FromBody] Account account)
    {
        await commandService.CreateAccount(account);
        return Ok();
    }

    [HttpPost]
    [Route("accounts/transfer")]
    public async Task<OkResult> Transfer([FromBody] Transfer transfer)
    {
        await commandService.Transfer(transfer.SourceAccountId, transfer.TargetAccountId, transfer.Amount, CancellationToken.None);
        return Ok();
    }

    [HttpPost]
    [Route("accounts/deposit")]
    public async Task<OkResult> Deposit([FromBody] Account account)
    {
        await commandService.Deposit(account);
        return Ok();
    }

    [HttpPost]
    [Route("accounts/withdraw")]
    public async Task<OkResult> Withdraw([FromBody] Account account)
    {
        await commandService.Withdraw(account);
        return Ok();
    }

    [HttpGet]
    [Route("accounts/{id}/info")]
    public async Task<OkObjectResult> GetAccount([FromRoute] string id)
    {
        var accountInfo = await queryService.GetAccount(id);
        return Ok(accountInfo);
    }

    [HttpGet]
    [Route("accounts/{id}/balance")]
    public async Task<OkObjectResult> GetBalance([FromRoute] string id)
    {
        var balance = await queryService.GetBalance(id);
        return Ok(balance);
    }

    [HttpGet]
    [Route("accounts/{id}/history")]
    public async Task<OkObjectResult> GetHistory([FromRoute] string id)
    {
        var accountInfo = await queryService.GetTransactionHistoryAsync(new AccountId(id), CancellationToken.None);
        return Ok(accountInfo);
    }
}