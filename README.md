# System.Threading.Channels Learnings
A place to practice [`System.Threading.Channels`](https://docs.microsoft.com/en-us/dotnet/api/system.threading.channels).

## Key Points
These are key points for _my_ understanding, not for understanding `System.Threading.Channels` in general.

### Reading
We can use the nice properties given to us with `IAsyncEnumerable<T>` to create a `foreach` that waits on new data coming in. An example of this is seen in `Basic.Run3()` and used through the `Workflows.cs` file:
```csharp
await foreach (string item in _channelReader.ReadAllAsync())
{
	Console.WriteLine(item);
}
```
The above seems to be preferred pattern because we can also write infinite loops. See `Basic.cs` for examples of loops.

### Passing data from one object to another
The pattern here seems to be construct the object with a reader, writer, or both. This is seen in `Workflows.cs`, where each class gets given the reader that the previous object in the workflow writes to, and a writer to pass information to the next object in the workflow. The class that creates these workflow steps is the one that determines who gets which channel's reader and writer.
```csharp
public class WorkflowStep1
{
	private readonly ChannelReader<string> _channelReader;
	private readonly ChannelWriter<string> _channelWriter;
	public WorkflowStep1(ChannelReader<string> channelReader, ChannelWriter<string> channelWriter)
	{
		_channelReader = channelReader;
		_channelWriter = channelWriter;
	}

	public async Task Run()
	{
		await foreach (string item in _channelReader.ReadAllAsync())
		{
			Console.WriteLine($"In {GetType().Name}, processing {item}");
			await _channelWriter.WriteAsync($"{item}i");
		}

		_channelWriter.Complete();
	}
}
```

## Completing a channel
Completing a channel will break the control flow out of the `ReadAllAsync()` call and continue into the rest of the function. I used this in `Workflows.cs` to cascade completions down the workflow. Not sure if this is a good pattern, but it was neat that it worked.

## Resources
- [An Introduction to System.Threading.Channels via devblogs](https://devblogs.microsoft.com/dotnet/an-introduction-to-system-threading-channels/) 
- [An Introduction to System.Threading.Channels via Steve Gordon](https://www.stevejgordon.co.uk/an-introduction-to-system-threading-channels)
- [Use System.IO.Pipelines and System.Threading.Channels APIs to Boost Performance](https://itnext.io/use-system-io-pipelines-and-system-threading-channels-apis-to-boost-performance-832d7ab7c719)
- [C# Channels - Publish / Subscribe Workflows](https://deniskyashif.com/2019/12/08/csharp-channels-part-1/)
- [Async Expert course video](https://academy.dotnetos.org/courses/take/async-expert-en/lessons/13606646-channels)
- [Using Channels In C# .NET – Part 1 – Getting Started](https://dotnetcoretutorials.com/2020/11/24/using-channels-in-net-core-part-1-getting-started/)
- [Example Producer/Consumer Patterns](https://github.com/dotnet/corefxlab/blob/31d98a89d2e38f786303bf1e9f8ba4cf5b203b0f/src/System.Threading.Tasks.Channels/README.md#example-producerconsumer-patterns)