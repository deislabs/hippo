# Project Hippo

Hippo is the easiest way to deploy and serve applications compiled to WASI.

**WARNING:** This is experimental code. It is not considered production-grade by
its developers, nor is it "supported" software.

> DeisLabs is experimenting with many WASM technologies right now. This is one
> of a multitude of projects (including
> [Krustlet](https://github.com/deislabs/krustlet)) designed to test the limits
> of WebAssembly as a cloud-based runtime.

Hippo deploys and serves applications compiled to WASI. Developers write an
application and compile it to WASI. Applications are uploaded via Hippo's
management portal. In seconds, applications are served on the web.

Hippo provides an easy-to-use management portal for your application, allowing
users to deploy and roll back their application releases, edit configuration,
manage domain routing and TLS certificates, and view agrregated logs. Hippo
provides a simple permissions model to manage your users and groups.

Hippo provides a seamless edge routing model so you and your developers can
spend more time writing your application, and less time editing load balancer
configuration. Just add a domain to an application, attach a certificate, and
let Hippo do the work.

All of Hippo's functionality is exposed through a web application and (coming
soon) a command line interface.

## Documentation

If you're new to the project, get started with [the
introduction](docs/intro/README.md). For more in-depth information about
Hippo, plunge right into the [topic guides](docs/topics/README.md).

Looking for the developer guide? [Start here](docs/community/developers.md).

## Community, discussion, contribution, and support

Right now, all community discussion takes place here at the [project
homepage](https://github.com/deislabs/hippo). If you have a question about the
project or want to start a discussion, [search for existing
topics](https://github.com/deislabs/hippo/issues?q=is%3Aissue) or [open a new
topic](https://github.com/deislabs/hippo/issues/new).

## Code of Conduct

This project has adopted the [Microsoft Open Source Code of
Conduct](https://opensource.microsoft.com/codeofconduct/).

For more information see the [Code of Conduct
FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact
[opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional
questions or comments.
