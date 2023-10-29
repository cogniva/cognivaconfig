using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Cogniva.Configuration;

internal static class CommonExtensions
{
    /// <summary>
    /// Does a case- and culture-insensitive string comparison
    /// </summary>
    /// <returns>
    /// <c>true</c> if the two strings are equivalent using <see cref="StringComparison.InvariantCultureIgnoreCase"/>
    /// rules, or <c>false</c> otherwise
    /// </returns>
    public static bool CompareIgnoringCase(this string firstString, string secondString)
    {
        return string.Compare(firstString, secondString, StringComparison.InvariantCultureIgnoreCase) == 0;
    }

    /// <summary>
    /// Returns the string value of a constant.
    /// </summary>
    public static string GetConstantStringValue(this FieldInfo field)
    {
        return field.GetRawConstantValue().ToString();
    }

    /// <summary>
    /// Returns the requested string from the ResourceManager, or <c>null</c> if the string couldn't be read.
    /// </summary>
    public static string ReadStringSafe(this ResourceManager resourceManager, string fieldName, CultureInfo? cultureInfo = null)
    {
        string? description = null;
        try
        {
            if (cultureInfo == null)
            {
                description = resourceManager.GetString(fieldName);
            }
            else
            {
                description = resourceManager.GetString(fieldName, cultureInfo);
            }
        }
        catch (Exception)
        {
            // Ignore the exception
        }
        return description ?? string.Empty;
    }

    /// <summary>
    /// Filters out any null values from the <paramref name="original"/> sequence
    /// </summary>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> original)
    {
        return original.Where(item => item is not null);
    }
}