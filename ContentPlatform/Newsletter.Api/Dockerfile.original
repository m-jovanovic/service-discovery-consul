FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Newsletter.Api/Newsletter.Api.csproj", "Newsletter.Api/"]
RUN dotnet restore "Newsletter.Api/Newsletter.Api.csproj"
COPY . .
WORKDIR "/src/Newsletter.Api"
RUN dotnet build "Newsletter.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Newsletter.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Newsletter.Api.dll"]