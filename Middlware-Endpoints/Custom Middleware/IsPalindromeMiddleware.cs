using Middleware_Endpoints.Interfaces;
using System.Text.Json;

namespace Middleware_Endpoints.Custom_Middleware;

public class IsPalindromeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IPalindromeService _palindromeService;
    public IsPalindromeMiddleware(RequestDelegate request, IPalindromeService palindromeService) { _next = request; _palindromeService = palindromeService; }
    public async Task InvokeAsync(HttpContext context)
    {
        string[] pathSegments = context.Request.Path.ToString().Trim('/').Split('/');

        if (pathSegments.FirstOrDefault()?.Equals("palindrome", StringComparison.OrdinalIgnoreCase) == true)
        {
            var results = new List<string>();
            foreach (var segment in pathSegments.Skip(1))
            {
                if (int.TryParse(segment, out int number))
                    results.Add($"Number: {number} , IsPalindrome: {_palindromeService.IsPalindrome(number)}");
            }
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true }));
            return;
        }
        await _next(context);
    }
}