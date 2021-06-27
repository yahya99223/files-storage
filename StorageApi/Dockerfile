FROM mcr.microsoft.com/dotnet/aspnet:5.0.5-buster-slim AS base


WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0.202-buster-slim AS build
WORKDIR /
COPY ./ ./
RUN dotnet restore FlyingDonkey.Storage.Api/FlyingDonkey.Storage.Api.csproj
COPY . .
WORKDIR /
RUN dotnet build FlyingDonkey.Storage.Api/FlyingDonkey.Storage.Api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish FlyingDonkey.Storage.Api/FlyingDonkey.Storage.Api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FlyingDonkey.Storage.Api.dll"]