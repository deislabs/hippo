# See here for image contents: https://github.com/microsoft/vscode-dev-containers/tree/v0.187.0/containers/dotnet/.devcontainer/base.Dockerfile

# [Choice] .NET version: 5.0, 3.1, 2.1
ARG VARIANT="5.0"
FROM mcr.microsoft.com/vscode/devcontainers/dotnet:0-${VARIANT}

# Install Node.js
ARG NODE_VERSION="lts/*"
RUN su vscode -c "umask 0002 && . /usr/local/share/nvm/nvm.sh && nvm install ${NODE_VERSION} 2>&1"

# Install hippo
ARG HIPPO_VERSION="v0.7.0"
RUN curl -fsSLo hippocli.tar.gz https://github.com/deislabs/hippo-cli/releases/download/${HIPPO_VERSION}/hippo-${HIPPO_VERSION}-linux-amd64.tar.gz && tar -xvf hippocli.tar.gz && mv hippo /usr/local/bin/  

# Install bindle
ARG BINDLE_VERSION="v0.4.1"
RUN curl -fsSLo bindle.tar.gz https://bindle.blob.core.windows.net/releases/bindle-${BINDLE_VERSION}-linux-amd64.tar.gz && tar -xvf bindle.tar.gz && mv bindle bindle-server /usr/local/bin/  

# Install WAGI
ARG WAGI_VERSION="v0.2.0"
RUN curl -fsSLo wagi.tar.gz https://github.com/deislabs/wagi/releases/download/${WAGI_VERSION}/wagi-${WAGI_VERSION}-linux-amd64.tar.gz  && tar -xvf wagi.tar.gz && mv wagi /usr/local/bin/ 

# Install yo-wasm
RUN su vscode -c "umask 0002 && . /usr/local/share/nvm/nvm.sh && npm install -g yo && npm install -g generator-wasm"

# Install Rust 
RUN curl -fsSLo install_rust.sh https://sh.rustup.rs && chmod +x ./install_rust.sh  && ./install_rust.sh -y -t wasm32-wasi
