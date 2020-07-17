namespace AlterBankApi.Extensions
{
    using System;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using AlterBankApi.Application.Responses;

    internal static class MediatRExtension
    {
        public static async Task<ActionResult> SendWithActionResult<TResponse>(this IMediator mediator, 
            IRequest<ExecutionResult<TResponse>> request,
            Func<TResponse, ActionResult> successResponseHandler,
            Func<ExecutionResult<TResponse>, ActionResult> errorResponseHandler)
        {
            var result = await mediator.Send(request);

            if (result.IsError)
                return errorResponseHandler(result);
            else
                return successResponseHandler(result.Data);
        }
    }
}
