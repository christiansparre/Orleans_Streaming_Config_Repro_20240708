using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using Orleans_Streaming_Config_Repro_20240708.Grains.Interfaces;

namespace Orleans_Streaming_Config_Repro_20240708.Grains;

public class HelloWorldGrain : IHelloWorldGrain, IGrainBase
{
    private readonly ILogger<HelloWorldGrain> _logger;

    public HelloWorldGrain(IGrainContext grainContext, ILogger<HelloWorldGrain> logger)
    {
        _logger = logger;
        GrainContext = grainContext;
    }

    public Task<HelloMessage> Hello(string name)
    {
        _logger.LogInformation("Received hello call from {Name}", name);

        var message = $"Hello {name}, {this.GetPrimaryKeyString()} says hi! 👋";

        this.GetStreamProvider(Constants.StreamingProvider)
            .GetStream<string>(StreamId.Create("test-ns", this.GetPrimaryKeyString()))
            .OnNextAsync(message);

        return Task.FromResult(new HelloMessage(message, DateTimeOffset.UtcNow));
    }

    public Task OnActivateAsync(CancellationToken token)
    {
        _logger.LogInformation("Hello grain {GrainId} was activated", GrainContext.GrainId);

        return Task.CompletedTask;
    }

    public IGrainContext GrainContext { get; }
}

[ImplicitStreamSubscription("test-ns")]
public class HelloWorldConsumerGrain : IHelloWorldConsumerGrain, IGrainBase
{
    private readonly ILogger<HelloWorldGrain> _logger;

    public HelloWorldConsumerGrain(IGrainContext grainContext, ILogger<HelloWorldGrain> logger)
    {
        _logger = logger;
        GrainContext = grainContext;
    }

    public async Task OnActivateAsync(CancellationToken token)
    {
        await this.GetStreamProvider(Constants.StreamingProvider)
            .GetStream<string>(StreamId.Create("test-ns", this.GetPrimaryKeyString()))
            .SubscribeAsync((message, t) =>
            {
                _logger.LogInformation("Received streaming message: {Message}", message);

                return Task.CompletedTask;
            });
    }

    public IGrainContext GrainContext { get; }
}

public static class Constants
{
    public const string StreamingProvider = "streaming-provider";
}