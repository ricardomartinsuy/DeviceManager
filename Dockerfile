FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY DevicesApi.sln ./
COPY src/DevicesApi.Domain/DevicesApi.Domain.csproj             src/DevicesApi.Domain/
COPY src/DevicesApi.Application/DevicesApi.Application.csproj   src/DevicesApi.Application/
COPY src/DevicesApi.Infrastructure/DevicesApi.Infrastructure.csproj src/DevicesApi.Infrastructure/
COPY src/DevicesApi.Api/DevicesApi.Api.csproj                   src/DevicesApi.Api/

RUN dotnet restore src/DevicesApi.Api/DevicesApi.Api.csproj

COPY src/ src/
RUN dotnet publish src/DevicesApi.Api/DevicesApi.Api.csproj \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "DevicesApi.Api.dll"]
