namespace ElProjectGrande.Middleware;

public class CorsMiddleware(RequestDelegate next)
{
    public Task Invoke(HttpContext httpContext)
    {
        httpContext.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        httpContext.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
        httpContext.Response.Headers.Append("Access-Control-Allow-Headers",
            "Content-Type, X-CSRF-Token, X-Requested-With, Accept, Accept-Version, Content-Length, Content-MD5, Date, X-Api-Version, X-File-Name");
        httpContext.Response.Headers.Append("Access-Control-Allow-Methods", "POST,GET,PUT,PATCH,DELETE,OPTIONS");
        return next(httpContext);
    }
}

public static class CorsMiddlewareExtensions
{
    public static IApplicationBuilder UseCorsMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CorsMiddleware>();
    }
}