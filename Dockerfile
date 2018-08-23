FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY AkkaAMQPConsumerProducer/AkkaAMQPConsumerProducer.csproj AkkaAMQPConsumerProducer/
COPY AkkaAMQPConsumerProducer.Core/AkkaAMQPConsumerProducer.Core.csproj AkkaAMQPConsumerProducer.Core/
COPY AkkaAMQPConsumerProducer.Infrastructure/AkkaAMQPConsumerProducer.Infrastructure.csproj AkkaAMQPConsumerProducer.Infrastructure/
RUN dotnet restore AkkaAMQPConsumerProducer/AkkaAMQPConsumerProducer.csproj
RUN dotnet restore AkkaAMQPConsumerProducer.Infrastructure/AkkaAMQPConsumerProducer.Infrastructure.csproj
COPY . .
WORKDIR /src/AkkaAMQPConsumerProducer
RUN dotnet build AkkaAMQPConsumerProducer.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish AkkaAMQPConsumerProducer.csproj -c Release -o /app

FROM build AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AkkaAMQPConsumerProducer.dll"]