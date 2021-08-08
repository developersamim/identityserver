#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["services/identity/identityserver/identityserver.csproj", "services/identity/identityserver/"]
COPY ["common/common.api/common.api.csproj", "common/common.api/"]
RUN dotnet restore "services/identity/identityserver/identityserver.csproj"
COPY . .
WORKDIR "/src/services/identity/identityserver"
RUN dotnet build "identityserver.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "identityserver.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "identityserver.dll"]
