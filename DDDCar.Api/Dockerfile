FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY DDDCar.Auth/DDDCar.Auth.csproj DDDCar.Auth/
COPY DDDCar.ApplicationSettings/DDDCar.ApplicationSettings.csproj DDDCar.ApplicationSettings/ 

RUN dotnet restore DDDCar.Auth/DDDCar.Auth.csproj

COPY . . 

WORKDIR /src/DDDCar.Auth

RUN dotnet publish DDDCar.Auth.csproj -c $BUILD_CONFIGURATION \
    -o /app/publish --no-restore --p:UseAppHost=false 

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DDDCar.Auth.dll"]