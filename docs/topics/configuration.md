# Configuration options

## Development configuration

During development you may want to change Hippo's configuration locally, without modifying
the checked-in config files. You can modify the following options using environment
variables.

### Job scheduler

By default, Hippo runs applications in your user account using the `wagi` binary. It assumes
`wagi` is on your system PATH. If `wagi` isn't on your path, you can set the environment
variable `HIPPO_WAGI_PATH` to the path of the binary you want to use. On Windows this should
include the `.exe` extension.

Alternatively, Hippo can schedule WAGI instances as system processes using `systemd`.
To do this, set the environment variable `HIPPO_JOB_SCHEDULER` to `systemd`. The `systemd`
scheduler expects the WAGI binary to be at `/usr/local/bin/wagi` and this is not configurable
at the moment.


