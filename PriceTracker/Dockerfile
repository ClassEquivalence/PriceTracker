
# --- Этап 1: Build ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем csproj и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# Копируем весь проект и публикуем в Release
COPY . ./
RUN dotnet publish -c Release --self-contained false -o /app/publish

# --- Этап 2: Runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Копируем публикуемый билд
COPY --from=build /app/publish .

# Переменная среды для Production (как в launch profile)
ENV ASPNETCORE_ENVIRONMENT=Production

ENV ASPNETCORE_HTTP_PORTS=5094

# Открываем порты
# EXPOSE 7170
EXPOSE 5094

# Если нужно HTTPS с сертификатом в контейнере:
# ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
# ENV ASPNETCORE_Kestrel__Certificates__Default__Password=YourPassword

# Запуск приложения
ENTRYPOINT ["dotnet", "PriceTracker.dll"]

# ENV ASPNETCORE_URLS=http://0.0.0.0:5094