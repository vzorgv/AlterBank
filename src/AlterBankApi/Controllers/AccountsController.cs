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
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IMediator mediator, ILogger<AccountsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [Route("list")]
        [ProducesResponseType(typeof(IEnumerable<GetAccountResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<GetAccountResponse>>> GetListOfAccountsAsync()
        {
            return await _mediator.SendWithActionResult(new GetListOfAccountsRequest(), result => Ok(result), result => BadRequest());
        }

        [HttpGet]
        [Route("account")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(GetAccountResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<GetAccountResponse>> GetAccountAsync([FromQuery] string accountNum)
        {
            if (string.IsNullOrEmpty(accountNum))
                return BadRequest();

            return await _mediator.SendWithActionResult(new GetAccountRequest(accountNum), result => Ok(result), result => NotFound());
        }

        [HttpPost("open")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(OpenAccountResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OpenAccountResponse>> OpenAccount([FromBody] OpenAccountCommand openAccountCommand)
        {
            return await _mediator.SendWithActionResult(openAccountCommand, result => Ok(result), result => BadRequest());
        }
    }
}
