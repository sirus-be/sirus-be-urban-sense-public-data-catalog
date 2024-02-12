#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["DataCatalog.Portal.csproj", "Components/FrontEnd/Portal/DataCatalog.Portal/"]
WORKDIR "src/Components/FrontEnd/Portal/DataCatalog.Portal"
RUN dotnet restore "DataCatalog.Portal.csproj"
COPY . .
RUN dotnet build "DataCatalog.Portal.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataCatalog.Portal.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataCatalog.Portal.dll"]