// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.Utility.Data;

/// <summary>
///     Used to calculate a hash code for a series of values.
/// </summary>
/// <remarks>
///     Recommended use for immutable objects is to define a private field and set it to the
///     results of either <see cref="Standard(Object[])"/> or
///     <see cref="Commutative(Object[])"/> in the constructor.
/// </remarks>
public static class Hash
{
    /// <summary>
    ///     The initial prime value to seed the calculation with
    /// </summary>
    private static readonly Int32 SEED = 487;
    
    /// <summary>
    ///     A prime value multiply by during hash code calculations
    /// </summary>
    private static readonly Int32 MODIFIER = 31;


    /// <summary>
    ///     Calculate a hash code for the specified values using the standard method.
    ///     This is likely the method that you want to use.
    /// </summary>
    /// <param name="items">
    ///     A list of items include in the calculation.
    /// </param>
    /// <returns>
    ///     The resulting hash code as a <see cref="Int32"/>.
    /// </returns>
    public static Int32 Standard(params Object?[] items)
    {
        return items.Aggregate(SEED, (current, item) =>
                (current * MODIFIER) + GetHashCode(item));
    }

    /// <summary>
    ///     Calculate a hash code for the specified values using the commutative method, which
    ///     produces a result that is order-neutral.
    /// </summary>
    /// <param name="items">
    ///     A list of items include in the calculation.
    /// </param>
    /// <remarks>
    ///     Use this when you explicitly do not want the order of the values being hashed
    ///     to count for or against the outcome.
    /// </remarks>
    /// <returns>
    ///     The resulting hash code as a <see cref="Int32"/>.
    /// </returns>
    public static Int32 Commutative(params Object?[] items)
    {
        return items.Aggregate(SEED, (current, item) => current ^ GetHashCode(item));
    }


    /// <summary>
    ///     Get the hash code for the object.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the object.
    /// </typeparam>
    /// <param name="item">
    ///     The object to calculate a hash code for.
    /// </param>
    /// <returns>
    ///     The result of item.GetHashCode, or 0 if the object is null.
    /// </returns>
    private static Int32 GetHashCode<T>(T? item)
    {
        return ReferenceEquals(item, null) ? 0 : item.GetHashCode();
    }
}