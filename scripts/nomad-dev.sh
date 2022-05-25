#!/usr/bin/env bash
set -euo pipefail

root_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${root_dir}"

require() {
  if ! hash "$1" &>/dev/null; then
    echo "'$1' not found in PATH"
    exit 1
  fi
}

require bindle-server
require consul
require nomad
require spin
require traefik

trap 'kill $(jobs -p)' EXIT

echo "Starting consul..."
consul agent -dev -bootstrap-expect 1 -client '0.0.0.0' &>/tmp/consul.log &

echo "Starting nomad..."
nomad agent -dev &>/tmp/nomad.log &

echo "Starting traefik..."
traefik --entryPoints.http.address=:8088 --providers.consulCatalog &>/tmp/traefik.log &

echo "Starting bindle..."
bindle-server --unauthenticated &>/tmp/bindle.log &

echo "Waiting for nomad..."
while ! nomad server members 2>/dev/null | grep -q alive; do
  sleep 2
done

echo "Starting hippo..."
dotnet run --project src/Web \
  --Hippo:PlatformDomain='127.0.0.1.sslip.io' &

wait
