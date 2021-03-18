# dotnex

![.NET Tool Release](https://github.com/piraces/dotnex/workflows/.NET%20Tool%20Release/badge.svg)
![Nuget](https://img.shields.io/nuget/v/dotnex)

A simple .NET tool to execute other dotnet tools without installing them globally or in a project (a similar approach to [npx](https://www.npmjs.com/package/npx) from [npm](https://www.npmjs.com/)).

[**View in Nuget.org**](https://www.nuget.org/packages/dotnex/)

## About

This simple tool provides the minimum necessary to run dotnet tools without the need of installing them globally or in a project, since this is not yet supported in dotnet cli.


### Features:
- **Cache**. This tool provides caching of used tools in a temporary directory (for better performance).

- **Version and framework selection**. You can specify whatever version you want and the target framework to use in every run.

- **SourceLink for debugging**. The binaries can be debbuged with [Source Link](https://github.com/dotnet/sourcelink). Example (with Developer Command Prompt for VS): `devenv /debugexe c:\Users\rich\.dotnet\tools\dotnex.exe`



## Usage
```
dotnex [options] <TOOL> [<TOOL-ARGS>...]
```

Where the arguments and options are the following:
```
Arguments:
  <TOOL>         The NuGet Package Id of the tool to execute
  <TOOL-ARGS>    Arguments to pass to the tool to execute

Options:
  -v, --version <version>        Version of the tool to use
  -f, --framework <framework>    Target framework for the tool
  -r, --remove-cache             Flag to remove the local cache before running the tool (can be run without tool)
  -?, -h, --help                 Show help and usage information
```

This same output can be obtained running the tool with the help option:
```
dotnex -h
```

Example with the simple `dotnetsay` tool:
```
dotnex dotnetsay Hello World!!
```

## Installation

Install the [dotnet cli](https://dotnet.microsoft.com/download) (included in the .NET SDK) and then run the following command:

```
dotnet tool install -g dotnex
```f

### SDK version

`dotnex` is actually built for .NET 5. The main idea is to maintain `dotnex` up-to-date with the current, most stable release of dotnet recommended to use at the date by Microsoft.

## Contributions

Feel free to open an issue or a PR if you want to without any problem :)

## License

This project is licensed under the [MIT License](./LICENSE).

See the `LICENSE` file in the root of this repository.
