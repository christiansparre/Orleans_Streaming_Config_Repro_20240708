using Orleans;

namespace Orleans_Streaming_Config_Repro_20240708.Grains.Interfaces;

[GenerateSerializer]
public record HelloMessage([property: Id(0)] string Message, [property: Id(1)] DateTimeOffset Timestamp);