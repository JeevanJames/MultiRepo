FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /
COPY . .
RUN dotnet publish -c Release -o /app -r linux-musl-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true ./src/Cli/Cli.csproj

FROM mcr.microsoft.com/dotnet/runtime-deps:5.0-alpine-amd64
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["./mr"]
