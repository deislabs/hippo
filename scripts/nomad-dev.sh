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

require dotnet
require bindle-server
require consul
require nomad
require spin
require traefik

trap 'sudo kill $(jobs -p)' EXIT

echo "Starting consul..."
consul agent -dev -bootstrap-expect 1 -client '0.0.0.0' &>/tmp/consul.log &

echo "Starting nomad..."
# NOTE(bacongobbler): nomad MUST run as root for the exec driver to work on Linux.
# https://github.com/deislabs/hippo/blob/de73ae52d606c0a2351f90069e96acea831281bc/src/Infrastructure/Jobs/NomadJob.cs#L28
# https://www.nomadproject.io/docs/drivers/exec#client-requirements
if [[ "$OSTYPE" == "linux"* ]]
then
  sudo nomad agent -dev &>/tmp/nomad.log &
else
  nomad agent -dev &>/tmp/nomad.log &
fi

echo "Waiting for nomad..."
while ! nomad server members 2>/dev/null | grep -q alive; do
  sleep 1
done

echo "Starting traefik..."
sudo traefik --api.dashboard=true --api.insecure=true --entryPoints.http.address=:80 --entryPoints.traefik.address=:8081 --providers.consulCatalog &>/tmp/traefik.log &

echo "Starting bindle..."
bindle-server --unauthenticated &>/tmp/bindle.log &

echo "Starting hippo..."
dotnet run --project src/Web &>/tmp/hippo.log &

echo "Logs are available at:"
echo
echo "Consul: /tmp/consul.log"
echo "Nomad: /tmp/nomad.log"
echo "Traefik: /tmp/traefik.log"
echo "Bindle: /tmp/bindle.log"
echo "Hippo: /tmp/hippo.log"
echo
echo "Consul:  http://localhost:8500"
echo "Nomad:   http://localhost:4646"
echo "Traefik: http://localhost:8081"
echo "Bindle:  http://bindle.hippofactory.io:8080"
echo "Hippo:   http://hippo.hippofactory.io:5309"
echo
echo
echo "Press CTRL+C to exit."

wait
