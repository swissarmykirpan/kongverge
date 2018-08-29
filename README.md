# Kongverge

A Desired State Configuration tool for Kong.

A command-line tool written in cross-platform [.NET Core 2.1](http://dot.net).

[Tutorial](Tutorial.md)

## Installation

Kongverge is built [as a .NET core global tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools). Builds are stored as nuget packages on the internal package server: http://packages.je-labs.com/feeds/Global/Kongverge/

You can install `kongverge` as a global tool as follows:

* First, have .NET Core 2.1 or later installed. At a commandline, `dotnet --list-runtimes` should succeed and show an item for `Microsoft.NETCore.App 2.1.0` or later.
* Install with `dotnet tool install kongverge --global --add-source http://packages.je-labs.com/nuget/Global/`.
* You should then be able to run `kongverge` from the commandline e.g. `kongverge --help`

Other operations:

* Update to latest: `dotnet tool update kongverge --global --add-source http://packages.je-labs.com/nuget/Global/`
* Uninstall: `dotnet tool uninstall kongverge --global`

This should all work on development and build machines, on windows, mac and linux, as long as .NET Core 2.1 is installed.

## Kong DTOs

Kongverge uses several DTOs to read from files and write to Kong (and vice versa). For simplicity, the field names on these objects generally match what is present in Kong. See `KongConfiguration`, `KongRoute`, `KongService` which are used to serialise these kong concepts.

These objects also handle matching - i.e. reconciling the state described by files with the state in Kong, and performing actions in Kong as needed to make them the same. The possible cases for these objects are:

* Unchanged; The object in Kong is identical to the object in config, so no action is required.
* Changed; the object in Kong is matched with an object in config, but not all of the properties are the same. Action is required to update the object in place.
* New; the object needs to be added to Kong.
* Deleted; the object needs to be removed from Kong.

Kong's plugin model is more complex, as each plugin has its own set of properties used to configure it.  Therefore each Kong plugin has it's own classes,
for config and serialisation, which inherit from `KongPluginBase<TConfig>` and `IKongPluginConfig`. These have to match what is read from Kong and from file.

e.g. the Plugin `rate-limiting-advanced` has classes `RateLimitingPlugin` and `RateLimitingConfig`.

The C# objects describe the serialisation, matching and update of a Kong plugin. The plugin itself is lua code. If you want new plugin functionality, you will need to write lua to do the plugin's work. To get Kongverge to handle it, you then make sure that the C# object correctly serialises and updates it.

For documentation on the plugin's fields and how they work, you can generally refer to the plugin's documentation on the Kong site if it's a standard plugin. E.g. [Rate Limiting Advanced Plugin Configuration Parameters](https://docs.konghq.com/enterprise/0.31-x/plugins/rate-limiting/#configuration-parameters).
