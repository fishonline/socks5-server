FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Socks5Server/Socks5Server.csproj", "Socks5Server/"]
RUN dotnet restore "Socks5Server/Socks5Server.csproj"
COPY . .
WORKDIR "/src/Socks5Server"
RUN dotnet build "Socks5Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Socks5Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 7582
ENTRYPOINT ["dotnet", "Socks5Server.dll"]
