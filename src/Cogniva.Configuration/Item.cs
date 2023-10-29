using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Cogniva.Configuration;

public class Item
{
    private readonly List<IItemFact> _facts = new();

    public Item(string name, IEnumerable<IItemFact> facts)
    {
        Name = name;
        _facts.AddRange(facts);
    }

    public string Description => this.GetDescriptionInfo()?.For(CultureInfo.CurrentUICulture);

    public IEnumerable<IItemFact> Facts => _facts;

    public string Name { get; init; }

    public void Add(IItemFact fact)
    {
        _facts.Add(fact);
    }
}

public static class ItemExtensions
{
    /// <summary>
    /// Gets the declared type of this item (e.g. String if this item has an <see cref="ItemType{String}"/> fact).
    /// </summary>
    /// <returns>
    /// The type T of the first <see cref="ItemType{T}" /> fact found on the item, or null if none are found
    /// </returns>
    public static Type GetDeclaredType(this Item item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        return item.Facts
            .OfType<ItemType>()
            .FirstOrDefault()
            ?.Type;
    }

    /// <summary>
    /// A convenience method to get a default value for the specified type.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is null</exception>
    /// <exception cref="ArgumentException"><paramref name="item"/> doesn't have the right type, or doesn't have a default</exception>
    public static T GetDefault<T>(this Item item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        var itemTypeInfo = item.Facts.OfType<ItemType<T>>().FirstOrDefault(typeInfo => typeInfo.HasDefault);
        if (itemTypeInfo == null)
        {
            throw new ArgumentException(
                $"{item.Name} either is not of type {typeof(T).Name} or does not have a default value");
        }
        return itemTypeInfo.Default;
    }

    /// <summary>
    /// Returns the default value in string form, if any. Otherwise, returns null.
    /// </summary>
    public static string GetDefaultAsString(this Item item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        return item.Facts.OfType<ItemType>().FirstOrDefault()?.DefaultAsText;
    }

    public static string GetDescriptionFor(this Item item, string languageName)
    {
        return item.GetDescriptionInfo()?.For(languageName);
    }

    /// <summary>
    /// Gets an <see cref="ILocalisedText"/> object allowing callers to look up localised descriptions for this item,
    /// if any such info is available. If not, returns null.
    /// </summary>
    public static ILocalisedText GetDescriptionInfo(this Item item)
    {
        return item.Facts.OfType<LocalisedTextFact>().FirstOrDefault(fact => fact.Label == TextLabels.Description)?.LocalisedText;
    }

    public static bool HasDefault(this Item item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        return item.Facts.OfType<ItemType>().Any(typeInfo => typeInfo.HasDefault);
    }

    public static bool HasDefault<T>(this Item item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        return item.Facts.OfType<ItemType<T>>().Any(typeInfo => typeInfo.HasDefault);
    }

    /// <summary>
    /// Checks to see if the item has the specified flag using a case-insensitive search.
    /// </summary>
    public static bool HasFlag(this Item item, string flagName)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        return item.Facts.OfType<ItemFlag>().Any(flag => flag.Name.CompareIgnoringCase(flagName));
    }

    /// <summary>
    /// Verifies whether the item is described as being the provided type.
    /// </summary>
    public static bool Is<T>(this Item item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        return item.Facts.OfType<ItemType<T>>().Any();
    }

    public static bool Is(this Item item, Type type)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        return type.IsAssignableFrom(item.GetDeclaredType());
    }

    /// <summary>
    /// An extension method to wrap up trying to parse the value and then returning the default if the text provided
    /// can't be parsed into a value of the expected type
    /// </summary>
    /// <remarks>
    /// <para>
    /// A simple wrapper around calling <see cref="Is{T}"/>, then <see cref="TryParse{T}"/>, then <see
    /// cref="TryGetDefault{T}"/>, finally falling back to <c>default(T)</c>
    /// </para>
    /// <para>
    /// Unlike <see cref="TryParse{T}"/>, this method will throw an exception if <paramref name="item"/> isn't marked
    /// as being of type <typeparamref name="T"/>. If you want to avoid that, you can use <see
    /// cref="TryParseOrDefault{T}"/> instead.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="item" /> is null</exception>
    public static T ParseOrDefault<T>(this Item item, string text)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        if (!item.Is<T>())
        {
            throw new ArgumentException($"{item.Name} is not of type {typeof(T).Name}");
        }

        if (item.TryParse(text, out T result))
        {
            return result;
        }

        item.TryGetDefault(out result);
        return result;
    }

    public static bool TryParseOrDefault<T>(this Item item, string text, out T result)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        if (!item.Is<T>())
        {
            result = default;
            return false;
        }

        if (item.TryParse(text, out result))
        {
            return true;
        }

        return item.TryGetDefault(out result);
    }

    public static bool TryGetDefault<T>(this Item item, out T result)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        var itemTypeFact = item.Facts.OfType<ItemType<T>>().FirstOrDefault();
        if (itemTypeFact is { HasDefault: true })
        {
            result = itemTypeFact.Default;
            return true;
        }
        result = default;
        return false;
    }

    public static bool TryGetFlag(this Item item, string name, out ItemFlag flag)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        flag = item.Facts.OfType<ItemFlag>().FirstOrDefault(maybe => maybe.Name == name);
        return flag != null;
    }

    /// <summary>
    /// Tries to parse the provided text using information in this item to help.
    /// </summary>
    /// <remarks>
    /// If <paramref name="item"/> isn't marked as being of the appropriate type, this method will return <c>false</c>
    /// instead of throwing an exception, in keeping with the general convention that TryX methods should almost never
    /// throw exceptions.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is null</exception>
    public static bool TryParse<T>(this Item item, string text, out T result)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        var typeFact = item.Facts.OfType<ItemType<T>>().FirstOrDefault();
        if (typeFact is null)
        {
            result = default;
            return false;
        }

        return typeFact.TryParse(text, out result);
    }

    public static IEnumerable<Item> WithFlag(this IEnumerable<Item> items, string flagName)
    {
        if (items is null) throw new ArgumentNullException(nameof(items));

        return items.Where(item => item.HasFlag(flagName));
    }

    public static Item Named(this IEnumerable<Item> items, string name)
    {
        if (items is null) throw new ArgumentNullException(nameof(items));

        return items.FirstOrDefault(item => item.Name.CompareIgnoringCase(name));
    }

    public static IEnumerable<Item> OfType<T>(this IEnumerable<Item> items)
    {
        if (items is null) throw new ArgumentNullException(nameof(items));

        return items.Where(item => item.Is<T>());
    }
}