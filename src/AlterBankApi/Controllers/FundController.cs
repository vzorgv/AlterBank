namespace AlterBankApi.Controllers
{
    using AlterBankApi.Application.Commands;
    using AlterBankApi.Application.Responses;
    using AlterBankApi.Extensions;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// The Fund controller
    /// </summary>
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class FundController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructs <c>FundController</c> instance
        /// </summary>
        /// <param name="mediator">The mediator insatnce</param>
        public FundController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Transfers amount between two accounts
        /// </summary>
        /// <remarks>
        ///
        ///     POST/transfer
        ///     {
        ///         "accountNumDebit": "1234567890",
        ///         "accountNumCredit": "0987654321",
        ///         "amount": 100.00
        ///     }
        ///
        /// </remarks>
        /// <param name="fundTransferCommand">The transfer command</param>
        /// <returns>The transfer result</returns>
        /// <response code="200">Returns the result of tranfer</response>
        /// <response code="400">Error in request</response>
        /// <response code="404">Error in case if one of the accounts not found</response>
        [HttpPost("transfer")]
        [ProducesResponseType(typeof(FundTransferResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task <ActionResult<FundTransferResponse>> Transfer([FromBody, Required] FundTransferCommand fundTransferCommand)
        {
            return await _mediator.SendWithActionResult(fundTransferCommand,
                result => Ok(result),
                result =>
                {
                    if (result.Description is AccountNotExistDescription)
                        return StatusCode((int)HttpStatusCode.NotFound, result.Description.Message);
                    else
                        return BadRequest(result.Description.Message);
                });
        }
    }
}
