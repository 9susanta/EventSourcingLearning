using Microsoft.AspNetCore.Mvc;

namespace EventSourcing.Bank.Api.Controllers
{
    using EventSourcing.Bank.Api.Domain;
    using global::EventSourcing.Bank.Api.Domain;
    using Microsoft.AspNetCore.Mvc;


    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private static BankAccount? _account;

        [HttpPost]
        public IActionResult Create(string name)
        {
            _account = BankAccount.Create(name);

            return Ok(_account);
        }

        [HttpPost("deposit")]
        public IActionResult Deposit(decimal amount)
        {
            _account!.Deposit(amount);

            return Ok(_account);
        }

        [HttpPost("withdraw")]
        public IActionResult Withdraw(decimal amount)
        {
            _account!.Withdraw(amount);

            return Ok(_account);
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_account);
        }
    }
}
