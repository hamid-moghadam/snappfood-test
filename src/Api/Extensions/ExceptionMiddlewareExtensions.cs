using System.Net;
using System.Text.Json;
using Api.Helpers;
using Microsoft.AspNetCore.Diagnostics;

namespace Api.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    if (contextFeature.Error is BaseDomainException baseDomainException)
                        await context.Response.WriteAsync(
                            new ErrorDetails(baseDomainException.Message, baseDomainException.Code).ToString());
                    else
                        await context.Response.WriteAsync(
                            new ErrorDetails(contextFeature.Error.Message).ToString());
                }
            });
        });
    }

    public record ErrorDetails(string Message, int Code = 0)
    {
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}