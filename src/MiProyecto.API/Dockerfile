FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/MiProyecto.Domain/MiProyecto.Domain.csproj src/MiProyecto.Domain/
COPY src/MiProyecto.Application/MiProyecto.Application.csproj src/MiProyecto.Application/
COPY src/MiProyecto.Infrastructure/MiProyecto.Infrastructure.csproj src/MiProyecto.Infrastructure/
COPY src/MiProyecto.API/MiProyecto.API.csproj src/MiProyecto.API/

RUN dotnet restore src/MiProyecto.API/MiProyecto.API.csproj

COPY src/ src/
RUN dotnet publish src/MiProyecto.API/MiProyecto.API.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "MiProyecto.API.dll"]
