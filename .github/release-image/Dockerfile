FROM amd64/debian:buster-slim

RUN export DEBIAN_FRONTEND=noninteractive && apt-get update \
    && apt-get install -y --no-install-recommends \
        ca-certificates \
        apt-transport-https \
        libc6 \
        libgcc1 \
        libgssapi-krb5-2 \
        libicu63 \
        libssl1.1 \
        libstdc++6 \
        zlib1g \
        curl \
        sudo \
        tar \
        gcc \
        gcc-multilib \
        procps \
        apache2-utils \
    && rm -rf /var/lib/apt/lists/*

ARG USER_NAME=""
ENV USER ${USER_NAME:-hippouser}

RUN groupadd --gid 1000 ${USER} &&  useradd -s /bin/bash --uid 1000 --gid 1000 -m ${USER}  &&  echo ${USER}  ALL=\(root\) NOPASSWD:ALL > /etc/sudoers.d/${USER}  && chmod 0440 /etc/sudoers.d/${USER}
RUN mkdir /data && chown ${USER} /data && chgrp ${USER} /data
RUN su ${USER} -c "mkdir -p /data/certs && mkdir -p /data/bindleserver && mkdir -p /data/hippo && mkdir -p /data/logs"

# Install Node.js

ARG NODE_VERSION="lts/*"
RUN su ${USER} -c "curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.38.0/install.sh |  bash && chmod +x /home/${USER}/.nvm/nvm.sh"

WORKDIR /tmp

# Install hippo cli
ARG HIPPO_CLI_VERSION="v0.14.1"
RUN mkdir hippocli && cd hippocli && curl -fsSLo hippocli.tar.gz https://github.com/deislabs/hippo-cli/releases/download/${HIPPO_CLI_VERSION}/hippo-${HIPPO_CLI_VERSION}-linux-amd64.tar.gz && tar -xvf hippocli.tar.gz && mv hippo README.md LICENSE /usr/local/bin/ && cd - && rm -r hippocli

# Install bindle
ARG BINDLE_VERSION="v0.8.0"
RUN mkdir bindle && cd bindle && curl -fsSLo bindle.tar.gz https://bindle.blob.core.windows.net/releases/bindle-${BINDLE_VERSION}-linux-amd64.tar.gz && tar -xvf bindle.tar.gz && mv bindle bindle-server README.md LICENSE.txt /usr/local/bin/ && cd - && rm -r bindle

# Install spin
ARG SPIN_VERSION="v0.2.0"
RUN mkdir spin && cd spin && curl -fsSLo spin.tar.gz https://github.com/fermyon/spin/releases/download/${SPIN_VERSION}/spin-${SPIN_VERSION}-linux-amd64.tar.gz  && tar -xvf spin.tar.gz && mv spin readme.md LICENSE /usr/local/bin/ && cd - && rm -r spin

# Install Rust
RUN su ${USER} -c "mkdir rust && cd rust && curl -fsSLo install_rust.sh https://sh.rustup.rs && chmod +x ./install_rust.sh  && ./install_rust.sh -y -t wasm32-wasi && cd - && rm -r rust"

COPY --chown=1000:1000 . /hippo

WORKDIR /hippo

ARG ENVIRONMENT="Development"
ENV ASPNETCORE_ENVIRONMENT ${ENVIRONMENT}

ARG BINDLE_PORT="8080"
ENV BINDLE_URL http://localhost:${BINDLE_PORT}/v1
ENV BINDLE_LISTEN_ADDRESS 0.0.0.0:${BINDLE_PORT}

ARG BINDLE_SERVER_USER=""
ENV BINDLE_USERNAME ${BINDLE_SERVER_USER:-bindleuser}

ARG FORCE_GLOBAL_AGENT="false"
ENV GLOBAL_AGENT_FORCE_GLOBAL_AGENT ${FORCE_GLOBAL_AGENT}

# Docker/WSL2 has an issue exposing ports from localhost. the following tries to work around this issue.
# See https://github.com/microsoft/WSL/issues/4983
ARG HIPPO_PORT="5309"

ARG HIPPOURL=http://hippo.hippofactory.io:${HIPPO_PORT}
ENV HIPPO_URL ${HIPPOURL}

ENV ASPNETCORE_URLS http://0.0.0.0:${HIPPO_PORT}

# Generate certs for hippo-server and proxy
WORKDIR /data/certs
RUN openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout localhost.key -out localhost.crt -config /hippo/certs/localhost.conf && openssl pkcs12 -export -out localhost.pfx -inkey localhost.key -in localhost.crt -password pass: &&  sudo chgrp ${USER} localhost.pfx && chmod g+r localhost.pfx
RUN cp /data/certs/localhost.crt /usr/local/share/ca-certificates && update-ca-certificates
ENV KESTREL__CERTIFICATES__DEFAULT__PATH /data/certs/localhost.pfx
ENV HIPPO_REVERSE_PROXY_KESTREL__CERTIFICATES__DEFAULT__PATH /data/certs/localhost.pfx

# Set the data source to a local folder that can be mounted from the host.
ENV CONNECTIONSTRINGS__HIPPO Data Source=/data/hippo/hippo.db;Cache=Shared

WORKDIR /hippo

# Run bindle-server and hippo-server.

USER 1000

ENTRYPOINT if [ -z ${BINDLE_PASSWORD} ]; then export BINDLE_PASSWORD=$(openssl rand -base64 12);echo export BINDLE_PASSWORD=${BINDLE_PASSWORD} >> ~/.bashrc;  fi && echo ${BINDLE_PASSWORD}|htpasswd -Bic /data/bindleserver/bindle-htpasswd ${BINDLE_USERNAME} && RUST_LOG=info bindle-server -i ${BINDLE_LISTEN_ADDRESS} --htpasswd-file /data/bindleserver/bindle-htpasswd -d /data/bindleserver >> /data/logs/bindle-server.log 2>&1 & ./hippo-server >> /data/logs/hippo-server.log 2>&1
