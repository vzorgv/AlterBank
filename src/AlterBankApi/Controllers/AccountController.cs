namespace AlterBankApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using AlterBankApi.Application.Commands;
    using AlterBankApi.Application.Requests;
    using AlterBankApi.Extensions;
    using AlterBankApi.Application.Model;

    /// <summary>
    /// The Account controller
    /// </summary>
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructs <c>AccountController</c> instance
        /// </summary>
        /// <param name="mediator">The mediator</param>
        public AccountController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Gets list of all accounts
        /// </summary>
        /// <remarks>
        /// GET /list
        /// </remarks>
        /// <returns>List of accounts</returns>
        /// <response code="200">List of accounts</response>
        /// <response code="404">Bad request</response>
        [HttpGet]
        [Route("list")]
        [ProducesResponseType(typeof(IEnumerable<Account>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<Account>>> GetListOfAccountsAsync()
        {
            return await _mediator.SendWithActionResult(new GetListOfAccountsRequest(),
                result => Ok(result),
                result => StatusCode((int)HttpStatusCode.BadRequest));
        }

        /// <summary>
        /// Gets the balance
        /// </summary>
        /// <remarks>
        /// 
        ///     GET /1234567890/balance
        /// 
        /// </remarks>
        /// <param name="accountNum">The account number</param>
        /// <return>Account balance</return>
        /// <response code="200">Returns balance amount</response>
        /// <response code="404">If the account not found</response>
        [HttpGet]
        [Route("{accountNum}/balance")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(decimal), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<decimal>> GetBalanceAsync([FromRoute, Required] string accountNum)
        {
             return await _mediator.SendWithActionResult(new GetAccountRequestById(accountNum),
                result => Ok(result.Balance),
                result => NotFound(result.Description.Message));
        }

        /// <summary>
        /// Creates an account
        /// </summary>
        /// <remarks>
        ///
        ///     POST/create
        ///     {
        ///         "accountNum": "1234567890",
        ///         "balance": 100.00
        ///     }
        ///
        /// </remarks>
        /// <param name="account">The account</param>
        /// <returns>New created Account</returns>
        /// <response code="201">Returns the newly created Account</response>
        /// <response code="400">If account exists</response>
        [HttpPost("create")]
        [ProducesResponseType(typeof(Account), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Account>> CreateAsync([FromBody, Required] Account account)
        {
            return await _mediator.SendWithActionResult(new CreateAccountCommand(account),
                result => Created(string.Empty, result),
                result => BadRequest(result.Description.Message));
        }
    }
}
