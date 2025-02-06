using Fiap.FileCut.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Mime;

namespace Fiap.FileCut.Infra.Api.Middlewares;

internal class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;
    private readonly Dictionary<Type, int> _statusCodeMappings = new()
        {
            { typeof(EntityNotFoundException), StatusCodes.Status404NotFound },
        };

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (FileCutException ex)
        {
            var status = _statusCodeMappings.GetValueOrDefault(ex.GetType(), StatusCodes.Status400BadRequest);
            await DoReturn(context, ex.Message, status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred");
            await DoReturn(context, ex.Message, StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task DoReturn(HttpContext context, string message, int status)
    {
        var problemDetails = new ProblemDetails { Status = status, Detail = message };
        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = problemDetails.Status.Value;
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}