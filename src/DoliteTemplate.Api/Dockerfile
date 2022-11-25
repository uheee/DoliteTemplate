FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src
COPY ./**/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./${file%.*}/ && mv $file ./${file%.*}/; done
RUN dotnet restore "DoliteTemplate.Api/DoliteTemplate.Api.csproj"
COPY . .
WORKDIR "/src/DoliteTemplate.Api"
RUN dotnet build "DoliteTemplate.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DoliteTemplate.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DoliteTemplate.Api.dll"]
