# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar o arquivo de solução e os arquivos .csproj
COPY RestApiCase.sln .
COPY RestApiCase.Api/RestApiCase.Api.csproj RestApiCase.Api/
COPY RestApiCase.Application/RestApiCase.Application.csproj RestApiCase.Application/
COPY RestApiCase.Domain/RestApiCase.Domain.csproj RestApiCase.Domain/
COPY RestApiCase.Infrastructure/RestApiCase.Infrastructure.csproj RestApiCase.Infrastructure/

# Restaurar dependências
RUN dotnet restore RestApiCase.sln

# Copiar o restante dos arquivos
COPY RestApiCase.Api/ RestApiCase.Api/
COPY RestApiCase.Application/ RestApiCase.Application/
COPY RestApiCase.Domain/ RestApiCase.Domain/
COPY RestApiCase.Infrastructure/ RestApiCase.Infrastructure/

# Build do projeto
WORKDIR /src/RestApiCase.Api
RUN dotnet build RestApiCase.Api.csproj -c Release -o /app/build

# Publicar o projeto
RUN dotnet publish RestApiCase.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
ENTRYPOINT ["dotnet", "RestApiCase.Api.dll"]