// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

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
    
}
