# Main site for **dotnex**

A simple .NET tool to execute other dotnet tools without installing them globally or in a project (a similar approach to [npx](https://www.npmjs.com/package/npx) from [npm](https://www.npmjs.com/)).

[**View in Nuget.org**](https://www.nuget.org/packages/dotnex/)

This simple tool provides the minimum necessary to run dotnet tools without the need of installing them globally or in a project, since this is not yet supported in dotnet cli.

## Quick Start

Install the [dotnet cli](https://dotnet.microsoft.com/download) (included in the .NET SDK) and then run the following command:

```shell
dotnet tool install -g dotnex
```

Execute your dotnet tool:

```shell
dotnex dotnetsay Easy!!
```