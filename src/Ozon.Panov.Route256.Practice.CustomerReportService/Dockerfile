FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Ozon.Panov.Route256.Practice.CustomerReportService/Ozon.Panov.Route256.Practice.CustomerReportService.csproj", "src/Ozon.Panov.Route256.Practice.CustomerReportService/"]
RUN dotnet restore "./src/Ozon.Panov.Route256.Practice.CustomerReportService/Ozon.Panov.Route256.Practice.CustomerReportService.csproj"
COPY . .
WORKDIR "/src/src/Ozon.Panov.Route256.Practice.CustomerReportService"
RUN dotnet build "./Ozon.Panov.Route256.Practice.CustomerReportService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ozon.Panov.Route256.Practice.CustomerReportService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ozon.Panov.Route256.Practice.CustomerReportService.dll"]