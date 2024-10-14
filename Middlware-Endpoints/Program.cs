using Middleware_Endpoints.Custom_Middleware;
using Middleware_Endpoints.Interfaces;
using Middleware_Endpoints.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ISegmentService, SegmentService>();
builder.Services.AddSingleton<IPrimeService, PrimeService>();
builder.Services.AddSingleton<IPalindromeService, PalindromeService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<IsPrimeMiddleware>();
app.UseMiddleware<IsPalindromeMiddleware>();

//--------------------------- Custom Middleware - Get Length And Type Of Each Segments ---------------------------

app.Use(async (context, next) =>
{
    string[] pathSegments = context.Request.Path.ToString().Trim('/').Split('/');
    var segmentService = context.RequestServices.GetRequiredService<ISegmentService>();


    if (pathSegments[0].Equals("info", StringComparison.OrdinalIgnoreCase))
    {
        var html = new StringBuilder(@"<html><head><style>table { width: 50%; border-collapse: collapse; margin: 20px auto; font-family: Arial, sans-serif; font-size: 18px; }th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }th { background-color: #f2f2f2; font-weight: bold; }tr:nth-child(even) { background-color: #f9f9f9; }tr:hover { background-color: #e2e2e2; }caption { margin-bottom: 20px; font-size: 24px; font-weight: bold; }</style></head><body><table><caption>Path Segments Information</caption><thead><tr><th>Segment</th><th>Length</th><th>Type</th></tr></thead><tbody>");

        foreach (string segment in pathSegments.Skip(1))
        {
            html.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>",
                segment, segment.Length, segmentService.GetSegmentType(segment));
        }

        html.Append("</tbody></table></body></html>");

        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync(html.ToString());
        return;
    }
    await next();
});


// Minimal APIs
//--------------------------- Minimal API - Store Prime Numbers In a List ---------------------------

List<int> primeNumbers = new List<int>();

app.MapGet("/saveprime", () =>
{
    if (primeNumbers.Count is 0) return Results.NoContent();
    return Results.Ok(new { message = "List Of Prime Numbers", numbers = primeNumbers });
});

app.MapPost("/saveprime/{number:int}", (int number, IPrimeService service) =>
{
    if (service.IsPrime(number))
    {
        primeNumbers.Add(number);
        return Results.Ok($"{number} Is a Prime Number - Added To List");
    }
    else { return Results.BadRequest($"{number} Is Not a Prime Number"); }
});

//--------------------------- Minimal API - Store Palindrome Numbers In a List ---------------------------

List<int> palindromeNumbers = new List<int>();

app.MapGet("/savepalindrome", () =>
{
    if (palindromeNumbers.Count is 0) return Results.NoContent();
    return Results.Ok(new { message = "List Of Palindrome Numbers", numbers = palindromeNumbers });
});

app.MapPost("/savepalindrome/{number:int}", (int number, IPalindromeService service) =>
{
    if (service.IsPalindrome(number))
    {
        palindromeNumbers.Add(number);
        return Results.Ok($"{number} Is a Palindrome Number - Added To List");
    }
    else { return Results.BadRequest($"{number} Is Not a Palindrome Number"); }
});

//--------------------------- Minimal API - Store Both Prime & Palindrome Numbers In a List ---------------------------

List<int> primePalindromeNumbers = new List<int>();

app.MapGet("/savepp", (IPrimeService primeService, IPalindromeService palindromeService) =>
{
    if (primePalindromeNumbers.Count is 0) return Results.NoContent();
    var primeNumbers = primePalindromeNumbers.Where(primeService.IsPrime).ToList();
    var palindromeNumbers = primePalindromeNumbers.Where(palindromeService.IsPalindrome).ToList();
    return Results.Ok(new
    {
        message = "List of Prime & Palindrome Numbers",
        primeNumbers,
        palindromeNumbers
    });
});

app.MapPost("/savepp/{number:int}", (int number, IPalindromeService palindromeService, IPrimeService primeService) =>
{
    switch ((palindromeService.IsPalindrome(number), primeService.IsPrime(number)))
    {
        case (true, true):
            primePalindromeNumbers.Add(number);
            return Results.Ok($"{number} Is Both a Prime Number And a Palindrome Number - Added To List.");

        case (true, _):
            primePalindromeNumbers.Add(number);
            return Results.Ok($"{number} Is a Palindrome Number - Added To List.");

        case (_, true):
            primePalindromeNumbers.Add(number);
            return Results.Ok($"{number} Is a Prime Number - Added To List.");

        default:
            return Results.BadRequest($"{number} Is Neither a Prime Nor a Palindrome Number.");
    }
});


app.Run();