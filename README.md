# OpenAPI Operation Id transformation sample

## Introduction

This sample demonstrates how a document can be parsed to replace the operation ids.

## Requirement

- Docker desktop or equivalent.

--or--

- [Dotnet SDK 9](https://get.dot.net)

## Running the code

> Note: in bose cases the parameters demonstrated here are the default values.

### With the dotnet SDK

```shell
dotnet run --project ./PreProcessor/PreProcessor.csproj -- -s "https://aka.ms/graph/v1.0/openapi.yaml" -o "./output.yaml"
```

### With docker

```shell
docker build . -t sample
docker run --rm -v $(pwd):/app/data:rw sample -s "https://aka.ms/graph/v1.0/openapi.yaml" -o "./output.yaml"
```

### The interesting content

Head over to **PreProcessor/OperationIdRenameVisitor.cs** the code should be self explanatory.

**PreProcessor/RootCommandHandler.cs** also demonstrates the management code (description read/queue the walker/save)

> Note: Although this sample should also work with a local source file, it's only been tested with a URL based source.
