using System;

namespace Cogniva.Configuration;

/// <summary>
/// Non-generic base class so we can work with these things without needing to specify the type
/// </summary>
public abstract class ItemType : IItemFact
{
    public abstract string DefaultAsText { get; }
    public bool HasDefault { get; protected init; }
    public abstract Type Type { get; }
}

public class ItemType<T> : ItemType
{
    private readonly Func<string, T> _parse;

    public ItemType(Func<string, T> parse = null)
    {
        _parse = parse;
    }

    public override Type Type => typeof(T);

    private readonly T _defaultValue;

    public T Default
    {
        get => _defaultValue;
        init
        {
            _defaultValue = value;
            HasDefault = true;
        }
    }

    public override string DefaultAsText => _defaultValue?.ToString();

    public bool TryParse(string text, out T result)
    {
        if (_parse == null)
        {
            result = default;
            return false;
        }

        try
        {
            result = _parse(text);
        }
        catch
        {
            result = default;
            return false;
        }

        return true;
    }
}

// These classes are just convenient ways to wrap up creating ItemType<T> types with common types and default parse
// methods. In .NET 7 and later, we'll be able to switch this to use the IParsable<TSelf>, which will let us make
// default parsing available for most of these things and let us do away with these temporary convenience types.
// Basically: Don't make 'em public!
internal class IntType : ItemType<int>
{
    public IntType() : base(int.Parse) {}
}

internal class BooleanType : ItemType<bool>
{
    public BooleanType() : base(bool.Parse) { }
}

internal class StringType : ItemType<string>
{
    public StringType() : base(originalValue => originalValue) { }
}