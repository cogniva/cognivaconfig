using System.Reflection;
using System.Resources;

namespace Cogniva.Configuration;

public static class ConfigConstantExtensions
{
    public static ItemCollection ExtractAllConfigItems(this Type type)
    {
        var lookup = type.LocalisedDescriptionLookup();
        var allFields = type.GetFields(BindingFlags.Static | BindingFlags.Public).ToList();
        var enumerable = allFields.Select(field => field.ExtractItemInfo(lookup));
        return new ItemCollection(enumerable.WhereNotNull());
    }

    internal static Item? ExtractItemInfo(this FieldInfo field, Func<string, LocalisedTextFact>? descriptionLookup = null)
    {
        if (!field.IsLiteral || field.FieldType != typeof(string))
        {
            return null;
        }

        var name = field.GetConstantStringValue();
        var itemFacts = GetItemFacts(field);
        if (descriptionLookup != null)
        {
            var descriptionFact = descriptionLookup.Invoke(field.Name);
            if (descriptionFact != null)
            {
                itemFacts = itemFacts.Concat(new[] {descriptionFact});
            }
        }
        return new Item(name, itemFacts);
    }

    internal static IEnumerable<IItemFact> GetItemFacts(this FieldInfo field)
    {
        return field.GetCustomAttributes().OfType<IItemFactAttribute>().Select(type => type.ToItemFact());
    }

    /// <summary>
    /// Creates a Func that allows you to create LocalisedTextFacts based on the key you provide. The <see
    /// cref="Type"/> provided to this function needs to have a <see cref="ResourceManagerAttribute"/> on it, which
    /// identifies the specific resource manager to use. The keys are expected to match the names of constants.
    /// </summary>
    internal static Func<string, LocalisedTextFact>? LocalisedDescriptionLookup(this Type type)
    {
        var sourceAttribute = type.GetCustomAttributes<ResourceManagerAttribute>(true).FirstOrDefault();
        if (sourceAttribute == null)
        {
            return null;
        }

        var propertyInfo = sourceAttribute.LocalisedStringProvider.GetProperty("ResourceManager", BindingFlags.Static | BindingFlags.Public);
        var localisedStringsProvider = propertyInfo?.GetValue(null, null) as ResourceManager;
        if (localisedStringsProvider == null)
        {
            return null;
        }

        return keyName =>
            new LocalisedTextFact(new ResourceManagerLocalisedText(TextLabels.Description, keyName, localisedStringsProvider));
    }
}