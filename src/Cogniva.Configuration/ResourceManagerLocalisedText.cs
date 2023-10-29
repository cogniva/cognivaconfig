using System.Globalization;
using System.Resources;

namespace Cogniva.Configuration;

public class ResourceManagerLocalisedText : ILocalisedText
{
    private readonly string _keyName;
    private readonly ResourceManager _resourceManager;

    public ResourceManagerLocalisedText(string textLabel, string keyName, ResourceManager resourceManager)
    {
        _keyName = keyName;
        _resourceManager = resourceManager;
        TextLabel = textLabel;
    }

    /// <inheritdoc />
    public string TextLabel { get; }

    /// <inheritdoc />
    public string For(string languageName)
    {
        return For(CultureInfo.CreateSpecificCulture(languageName));
    }

    /// <inheritdoc />
    public string For(CultureInfo culture)
    {
        return _resourceManager.ReadStringSafe(_keyName, culture);
    }
}