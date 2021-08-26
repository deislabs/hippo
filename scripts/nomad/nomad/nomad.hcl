# bind_addr = "0.0.0.0"
log_level            = "INFO"
disable_update_check = true

# these settings allow Nomad to automatically find its peers through Consul
consul {
  server_service_name = "nomad"
  server_auto_join    = true
  client_service_name = "nomad-client"
  client_auto_join    = true
  auto_advertise      = true
}

server {
  enabled          = true
  bootstrap_expect = 1
}

client {
  options = {
    "driver.blacklist" = "java"
  }
}

vault {
  enabled          = true
  tls_skip_verify  = true
  create_from_role = "nomad-cluster"
}

telemetry {
  statsd_address             = "10.10.10.133:9125"
  publish_allocation_metrics = true
  publish_node_metrics       = true
}
