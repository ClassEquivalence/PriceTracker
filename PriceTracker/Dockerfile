
# Stage 1: Download headless Chromium via Playwright
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS playwright

WORKDIR /app
COPY bin/Release/net8.0/linux-x64/ .

# Download ONLY Chromium (headless) using playwright.ps1
RUN pwsh ./playwright.ps1 install chromium --with-deps

ENTRYPOINT ["dotnet", "PriceTracker.dll"]

