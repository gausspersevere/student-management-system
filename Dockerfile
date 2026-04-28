


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app


COPY Backend/StudentMS.Api.csproj ./Backend/
RUN dotnet restore ./Backend/StudentMS.Api.csproj


COPY Backend/ ./Backend/
RUN dotnet publish ./Backend/StudentMS.Api.csproj -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .


EXPOSE 8080

ENTRYPOINT ["dotnet", "StudentMS.Api.dll"]
