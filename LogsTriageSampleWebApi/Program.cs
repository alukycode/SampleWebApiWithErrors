using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Adds the services needed to generate your API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IMapper>(
    _ => new MapperConfiguration(_ => { }, NullLoggerFactory.Instance).CreateMapper()
);
builder.Services.AddSingleton<AsyncPitfallService>();
builder.Services.AddDbContext<IssueDbContext>(
    options => options.UseSqlite(builder.Configuration.GetConnectionString("IssueScenariosDb"))
);
builder.Services.AddIssueScenarioHandlers();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// 👇 Drop your super-simple testing endpoints right here

app.MapGet("/api/test", () => new { Status = "Success", Message = "Vibe check passed." })
    .WithOpenApi()
    .WithSummary("Runs a simple test")
    .WithDescription("Returns a success message to verify the API is running.");

app.MapPost("/api/data", (MyData model) => Results.Ok(new { Message = $"Received: {model.Name}" }))
    .WithOpenApi()
    .WithSummary("Submits data")
    .WithDescription("Accepts a JSON payload and returns a confirmation.");

app.MapGet("/api/error", () => { throw new InvalidOperationException("This is a test exception for Application Insights."); })
    .WithOpenApi()
    .WithSummary("Throws a test exception")
    .WithDescription("Generates an unhandled exception to test error logging in Application Insights.");

app.MapIssueScenarioEndpoints();

app.Run();

// A simple record to represent incoming data
public record MyData(string Name);
