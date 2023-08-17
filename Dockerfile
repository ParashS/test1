#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
#FROM reg-harbor.agiletechnologies.in/test1/test1:pro AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
#FROM reg-harbor.agiletechnologies.in/test1/test1:pro AS build
WORKDIR /src
COPY ["Vessels/Vessels.csproj", "Vessels/"]
COPY ["Entities/Entities.csproj", "Entities/"]
COPY ["Repository/Repository.csproj", "Repository/"]
COPY ["Contracts/Contracts.csproj", "Contracts/"]
COPY ["ServiceContracts/ServiceContracts.csproj", "ServiceContracts/"]
COPY ["DataTransferObjects/DataTransferObjects.csproj", "DataTransferObjects/"]
COPY ["Services/Services.csproj", "Services/"]
RUN dotnet restore "Vessels/Vessels.csproj"
COPY . .
WORKDIR "/src/Vessels"
RUN dotnet build "Vessels.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Vessels.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vessels.dll"]
