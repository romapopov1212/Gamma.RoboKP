FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем ТОЛЬКО файлы проектов (для ускорения сборки)
COPY ["Gamma.RoboKP/Gamma.RoboKP.csproj", "Gamma.RoboKP/"]
COPY ["Gamma.RoboKP.Infrastructure/Gamma.RoboKP.Infrastructure.csproj", "Gamma.RoboKP.Infrastructure/"]
COPY ["Gamma.RoboKP.Application/Gamma.RoboKP.Application.csproj", "Gamma.RoboKP.Application/"]
COPY ["Gamma.RoboKP.Domain/Gamma.RoboKP.Domain.csproj", "Gamma.RoboKP.Domain/"]

# Восстанавливаем зависимости основного проекта
RUN dotnet restore "Gamma.RoboKP/Gamma.RoboKP.csproj"

# Копируем ВСЕ остальные файлы
COPY . .

# Собираем проект
WORKDIR "/src/Gamma.RoboKP"
RUN dotnet build "Gamma.RoboKP.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Устанавливаем dotnet-ef (если нужно)
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Публикуем проект
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Gamma.RoboKP.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /root/.dotnet/tools /root/.dotnet/tools
ENV PATH="$PATH:/root/.dotnet/tools"
ENTRYPOINT ["dotnet", "Gamma.RoboKP.dll"]