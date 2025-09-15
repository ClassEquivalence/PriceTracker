
# ===== STAGE 1: Build =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем csproj и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# Копируем остальной код и собираем проект
COPY . .
RUN dotnet publish -c Release -r linux-x64 --self-contained false -o /app

# Устанавливаем Playwright и скачиваем Chromium с зависимостями
RUN pwsh /app/playwright.ps1 install chromium --with-deps

# Опционально: задаем переменную для Playwright
ENV DOTNET_RUNNING_IN_CONTAINER=true

ENTRYPOINT ["dotnet", "PriceTracker.dll"]
