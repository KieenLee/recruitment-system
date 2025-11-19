FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["Backend.csproj", "./"]
RUN dotnet restore "Backend.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Backend.csproj" -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create directory for CV storage
RUN mkdir -p Storage/CVs

COPY --from=publish /app/publish .

# Expose port 5175
EXPOSE 5175

ENV ASPNETCORE_URLS=http://+:5175

ENTRYPOINT ["dotnet", "Backend.dll"]