using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddSingleton<IMapper>(
    _ => new MapperConfiguration(_ => { }, NullLoggerFactory.Instance).CreateMapper()
);
builder.Services.AddDbContext<IssueDbContext>(
    options => options.UseSqlite(builder.Configuration.GetConnectionString("IssueScenariosDb"))
);
builder.Services.AddSampleApplication(builder.Configuration);
builder.Services.AddIssueScenarioHandlers();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapOperationalEndpoints();
app.MapOrderEndpoints();
app.MapIssueScenarioEndpoints();

app.Run();
