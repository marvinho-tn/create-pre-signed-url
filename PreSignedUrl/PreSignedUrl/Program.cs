using FastEndpoints;
using PreSignedUrl.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AwsConfig>(builder.Configuration.GetSection("AwsConfig"));
builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseFastEndpoints();
app.Run();

