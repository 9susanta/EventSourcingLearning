using Microsoft.AspNetCore.Mvc;

namespace EventSourcing.Bank.Api.Controllers
{
    using EventSourcing.Bank.Api.Domain;
    using EventSourcing.Bank.Api.Services;
    using EventSourcing.Bank.Api.Store;
    using global::EventSourcing.Bank.Api.Domain;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Principal;

    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly AccountService _service;

        public AccountsController(AccountService service)
        {
            _service = service;
        }

        // Called by HTTP client to create a new account
        [HttpPost]
        public IActionResult Create(string name)
        {
            return Ok(
        _service.CreateAccount(name));
        }

        // Called by HTTP client to deposit: load -> command -> save
        [HttpPost("{accountId}/deposit")]
        public IActionResult Deposit(Guid accountId, decimal amount)
        {
            return Ok(_service.Deposit(accountId, amount));
        }

        // Called by HTTP client to withdraw: load -> command -> save
        [HttpPost("{accountId}/withdraw")]
        public IActionResult Withdraw(Guid accountId, decimal amount)
        {
            return Ok(
        _service.Withdraw(accountId, amount));
        }

        // Called by HTTP client to query current account state
        [HttpGet("{accountId}")]
        public IActionResult Get(Guid accountId)
        {
            return Ok(_service.Get(accountId));
        }
    }
}
