using System;

namespace Cogniva.Configuration;

/// <summary>
/// Provides information about where to look up localised descriptions of the keys
/// </summary>
public class ResourceManagerAttribute : Attribute
{
    public Type LocalisedStringProvider { get; set; }
}