namespace AlterBankApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using AlterBankApi.Application.Commands;
    using AlterBankApi.Application.Requests;
    using AlterBankApi.Application.Responses;
    using Microsoft.Extensions.Logging;
    using AlterBankApi.Extensions;

    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IMediator mediator, ILogger<AccountController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [Route("list")]
        [ProducesResponseType(typeof(List<GetAccountResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<GetAccountResponse>>> GetListOfAccountsAsync()
        {
            return await _mediator.SendWithActionResult(new GetListOfAccountsRequest(), result => Ok(result), result => BadRequest());
        }

        [HttpGet]
        [Route("balance")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(decimal), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<decimal>> GetAccountAsync([FromQuery] string accountNum)
        {
            if (string.IsNullOrEmpty(accountNum))
                return BadRequest();
            
            return await _mediator.SendWithActionResult(new GetAccountRequest(accountNum), result => Ok(result.Balance), result => NotFound());
        }

        [HttpPost("open")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(OpenAccountResponse), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<OpenAccountResponse>> OpenAccount([FromBody] OpenAccountCommand openAccountCommand)
        {
            return await _mediator.SendWithActionResult(openAccountCommand, result => Created(string.Empty, result), result => BadRequest());
        }
    }
}
