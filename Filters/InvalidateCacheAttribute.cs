using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.OutputCaching;

namespace MedicalClinicAPI.Filters;

public class InvalidateCacheAttribute : Attribute, IAsyncActionFilter
{
    private readonly string _tag;

    // Cache tag 
    public InvalidateCacheAttribute(string tag)
    {
        _tag = tag;
    }

    // Invalidate cache after action (POST, PUT, DELETE) 
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Execute the action and get the result
        var resultContext = await next();

        // If the response is not successful, don't invalidate the cache
        if (resultContext != null || resultContext?.HttpContext.Response.StatusCode < 200 || resultContext?.HttpContext.Response.StatusCode >= 300)
        {
            return;
        }
        
        // Invalidate the cache for the specified tag
        var cacheStore = context.HttpContext.RequestServices.GetRequiredService<IOutputCacheStore>();
        await cacheStore.EvictByTagAsync(_tag, default);
    }
}

