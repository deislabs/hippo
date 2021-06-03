# Configuration options

## Development configuration

During development you may want to change Hippo's configuration locally, without modifying
the checked-in config files. You can modify the following options using environment
variables.

### Job scheduler

By default, Hippo schedules WAGI instances using `systemd`, which requires sudo privileges.
If you have the `wagi` binary locally, you can instead schedule WAGI instances in your user account
without sudo.  To do this, set the environment variable `HIPPO_JOB_SCHEDULER` to `local_process`.

By default, the `local_process` scheduler expects to find the `wagi` binary on your path.
You can set the environment variable `HIPPO_WAGI_PATH` to the path of the binary you want to
use. On Windows this should include the `.exe` extension.
