FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Newsletter.Reporting.Api/Newsletter.Reporting.Api.csproj", "Newsletter.Api/"]
RUN dotnet restore "Newsletter.Reporting.Api/Newsletter.Reporting.Api.csproj"
COPY . .
WORKDIR "/src/Newsletter.Reporting.Api"
RUN dotnet build "Newsletter.Reporting.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Newsletter.Reporting.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Newsletter.Reporting.Api.dll"]