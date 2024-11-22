using System.CommandLine;
using PreProcessor;

var descriptionSourceArgument = new Option<string>("--source", () => "https://aka.ms/graph/v1.0/openapi.yaml", "The path or URL to the file containing the description of the API.") {
	Description = "The path or URL to the file containing the description of the API.",
	Arity = ArgumentArity.ExactlyOne,
};
var outputPathOption = new Option<string>("--output-path", () => "./output.yaml", "The path to the output file.") {
	Description = "The path to the output file.",
	Arity = ArgumentArity.ExactlyOne,
};
outputPathOption.AddAlias("-o");
var rootCommand = new RootCommand() {
	descriptionSourceArgument,
	outputPathOption,
};
rootCommand.Handler = new RootCommandHandler() {
	DescriptionSource = descriptionSourceArgument,
	OutputPathOption = outputPathOption,
};
return await rootCommand.InvokeAsync(args);