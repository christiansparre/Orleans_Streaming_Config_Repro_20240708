using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans_Streaming_Config_Repro_20240708.Grains;

var builder = Host.CreateDefaultBuilder();

builder.ConfigureAppConfiguration(configurationBuilder => configurationBuilder.AddUserSecrets(typeof(Program).Assembly, optional: true));

builder.UseOrleans((ctx, silo) =>
{
    silo.UseLocalhostClustering();
    silo.Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "Local";
        options.ServiceId = "Orleans_Streaming_Config_Repro_20240708";
    });

    silo.AddEventHubStreams(Constants.StreamingProvider, configurator =>
    {
        configurator.ConfigureEventHub(eventHubOptionsBuilder => eventHubOptionsBuilder.Configure(eventHubOptions =>
        {
            eventHubOptions.ConfigureEventHubConnection(
                connectionString: ctx.Configuration["EventHubConnectionString"],
                eventHubName: ctx.Configuration["EventHubName"],
                consumerGroup: "$Default");
        }));

        configurator.UseAzureTableCheckpointer(checkpointerOptionsBuilder => checkpointerOptionsBuilder.Configure(checkPointerOptions =>
        {
            checkPointerOptions.ConfigureTableServiceClient("UseDevelopmentStorage=true");
        }));

        silo.AddMemoryGrainStorage("PubSubStore");
    });
});

var host = builder.Build();

await host.RunAsync();
