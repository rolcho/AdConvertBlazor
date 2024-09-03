FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . .

RUN dotnet publish AdConvert.csproj -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine

WORKDIR /app

COPY --from=build /app/out .
COPY --from=build /app/wwwroot ./wwwroot

CMD ["dotnet", "AdConvert.dll"]