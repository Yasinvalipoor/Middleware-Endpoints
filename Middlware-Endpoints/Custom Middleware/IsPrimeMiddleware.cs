using Middleware_Endpoints.Interfaces;
using System.Text;

namespace Middleware_Endpoints.Custom_Middleware;

public class IsPrimeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IPrimeService _primeService;
    public IsPrimeMiddleware(RequestDelegate next, IPrimeService primeService) { _next = next; _primeService = primeService; }
    public async Task InvokeAsync(HttpContext context)
    {
        string[] pathSegments = context.Request.Path.ToString().Trim('/').Split('/');

        if (pathSegments.Length > 0 && pathSegments[0].Equals("prime", StringComparison.OrdinalIgnoreCase))
        {
            var htmlBuilder = new StringBuilder($@"<html><head><title>Prime Number Check</title><style>body {{font-family: Arial, sans-serif;margin: 40px;}}.title {{text-align: center;margin-bottom: 20px;}}.result-container {{margin: auto;width: 50%;}}table {{width: 100%;border-collapse: collapse;}}th, td {{border: 1px solid #ccc;padding: 10px;text-align: center;}}.prime {{background-color: #e0f7e0; color: green;}}.not-prime {{background-color: #ffe0e0;color: red;}}.not-number {{background-color: #f0f0f0; color: gray;}}</style></head><body><h1 class='title'>Prime Number Check</h1><div class='result-container'><table><tr><th>Number</th><th>Status</th></tr>");

            foreach (var segment in pathSegments.Skip(1))
            {
                if (int.TryParse(segment, out int number))
                {
                    bool isPrime = _primeService.IsPrime(number);
                    string status = isPrime ? "prime" : "not prime";
                    string cssClass = isPrime ? "prime" : "not-prime";

                    htmlBuilder.Append($@"
                        <tr class='{cssClass}'>
                            <td>{number}</td>
                            <td>{status}</td>
                        </tr>");
                }
                else
                {
                    htmlBuilder.Append($@"
                        <tr class='not-number'>
                            <td>{segment}</td>
                            <td>not a number</td>
                        </tr>");
                }
            }

            htmlBuilder.Append(@"</table></div></body></html>");

            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync(htmlBuilder.ToString());
            return;
        }
        await _next(context);
    }
}