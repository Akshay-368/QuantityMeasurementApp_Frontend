// Created with dotnet new web -n QuantityMeasurement.Gateway
// then added the package of dotnet add package Yarp.ReverseProxy for making it the api gateway ( YARP - Yet Another Reverse Proxy )
var builder = WebApplication.CreateBuilder(args);

// Adding the reverse proxy to the service collection
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapReverseProxy();

app.Run();
