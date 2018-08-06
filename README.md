# Kongverge

A Desired State Configuration tool for Kong.

A command-line tool written in cross-platform [.NET Core 2.1](http://dot.net).

[Tutorial](Tutorial.md)

## Kong DTOs

Kongverge uses several DTOs to read from files and write to Kong (and vice versa). For simplicity, the field names on these objects generally match what is present in Kong. See `KongConfiguration`, `KongRoute`, `KongService` which are used to serialise these kong concepts.

These objects also handle matching - i.e. reconciling the state described by files with the state in Kong, and performing actions in Kong as needed to make them the same. The possible cases for these objects are:

* Existing unchanged; no action is required.
* Changed; action is required to update it in place.
* New; the object needs to be added.
* Deleted; the object needs to be removed.

Kong's plugin model is more complex, as each plugin has its own set of properties used to configure it.  Therefor each Kong plugin has it's own classes,
for config and serialisation, which inherit from `KongPluginBase<TConfig>` and `IKongPluginConfig`. These have to match what is read from Kong and from file.

e.g. the Plugin `rate-limiting-advanced` has classes `RateLimitingPlugin` and `RateLimitingConfig`.

The C# objects describe the serialisation, matching and update of a Kong plugin. The plugin itself is lua code. If you want new plugin functionality, you will need to write lua to do the plugin's work. To get Kongverge to handle it, you then make sure that the C# object correctly serialises and updates it.
