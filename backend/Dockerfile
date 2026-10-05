# Stage 1: build the app using the full .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ClinicBooking.Api/ClinicBooking.Api.csproj ClinicBooking.Api/
RUN dotnet restore ClinicBooking.Api/ClinicBooking.Api.csproj
COPY ClinicBooking.Api/ ClinicBooking.Api/
RUN dotnet publish ClinicBooking.Api/ClinicBooking.Api.csproj -c Release -o /app

# Stage 2: run the app using the smaller ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "ClinicBooking.Api.dll"]