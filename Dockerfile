# Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env

WORKDIR /app

COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:5.0-alpine

WORKDIR /app

COPY --from=build-env /app/out /app/

RUN cat | ls /app/Assets/

CMD ASPNETCORE_URLS=http://*:$PORT dotnet pesisBackend.dll