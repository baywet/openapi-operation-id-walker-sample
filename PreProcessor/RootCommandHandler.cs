using System.CommandLine;
using System.CommandLine.Invocation;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Writers;

namespace PreProcessor;

public class RootCommandHandler : ICommandHandler
{
	public required Option<string> DescriptionSource { get; init; }
	public required Option<string> OutputPathOption { get; init; }
	public int Invoke(InvocationContext context)
	{
		throw new InvalidOperationException("This handler should not be invoked. Use the async one instead.");
	}

	public async Task<int> InvokeAsync(InvocationContext context)
	{
		var cancellationToken = context.GetCancellationToken();
		string output = context.ParseResult.GetValueForOption(OutputPathOption) ?? string.Empty;
		string descriptionSource = context.ParseResult.GetValueForOption(DescriptionSource) ?? string.Empty;
		
		var document = await GetOpenApiDocumentAsync(descriptionSource, cancellationToken);
		ReplaceOperationIds(document);
		await SaveDocumentAsync(document, output, cancellationToken);

		return 0;
	}

    private static void ReplaceOperationIds(OpenApiDocument document)
    {
		var walker = new OpenApiWalker(new OperationIdRenameVisitor());
		walker.Walk(document);
    }

    private static async Task SaveDocumentAsync(OpenApiDocument document, string output, CancellationToken cancellationToken)
    {
		if (File.Exists(output))
		{
			File.Delete(output);
		}
        using var outputWriter = File.Create(output);
		using var textWriter = new StreamWriter(outputWriter);
		var writer = new OpenApiYamlWriter(textWriter);
		document.SerializeAsV3(writer);
		await outputWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    private static readonly HttpClient httpClient = new();
	private static async Task<OpenApiDocument> GetOpenApiDocumentAsync(string source, CancellationToken cancellationToken)
	{
		var settings = new OpenApiReaderSettings();
		var reader = new OpenApiStreamReader(settings);
		using var input = source.StartsWith("http", StringComparison.OrdinalIgnoreCase) switch
		{
			true => await httpClient.GetStreamAsync(source, cancellationToken).ConfigureAwait(false),
			false => File.Open(source, FileMode.Open, FileAccess.Read, FileShare.Read),
		};
		var readResult = await reader.ReadAsync(input, cancellationToken).ConfigureAwait(false);
		if (readResult.OpenApiDocument is not null)
		{
			return readResult.OpenApiDocument;
		}
		else
		{
			throw new InvalidOperationException("The OpenAPI document could not be read. {Message}", new Exception(readResult.OpenApiDiagnostic.Errors.FirstOrDefault()?.Message));
		}
	}
}