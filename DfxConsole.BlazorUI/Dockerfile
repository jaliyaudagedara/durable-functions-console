# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG VERSION=2.0.0
ARG BUILD_DATE
ARG VCS_REF
WORKDIR /src
COPY ["DfxConsole.BlazorUI.csproj", "."]
RUN dotnet restore "./DfxConsole.BlazorUI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./DfxConsole.BlazorUI.csproj" -c $BUILD_CONFIGURATION -o /app/build /p:Version=$VERSION /p:InformationalVersion=$VERSION

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ARG VERSION=2.0.0
RUN dotnet publish "./DfxConsole.BlazorUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:Version=$VERSION /p:InformationalVersion=$VERSION

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DfxConsole.BlazorUI.dll"]