# =====================================================
# Dockerfile for StudentMS ASP.NET Core Web API
# Place this file in the ROOT of your StudentMS folder
# =====================================================

# Stage 1: Build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project file and restore dependencies
COPY Backend/StudentMS.Api.csproj ./Backend/
RUN dotnet restore ./Backend/StudentMS.Api.csproj

# Copy everything else and publish
COPY Backend/ ./Backend/
RUN dotnet publish ./Backend/StudentMS.Api.csproj -c Release -o /app/publish

# Stage 2: Run the app (smaller final image)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Render sets the PORT environment variable automatically
# ASP.NET Core reads it from Program.cs (already configured)
EXPOSE 8080

ENTRYPOINT ["dotnet", "StudentMS.Api.dll"]
