# Hippo, the WebAssembly PaaS

Hippo is the easiest way to deploy and serve applications compiled to
[WebAssembly](https://webassembly.org/).

**WARNING:** This is experimental code. It is not considered production-grade
by its developers, nor is it "supported" software.

> DeisLabs is experimenting with many WASM technologies right now. This is one
> of a multitude of projects (including
> [Krustlet](https://github.com/deislabs/krustlet)) designed to test the limits
> of WebAssembly as a cloud-based runtime.

Hippo takes a fresh spin on the PaaS ecosystem, taking advantage of the
technology WebAssembly brings to the space.

Hippo works like this: A WebAssembly package is bundled up as a
[bindle](https://github.com/deislabs/bindle). Bindles are collected together in
a bindle server that you can search. Hippo uses bindle under the hood for
storing and organizing applications.

Using the [hippo command line
interface](https://github.com/deislabs/hippo-cli), you can upload new releases
or prepare a bindle for local development. In the future, you can use this CLI
to create applications, configure channels, gather logs, attach TLS
certificates, and other commands you’d expect to use with a PaaS.

Hippo provides a web interface for users to register new accounts, access their
applications, and create new environments for testing.

Hippo makes it easy to run WebAssembly applications and services at scale.

## Documentation

If you're new to the project, get started with [the
introduction](https://docs.hippofactory.dev/intro/). For more in-depth
information about Hippo, plunge right into the [topic
guides](https://docs.hippofactory.dev/topics/).

Looking for the developer guide? [Start
here](https://docs.hippofactory.dev/developers).

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
