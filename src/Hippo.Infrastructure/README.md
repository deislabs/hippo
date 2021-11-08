# Hippo.Infrastructure

Most of Hippo's dependencies on external resources should be implemented in classes defined here in the Infrastructure project. These classes should implement interfaces defined in Core.

The Infrastructure project should include things like:

- data access
- domain event implementations
- authentication providers
- email providers
- file access
- web api clients

This way there's no tight coupling of these implementation's dependencies to either Hippo's Core or Web projects.
