using Orleans;

namespace Orleans_Streaming_Config_Repro_20240708.Grains.Interfaces;

public interface IHelloWorldGrain : IGrainWithStringKey
{
    public const string Key = "HelloWorld";

    Task<HelloMessage> Hello(string name);
}

public interface IHelloWorldConsumerGrain : IGrainWithStringKey
{
}