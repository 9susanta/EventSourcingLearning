using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EventSourcing.Bank.Application.Services;
using EventSourcing.Bank.Application.DTOs;
using EventSourcing.Bank.Application.Abstractions;

namespace EventSourcing.Bank.Api.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class BankAccountsController : ControllerBase
    {
        private readonly IAccountService _service;
        private readonly ILogger<BankAccountsController> _logger;

        public BankAccountsController(IAccountService service, ILogger<BankAccountsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAccountRequest req, CancellationToken cancellationToken)
        {
            try
            {
                var account = await _service.CreateAccountAsync(req.Name, cancellationToken);
                var res = new AccountResponse { Id = account.Id, AccountHolder = account.AccountHolder, Balance = account.Balance };
                return Ok(res);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Request cancelled by client for Create account {AccountHolder}", req?.Name);
                return StatusCode(499);
            }
            catch (ConcurrencyException)
            {
                return Conflict();
            }
        }

        [HttpPost("{accountId}/deposit")]
        public async Task<IActionResult> Deposit(Guid accountId, [FromBody] DepositRequest req, CancellationToken cancellationToken)
        {
            try
            {
                var account = await _service.DepositAsync(accountId, req.Amount, cancellationToken);
                var res = new AccountResponse { Id = account.Id, AccountHolder = account.AccountHolder, Balance = account.Balance };
                return Ok(res);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Request cancelled by client for Deposit {AccountId}", accountId);
                return StatusCode(499);
            }
            catch (ConcurrencyException)
            {
                return Conflict();
            }
        }

        [HttpPost("{accountId}/withdraw")]
        public async Task<IActionResult> Withdraw(Guid accountId, [FromBody] WithdrawRequest req, CancellationToken cancellationToken)
        {
            try
            {
                var account = await _service.WithdrawAsync(accountId, req.Amount, cancellationToken);
                var res = new AccountResponse { Id = account.Id, AccountHolder = account.AccountHolder, Balance = account.Balance };
                return Ok(res);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Request cancelled by client for Withdraw {AccountId}", accountId);
                return StatusCode(499);
            }
            catch (ConcurrencyException)
            {
                return Conflict();
            }
        }

        [HttpGet("{accountId}")]
        public async Task<IActionResult> Get(Guid accountId, CancellationToken cancellationToken)
        {
            try
            {
                var account = await _service.GetAsync(accountId, cancellationToken);
                var res = new AccountResponse { Id = account.Id, AccountHolder = account.AccountHolder, Balance = account.Balance };
                return Ok(res);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Request cancelled by client for Get account {AccountId}", accountId);
                return StatusCode(499);
            }
            catch (ConcurrencyException)
            {
                return Conflict();
            }
        }

        [HttpGet("{accountId}/events")]
        public async Task<IActionResult> GetEvents(Guid accountId, CancellationToken cancellationToken)
        {
            try
            {
                var events = await _service.GetEventsAsync(accountId, cancellationToken);
                return Ok(events);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Request cancelled by client for GetEvents {AccountId}", accountId);
                return StatusCode(499);
            }
            catch (ConcurrencyException)
            {
                return Conflict();
            }
        }
    }
}
