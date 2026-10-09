using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace apiEcom.Middleware
{
	public class ExceptionHandler : IExceptionHandler
	{
		private readonly ILogger<ExceptionHandler> myLogger;

		public ExceptionHandler(ILogger<ExceptionHandler> logger)
		{
			myLogger = logger;

		}

		public async ValueTask<bool> TryHandleAsync(
			HttpContext httpContext,
			Exception exception,
			CancellationToken cancellationToken)
		{
			var exceptionMessage = exception.Message;
			myLogger.LogError("Error Message: {exceptionMessage}", exceptionMessage);

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "An internal error has occurred.",
                Detail = "Sorry, we are experiencing technical difficulties. Please try again later on."
            };


			httpContext.Response.StatusCode = problemDetails.Status.Value;
			await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

			return true;
        }
	}
}

