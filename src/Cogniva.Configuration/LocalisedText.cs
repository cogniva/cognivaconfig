using System;
using System.Collections.Generic;
using System.Globalization;

namespace Cogniva.Configuration;

public interface ILocalisedText
{
    /// <summary>
    /// A label that can be used to identify what text you're looking at. In most cases, will probably be the same as
    /// the key name in a resource dictionary.
    /// </summary>
    string TextLabel { get; }

    /// <summary>
    /// Looks up the translation based on the provided language name (typically a two-letter ISO language name).
    /// </summary>
    string For(string languageName);

    /// <summary>
    /// Looks up the translation based on the provided culture.
    /// </summary>
    string For(CultureInfo culture);
}

/// <summary>
/// Provides a way to store various translations of a piece of text, and to look up the appropriate one based on the
/// supplied two-letter ISO language name (from <see cref="CultureInfo.TwoLetterISOLanguageName"/>)
/// </summary>
public class LocalisedText : ILocalisedText
{
    private readonly string _defaultText;
    private readonly Dictionary<string, string> _localisedTexts;

    /// <summary>
    /// Creates the localised text object
    /// </summary>
    /// <param name="textLabel">
    /// A label that can be used to identify what text you're looking at. In most cases, will probably be the same as
    /// the key name in a resource dictionary.
    /// </param>
    /// <param name="defaultText">The default text to use if the user asks for a culture that can't be found</param>
    /// <param name="localisedTexts">
    /// A dictionary mapping culture names to the appropriate localised text.
    /// </param>
    /// <remarks>
    /// <para>
    /// This probably looks a lot like reinventing the wheel, since ResourceDictionary already supports better
    /// versions of this kind of functionality, but separating it out into another type will let us handle cases where
    /// the resource manager isn't available, and also to transmit it across systems.
    /// </para>
    /// <para>
    /// Consumers should use this via <see cref="ILocalisedText"/> rather than this concrete class where possible,
    /// because there is also an implementation that directly uses a resource manager.
    /// </para>
    /// </remarks>
    public LocalisedText(string textLabel, string defaultText, IDictionary<string, string> localisedTexts)
    {
        _defaultText = defaultText;
        _localisedTexts = new Dictionary<string, string>(localisedTexts);
        TextLabel = textLabel;
    }

    /// <inheritdoc />
    public string TextLabel { get; }

    /// <inheritdoc />
    public string For(string languageName)
    {
        if (string.IsNullOrEmpty(languageName))
        {
            throw new ArgumentException("A culture name (such as from TwoLetterISOLanguageName) must be supplied",
                nameof(languageName));
        }

        if (_localisedTexts.TryGetValue(languageName, out var localisedText))
        {
            return localisedText;
        }

        return _defaultText;
    }

    /// <inheritdoc />
    public string For(CultureInfo culture)
    {
        return For(culture.TwoLetterISOLanguageName);
    }
}

/// <summary>
/// Commonly-used labels for text we want to be able to look up
/// </summary>
public static class TextLabels
{
    public const string Description = nameof(Description);
}