using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Cogniva.Configuration;

public class ItemCollection : IEnumerable<Item>, INotifyCollectionChanged
{
    private readonly Dictionary<string, Item> _items = new();

    public ItemCollection() {}

    /// <summary>Creates a new collection and initialises it with the provided list</summary>
    /// <exception cref="ArgumentNullException">
    /// Thrown if any of the elements of <paramref name="items"/> is <c>null</c>
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if there are any items with duplicated names.
    /// </exception>
    public ItemCollection(IEnumerable<Item> items)
    {
        if (items is null) throw new ArgumentNullException(nameof(items));
        AddRange(items);
    }

    public IEnumerator<Item> GetEnumerator()
    {
        return _items.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable) _items).GetEnumerator();
    }

    /// <summary>
    /// Add the item to the collection
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="item"/> is <c>null</c></exception>
    /// <exception cref="ArgumentException">
    /// Thrown if the collection already contains an exception with the same name as <paramref name="item"/>.
    /// </exception>
    public void Add(Item item)
    {
        AddInternal(item);
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
    }

    private void AddInternal(Item item)
    {
        if (item is null)
            throw new ArgumentNullException(nameof(item));
        if (_items.ContainsKey(item.Name))
        {
            throw new ArgumentException($"Collection already contains an item named {item.Name}", nameof(item));
        }

        _items[item.Name] = item;
    }

    public void AddRange(IEnumerable<Item> items)
    {
        foreach (Item item in items)
        {
            AddInternal(item);
        }
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items));
    }

    public int Count => _items.Count;

    public Item this[string itemName] => _items[itemName];

    /// <summary>
    /// Checks to see if there are any items in the supplied collection that have the same name as an item already in
    /// this collection.
    /// </summary>
    /// <returns>The names of any items that appear in both this list and the target list</returns>
    public IEnumerable<string> CheckForDuplicates(IEnumerable<Item> itemsToCheck)
    {
        return itemsToCheck
            .Where(item => _items.ContainsKey(item.Name))
            .Select(item => item.Name);
    }

    public bool Remove(string name)
    {
        _items.TryGetValue(name, out var toBeRemoved);
        var removed = _items.Remove(name);
        if (removed)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, toBeRemoved));
        }
        return removed;
    }

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        CollectionChanged?.Invoke(this, e);
    }
}
