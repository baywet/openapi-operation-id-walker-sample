using System.Text.RegularExpressions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;

namespace PreProcessor;

public partial class OperationIdRenameVisitor : OpenApiVisitorBase
{
	public override void Visit(OpenApiPaths paths)
	{
		foreach (var pathItem in paths)
		{
			foreach (var operation in pathItem.Value.Operations)
			{
				operation.Value.OperationId = GetNewOperationId(pathItem.Key, operation.Value, operation.Key);
			}
		}
	}
	[GeneratedRegex(@"[(){}\/](?<firstLetter>\w)", RegexOptions.Compiled)]
	private static partial Regex PathCleanupRegex { get;}
	private static string GetNewOperationId(string path, OpenApiOperation operation, OperationType operationType)
	{
		var cleanedUpPath = FirstCharToUpper(PathCleanupRegex.Replace(path, (m) => m.Groups["firstLetter"].Value.ToUpperInvariant()));
		return $"{operationType.ToString().ToLowerInvariant()}{cleanedUpPath}";
	}
	public static string FirstCharToUpper(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return string.Empty;
		}
		return $"{char.ToUpper(input[0])}{input[1..]}";
	}
}