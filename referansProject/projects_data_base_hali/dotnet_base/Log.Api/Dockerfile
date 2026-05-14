# Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Presentation\Log.Api\Dockerfile

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Presentation/Log.Api/Log.Api.csproj", "src/Presentation/Log.Api/"]
COPY ["src/Shared/Shared.Domain/Shared.Domain.csproj", "src/Shared/Shared.Domain/"]
COPY ["Framework/Core/FrameworkCore/FrameworkCore.csproj", "Framework/Core/FrameworkCore/"]
COPY ["src/Core/Applications/Log.Application/Log.Application.csproj", "src/Core/Applications/Log.Application/"]
COPY ["src/Core/Persistences/Log.Persistence/Log.Persistence.csproj", "src/Core/Persistences/Log.Persistence/"]
RUN dotnet restore "src/Presentation/Log.Api/Log.Api.csproj"
COPY . .
WORKDIR "/src/src/Presentation/Log.Api"
RUN dotnet build "Log.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Log.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Log.Api.dll"]

ENV TZ=Europe/Istanbul
RUN ln -fs /usr/share/zoneinfo/$TZ /etc/localtime && dpkg-reconfigure -f noninteractive tzdata
RUN date
