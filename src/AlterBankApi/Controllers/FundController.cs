namespace AlterBankApi.Controllers
{
    using AlterBankApi.Application.Commands;
    using AlterBankApi.Application.Responses;
    using AlterBankApi.Extensions;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Net;
    using System.Threading.Tasks;

    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class FundController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<FundController> _logger;

        public FundController(IMediator mediator, ILogger<FundController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("transfer")]
        [ProducesResponseType((int)HttpStatusCode.ServiceUnavailable)]
        [ProducesResponseType(typeof(FundTransferResponse), (int)HttpStatusCode.OK)]
        public async Task <ActionResult<FundTransferResponse>> Transfer([FromBody] FundTransferCommand fundTransferCommand)
        {
            return await _mediator.SendWithActionResult(fundTransferCommand, result => Ok(result), result => new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable));
        }
    }
}
