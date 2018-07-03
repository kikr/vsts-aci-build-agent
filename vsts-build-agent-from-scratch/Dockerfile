FROM microsoft/vsts-agent:ubuntu-14.04

# Install basic command-line utilities
RUN apt-get update \
 && apt-get install -y --no-install-recommends \
    curl \
    zip \
 && rm -rf /var/lib/apt/lists/*

# Install .NET Core SDK and initialize package cache
RUN curl https://packages.microsoft.com/config/ubuntu/14.04/packages-microsoft-prod.deb > packages-microsoft-prod.deb \
 && dpkg -i packages-microsoft-prod.deb \
 && rm packages-microsoft-prod.deb \
 && apt-get update

# Do not cache packages on first time. Containers are spun up on every build and cache is lost each time
# http://donovanbrown.com/post/Stop-wasting-time-during-NET-Core-builds
RUN DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true \
    apt-get install -y --no-install-recommends \
    apt-transport-https \
    dotnet-sdk-2.1 \
    && rm -rf /var/lib/apt/lists/*
ENV dotnet=/usr/bin/dotnet

# Install NodeJS
RUN curl -sL https://deb.nodesource.com/setup_8.x | sudo -E bash - \
    && apt-get install -y nodejs

# Install Chromium for headless front end tests
RUN apt-get update \
    && apt-get install -y chromium-browser
# Tell Karma where to find the Chrome alternative
ENV CHROME_BIN=/usr/bin/chromium-browser