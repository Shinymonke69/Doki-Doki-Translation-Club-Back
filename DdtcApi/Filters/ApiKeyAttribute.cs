using DdtcApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;


namespace DdtcApi.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "Authorization";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Chave inválida ou não fornecida no Header 'Authorization'." });
                return;
            }

            var key = extractedApiKey.ToString();
            if (key.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                key = key.Substring("Bearer ".Length).Trim();
            }

            var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

            var isValidKey = await dbContext.Admins.AnyAsync(a => a.Key == key);

            if (!isValidKey)
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Chave inválida." });
                return;
            }

            await next();
        }
    }
}
