# Kongverge

A Desired State Configuration tool for Kong.

A command-line tool written in cross-platform [.NET Core 2.1](http://dot.net).

[Tutorial](Tutorial.md)

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
