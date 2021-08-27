# See here for image contents: https://github.com/microsoft/vscode-dev-containers/tree/v0.187.0/containers/dotnet/.devcontainer/base.Dockerfile

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
ARG WAGI_VERSION="v0.3.0"
RUN curl -fsSLo wagi.tar.gz https://github.com/deislabs/wagi/releases/download/${WAGI_VERSION}/wagi-${WAGI_VERSION}-linux-amd64.tar.gz  && tar -xvf wagi.tar.gz && mv wagi /usr/local/bin/ 

# Install Rust 
RUN su vscode -c "umask 0002 && cd /tmp && mkdir rust && cd rust && curl -fsSLo install_rust.sh https://sh.rustup.rs && chmod +x ./install_rust.sh  && ./install_rust.sh -y -t wasm32-wasi"
RUN apt-get update && export DEBIAN_FRONTEND=noninteractive && apt-get -y install --no-install-recommends gcc gcc-multilib

# Install yo-wasm
RUN su vscode -c "umask 0002 && . /usr/local/share/nvm/nvm.sh && npm install -g yo && npm install -g generator-wasm"

# TODO remove this as it is temporary. The hippo enabled yo wasm package is not published yet so we need to use the one from the repo.
RUN su vscode -c "umask 0002 && cd /tmp && mkdir yowasm && cd yowasm && curl -fsSLo ./yowasm.tar.gz https://github.com/deislabs/yo-wasm/releases/download/v0.0.4%2Bpreview1/generator-wasm-0.0.3.tgz && . /usr/local/share/nvm/nvm.sh && npm install -g ./yowasm.tar.gz"