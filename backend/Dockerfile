FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish src/Api/Api.csproj -c Release -o /out
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
WORKDIR /app
RUN adduser -D app
COPY --from=build --chown=app:app /out .
USER app
EXPOSE 5100
ENTRYPOINT ["dotnet", "Api.dll"]
