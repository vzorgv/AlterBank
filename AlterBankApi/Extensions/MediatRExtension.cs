namespace AlterBankApi.Extensions
{
    using System;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;

    internal static class MediatRExtension
    {
        public static async Task<ActionResult<TResponse>> SendWithActionResult<TResponse>(this IMediator mediator, 
            IRequest<TResponse> request,
            Func<TResponse, ActionResult> successResponseHandler,
            Func<TResponse, ActionResult> nullResponseHandler)
        {
            var result = await mediator.Send(request);

            if (result == null)
                return nullResponseHandler(result);
            else
                return successResponseHandler(result);
        }
    }
}
