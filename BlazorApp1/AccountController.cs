using BlazorApp1.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp1;

[Route("api/account/[action]")]
[ApiController]
public class AccountController : ControllerBase
{
    [HttpPost("create")]
    public ActionResult CreateAccount([FromBody] Account account)
    {
        
        return new ViewResult();
    }
    
    public ActionResult TransferMoney()
    {
        return new ViewResult();
    }
    
    public ActionResult DepositMoney()
    {
        return new ViewResult();
    }
    
    public ActionResult WithdrawMoney()
    {
        return new ViewResult();
    }
}