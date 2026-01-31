FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/ .
RUN dotnet restore "Backend.WebAPI.Hades/Backend.WebAPI.Hades.csproj"
RUN dotnet publish "Backend.WebAPI.Hades/Backend.WebAPI.Hades.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY src/Backend.Data/Db/hades.db /app/data/hades.db
ENV ASPNETCORE_URLS=http://+:8080
ENV ConnectionStrings__Sqlite="Data Source=/app/data/hades.db"
ENTRYPOINT ["dotnet", "Backend.WebAPI.Hades.dll"]
