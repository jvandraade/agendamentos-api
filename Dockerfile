# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["AgendamentosApi.csproj", "./"]
RUN dotnet restore "AgendamentosApi.csproj"
COPY . .
RUN dotnet build "AgendamentosApi.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "AgendamentosApi.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AgendamentosApi.dll"]