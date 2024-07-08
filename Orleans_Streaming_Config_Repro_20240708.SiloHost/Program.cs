using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans.Hosting;

var builder = Host.CreateDefaultBuilder();

builder.UseOrleans((ctx, silo) =>
{
    silo.UseLocalhostClustering();
    silo.Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "Local";
        options.ServiceId = "Orleans_Streaming_Config_Repro_20240708";
    });
});

var host = builder.Build();

await host.RunAsync();
