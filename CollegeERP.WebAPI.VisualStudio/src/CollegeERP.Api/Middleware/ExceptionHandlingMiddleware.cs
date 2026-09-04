using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace CollegeERP.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "Database update failed");
            await Write(context, HttpStatusCode.Conflict, "The operation conflicts with existing data or database constraints.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled API error");
            await Write(context, HttpStatusCode.InternalServerError, "An unexpected server error occurred.");
        }
    }

    private static async Task Write(HttpContext context, HttpStatusCode status, string message)
    {
        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { message }));
    }
}
