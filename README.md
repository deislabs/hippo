# Project Hippo

Hippo is the easiest way to deploy and serve applications compiled to WASI.

**WARNING:** This is experimental code. It is not considered production-grade
by its developers, nor is it "supported" software.

> DeisLabs is experimenting with many WASM technologies right now. This is one
> of a multitude of projects (including
> [Krustlet](https://github.com/deislabs/krustlet)) designed to test the limits
> of WebAssembly as a cloud-based runtime.

Hippo provides an easy-to-use management portal for your application, allowing
users to deploy and roll back their application releases, edit configuration,
manage domain routing and TLS certificates, and view agrregated logs. Hippo
provides a simple permissions model to manage your users and groups.

Hippo comes integrated with a powerful Layer 7 Load Balancer so you and your
developers can spend more time writing applications, and less time managing
your load balancer. After initial setup, create an application, attach a domain
and a TLS certificate, and let Hippo handle the backend routing.

All of Hippo's functionality is exposed through a web application and a
[command line interface](https://github.com/deislabs/hippo-cli).

## Documentation

If you're new to the project, get started with [the
introduction](https://hippo-docs.netlify.app/intro/). For more in-depth
information about Hippo, plunge right into the [topic
guides](https://hippo-docs.netlify.app/topics/).

Looking for the developer guide? [Start
here](https://hippo-docs.netlify.app/developers).

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
