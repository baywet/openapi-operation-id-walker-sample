FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
ARG version_suffix
WORKDIR /app

COPY ./PreProcessor PreProcessor
RUN mkdir -p /app/data
RUN dotnet publish ./PreProcessor/PreProcessor.csproj -c Release -p:TreatWarningsAsErrors=false;

FROM mcr.microsoft.com/dotnet/runtime:9.0-noble-chiseled AS runtime
WORKDIR /app

COPY --from=build-env /app/PreProcessor/bin/Release/net9.0 ./
COPY --from=build-env /app/data /app/data
VOLUME [ "/app/data" ]

ENV DOTNET_TieredPGO=1 DOTNET_TC_QuickJitForLoops=1
ENTRYPOINT ["dotnet", "PreProcessor.dll"]