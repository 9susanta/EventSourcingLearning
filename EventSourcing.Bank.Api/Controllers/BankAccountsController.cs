using Microsoft.AspNetCore.Mvc;
using EventSourcing.Bank.Application.DTOs;
using EventSourcing.Bank.Application.Abstractions;
using EventSourcing.Bank.Application.CQRS.Commands.Account.Handlers;
using EventSourcing.Bank.Application.CQRS.Queries.Account.Handlers;

namespace EventSourcing.Bank.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly MediatR.IMediator _mediator;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(MediatR.IMediator mediator, ILogger<AccountsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAccountRequest req, CancellationToken cancellationToken)
        {
            try
            {
                var command = new CreateAccountCommand{ AccountHolder = req.Name };
                var account = await _mediator.Send(command, cancellationToken);
                var res = new AccountResponse { Id = account.Id, AccountHolder = account.AccountHolder, Balance = account.Balance.Amount };
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

        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetById(Guid accountId, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetAccountQuery{ AccountId = accountId };
                var account = await _mediator.Send(query, cancellationToken);
                return Ok(account);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Request cancelled by client for GetById account {AccountId}", accountId);
                return StatusCode(499);
            }
        }

        [HttpGet("{accountId}/transactions")]
        public async Task<IActionResult> GetTransactionHistory(Guid accountId, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetAccountHistoryQuery{ AccountId = accountId };
                var transactions = await _mediator.Send(query, cancellationToken);
                return Ok(transactions);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Request cancelled by client for GetTransactionHistory {AccountId}", accountId);
                return StatusCode(499);
            }
        }

        [HttpPost("{accountId}/deposit")]
        public async Task<IActionResult> Deposit(Guid accountId, [FromBody] DepositRequest req, CancellationToken cancellationToken)
        {
            try
            {
                var command = new DepositCommand{ AccountId = accountId, Amount = req.Amount };
                var account = await _mediator.Send(command, cancellationToken);
                var res = new AccountResponse { Id = account.Id, AccountHolder = account.AccountHolder, Balance = account.Balance.Amount };
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
                var command = new WithdrawCommand{ AccountId = accountId, Amount = req.Amount };
                var account = await _mediator.Send(command, cancellationToken);
                var res = new AccountResponse { Id = account.Id, AccountHolder = account.AccountHolder, Balance = account.Balance.Amount };
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
        [HttpPost("{accountId}/transfer")]
        public async Task<IActionResult> Transfer(Guid accountId, [FromBody] TransferRequest req, CancellationToken cancellationToken)
        {
            try
            {
                var command = new EventSourcing.Bank.Application.CQRS.Commands.Account.Handlers.TransferCommand 
                { 
                    SourceAccountId = accountId, 
                    DestinationAccountId = req.DestinationAccountId, 
                    Amount = req.Amount 
                };
                
                await _mediator.Send(command, cancellationToken);
                return Ok(new { Message = "Transfer successful" });
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Request cancelled by client for Transfer {AccountId}", accountId);
                return StatusCode(499);
            }
            catch (ConcurrencyException)
            {
                return Conflict();
            }
        }
    }
}
