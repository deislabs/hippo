# Developer guide

This guide explains how to set up your environment for developing Hippo.

Hippo is a .NET webapp, built with the [Model-View-Controller](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-5.0&tabs=visual-studio) (MVC) approach.

The front-end uses the Bootstrap design framework, which (along with some other packages) is managed via [npm](https://www.npmjs.com/) and [gulp](https://gulpjs.com/).

## System Requirements

Install the following development tools:

- [.NET 5](https://dot.net/)
- [Node.js](https://nodejs.org/en/download/)
- [npm](https://www.npmjs.com/get-npm)

## Building

```console
$ dotnet restore
$ npm run build
```

## Running

```console
$ dotnet run
```

## Testing

```console
$ dotnet test
```
