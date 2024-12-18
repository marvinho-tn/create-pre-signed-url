using Amazon.S3;
using FastEndpoints;
using PreSignedUrl.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.Configure<AwsConfig>(builder.Configuration.GetSection("AwsConfig"));
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseFastEndpoints();
app.Run();

