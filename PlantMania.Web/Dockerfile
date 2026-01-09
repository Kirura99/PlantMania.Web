# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY PlantMania.Web/PlantMania.Web.csproj PlantMania.Web/
RUN dotnet restore PlantMania.Web/PlantMania.Web.csproj

COPY . .
RUN dotnet publish PlantMania.Web/PlantMania.Web.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "PlantMania.Web.dll"]
