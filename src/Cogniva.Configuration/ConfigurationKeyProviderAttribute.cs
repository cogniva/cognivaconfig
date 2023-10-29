using System;

namespace Cogniva.Configuration;

/// <summary>
/// An assembly-level attribute that allows tools to automatically find and load types that contain configuration keys.
/// </summary>
/// <remarks>
/// Most of the time you'll want all your config key types in an assembly to be in a single class, but there may be
/// situations where it makes sense to have multiple types after all. To support those possible situations, this
/// attribute can be specified multiple times on a single assembly.
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class ConfigurationKeyProviderAttribute : Attribute
{
    public ConfigurationKeyProviderAttribute(Type type)
    {
        Type = type;
    }

    public Type Type { get; }
}