FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
RUN apk --update add icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT false
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY src/**/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./${file%.*}/ && mv $file ./${file%.*}/; done
ENV HUSKY=0
RUN dotnet restore "DoliteTemplate.Api/DoliteTemplate.Api.csproj"
COPY src .
WORKDIR "/src/DoliteTemplate.Api"
RUN dotnet build "DoliteTemplate.Api.csproj" -c Release -o /app/build --no-self-contained

FROM build AS publish
RUN dotnet publish "DoliteTemplate.Api.csproj" -c Release -o /app/publish --no-self-contained

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DoliteTemplate.Api.dll"]
