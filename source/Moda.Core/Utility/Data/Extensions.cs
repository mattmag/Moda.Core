// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Optional;

namespace Moda.Core.Utility.Data;

/// <summary>
///     Adds useful methods to various data structures.
/// </summary>
public static class Extensions
{
    // Dictionaries
    //----------------------------------------------------------------------------------------------

    /// <summary>
    ///     If a value exists associated with the specified key, return it, otherwise, add the
    ///     result of <paramref name="create"/> to the dictionary and return it.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of the dictionary's Key
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of the dictionary's Values
    /// </typeparam>
    /// <param name="dict">
    ///     The dictionary to act on.
    /// </param>
    /// <param name="key">
    ///     The key check or add to the dictionary.
    /// </param>
    /// <param name="create">
    ///     The function to use when creating a new instance of TValue
    /// </param>
    /// <returns>
    ///     The new or existing value.
    /// </returns>
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict,
        TKey key, Func<TValue> create)
    {
        if (!dict.TryGetValue(key, out TValue? val))
        {
            val = create();
            dict.Add(key, val);
        }

        return val;
    }


    /// <summary>
    ///     If a value exists associated with the specified key, return it, add an instance created
    ///     via new() to the dictionary and return it.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of the dictionary's Key.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of the dictionary's Values.
    /// </typeparam>
    /// <param name="dict">
    ///     The dictionary to act on.
    /// </param>
    /// <param name="key">
    ///     The key check or add to the dictionary.
    /// </param>
    /// <returns>
    ///     The new or existing value.
    /// </returns>
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict,
        TKey key)
        where TValue : new()
    {
        return GetOrAdd(dict, key, () => new TValue());
    }


    /// <summary>
    ///     Attempt to get multiple values out of a dictionary by key.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of the dictionary's Keys.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of the dictionary's Values.
    /// </typeparam>
    /// <param name="dict">
    ///     The dictionary to act on.
    /// </param>
    /// <param name="keys">
    ///     The keys to retrieve values for.
    /// </param>
    /// <returns>
    ///     An enumeration of <see cref="KeyValuePair{TKey, TValue}"/> instances for each key that
    ///     was found.
    /// </returns>
    public static IEnumerable<KeyValuePair<TKey, TValue>> TryGetMultiple<TKey, TValue>(
            this IDictionary<TKey, TValue> dict, params TKey[] keys)
    {
        return TryGetMultiple(dict, (IEnumerable<TKey>)keys);
    }
    
    /// <summary>
    ///     Attempt to get multiple values out of a dictionary by key.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of the dictionary's Keys.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of the dictionary's Values.
    /// </typeparam>
    /// <param name="dict">
    ///     The dictionary to act on.
    /// </param>
    /// <param name="keys">
    ///     The keys to retrieve values for.
    /// </param>
    /// <returns>
    ///     An enumeration of <see cref="KeyValuePair{TKey, TValue}"/> instances for each key that
    ///     was found.
    /// </returns>
    public static IEnumerable<KeyValuePair<TKey, TValue>> TryGetMultiple<TKey, TValue>(
            this IDictionary<TKey, TValue> dict, IEnumerable<TKey> keys)
    {
        foreach (TKey key in keys)
        {
            if (dict.TryGetValue(key, out TValue? val))
            {
                yield return new(key, val);
            }
        }
    }

    /// <summary>
    ///     Adds a series of items to the dictionary.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of the dictionary's Key.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of the dictionary's Values.
    /// </typeparam>
    /// <param name="dict">
    ///     The dictionary to add values to.
    /// </param>
    /// <param name="values">
    ///     They key/value pairs to be added.
    /// </param>
    /// <exception cref="KeysAlreadyExistException">
    ///     If any keys in <paramref name="values"/> collection already exist, a
    ///     <see cref="KeysAlreadyExistException"/> will be thrown.  Valid key/value pairs will
    ///     still be added. 
    /// </exception>
    public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dict,
        params KeyValuePair<TKey, TValue>[] values)
    {
        AddRange(dict, (IEnumerable<KeyValuePair<TKey, TValue>>)values);
    }

    /// <summary>
    ///     Adds a series of items to the dictionary.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of the dictionary's Key.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of the dictionary's Values.
    /// </typeparam>
    /// <param name="dict">
    ///     The dictionary to add values to.
    /// </param>
    /// <param name="values">
    ///     They key/value pairs to be added.
    /// </param>
    /// <exception cref="KeysAlreadyExistException">
    ///     If any keys in <paramref name="values"/> collection already exist, a
    ///     <see cref="KeysAlreadyExistException"/> will be thrown.  Valid key/value pairs will
    ///     still be added. 
    /// </exception>
    public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dict,
        IEnumerable<KeyValuePair<TKey, TValue>> values)
    {
        List<string> keysOfFailedAdditions = new();
        foreach ((TKey key, TValue value) in values)
        {
            if (dict.ContainsKey(key))
            {
                keysOfFailedAdditions.Add(key?.ToString() ?? String.Empty);
            }
            else
            {
                dict.Add(key, value);
            }
        }

        if (keysOfFailedAdditions.Any())
        {
            throw new KeysAlreadyExistException(keysOfFailedAdditions);
        }
    }
    
    
    /// <summary>
    ///     Adds a series of items to the dictionary, or, if any of them exist by key, update their
    ///     value.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of the dictionary's Key.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of the dictionary's Values.
    /// </typeparam>
    /// <param name="dict">
    ///     The dictionary to add values to.
    /// </param>
    /// <param name="values">
    ///     They key/value pairs to be added, or updated.
    /// </param>
    public static void AddOrUpdateRange<TKey, TValue>(this IDictionary<TKey, TValue> dict,
        params KeyValuePair<TKey, TValue>[] values)
    {
        AddOrUpdateRange(dict, (IEnumerable<KeyValuePair<TKey, TValue>>)values);
    }
    
    /// <summary>
    ///     Adds a series of items to the dictionary, or, if any of them exist by key, update their
    ///     value.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of the dictionary's Key.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of the dictionary's Values.
    /// </typeparam>
    /// <param name="dict">
    ///     The dictionary to add values to.
    /// </param>
    /// <param name="values">
    ///     They key/value pairs to be added, or updated.
    /// </param>
    public static void AddOrUpdateRange<TKey, TValue>(this IDictionary<TKey, TValue> dict,
        IEnumerable<KeyValuePair<TKey, TValue>> values)
    {
        foreach ((TKey key, TValue value) in values)
        {
            dict[key] = value;
        }
    }
    
    
    /// <summary>
    ///     Syntactic alternative to created a new <see cref="KeyValuePair{TKey, TValue}"/>
    /// </summary>
    /// <typeparam name="TKey">
    ///     The <see cref="Type"/> of the key.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The <see cref="Type"/> of the value.
    /// </typeparam>
    /// <param name="val">
    ///     The value.
    /// </param>
    /// <param name="key">
    ///     The key.
    /// </param>
    /// <returns>
    ///     A new <see cref="KeyValuePair{TKey, TValue}"/> instance with the specified key and
    ///     value.
    /// </returns>
    public static KeyValuePair<TKey, TValue> KeyedOn<TKey, TValue>(this TValue val, TKey key)
    {
        return new(key, val);
    }
    


    // Collections
    //----------------------------------------------------------------------------------------------
    
    /// <summary>
    ///     Add an value to a collection if it is not <see cref="Option.None{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the value stored in the option and in the collection.
    /// </typeparam>
    /// <param name="collection">
    ///     The collection to add to.
    /// </param>
    /// <param name="value">
    ///     The optional value.
    /// </param>
    /// <seealso cref="IfSomeAddTo{T}"/>
    /// <remarks>
    ///     Allows for a nicer syntax and/or continuation of a fluent-style call chain.
    /// </remarks>
    public static void AddIfSome<T>(this ICollection<T> collection, Option<T> value)
    {
        value.MatchSome(a => collection.Add(a));
    }

    /// <summary>
    ///     Add an value to a collection if it is not <see cref="Option.None{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the value stored in the option and in the collection.
    /// </typeparam>
    /// <param name="value">
    ///     The optional value.
    /// </param>
    /// <param name="collection">
    ///     The collection to add to.
    /// </param>
    /// <seealso cref="AddIfSome{T}(ICollection{T}, Option{T})"/>
    /// <remarks>
    ///     Allows for a nicer syntax and/or continuation of a fluent-style call chain.
    /// </remarks>
    public static void IfSomeAddTo<T>(this Option<T> value, ICollection<T> collection)
    {
        value.MatchSome(a => collection.Add(a));
    }


    

}
