using BookMyDoctor.Server.Extentions;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

builder.Services.AddAppConfiguration();
builder.Services.AddApiLayer(builder.Configuration);
builder.Services.AddDataLayer(builder.Configuration);
builder.Services.AddApplicationLayer();

var app = builder.Build();

app.UseApiMiddleware();

await app.RunAsync();
