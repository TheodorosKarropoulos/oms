FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/app/OMS.Api/OMS.Api.csproj", "src/app/OMS.Api/"]
COPY ["src/lib/OMS.Application/OMS.Application.csproj", "src/lib/OMS.Application/"]
COPY ["src/lib/OMS.Domain/OMS.Domain.csproj", "src/lib/OMS.Domain/"]
COPY ["src/lib/OMS.Infrastructure/OMS.Infrastructure.csproj", "src/lib/OMS.Infrastructure/"]

RUN dotnet restore "src/app/OMS.Api/OMS.Api.csproj"

COPY . .

RUN dotnet publish "src/app/OMS.Api/OMS.Api.csproj" \
      -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "OMS.Api.dll"]