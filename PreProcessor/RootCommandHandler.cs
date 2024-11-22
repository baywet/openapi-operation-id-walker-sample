using System.CommandLine;
using System.CommandLine.Invocation;

namespace PreProcessor;

public class RootCommandHandler : ICommandHandler
{
	public required Argument<string> DescriptionSource { get; init; }
	public required Option<string> OutputPath { get; init; }
    public int Invoke(InvocationContext context)
    {
        throw new InvalidOperationException("This handler should not be invoked. Use the async one instead.");
    }

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        throw new NotImplementedException();
    }
}