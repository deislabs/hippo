# Nomad Local Development Environment

[Consul](https://www.consul.io)
[Vault](https://www.vaultproject.io)
[Nomad](https://www.nomadproject.io/)
[Traefik](https://traefik.io/)

## Run services locally

### Prerequisites

* Consul
* Nomad
* Traefik
* Vault

```shell
run_servers.sh
```

## Web interfaces

[Nomad](http://localhost:4646)
[Consul](http://localhost:8500)
[Vault](http://localhost:8200)
[Traefik](http://localhost:8081)

## IPv6 and docker

If nomad complains about IPv6 it can be disabled on the system by using this
command.

```
networksetup -setv6linklocal Wi-Fi
```
