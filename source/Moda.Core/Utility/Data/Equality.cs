// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System.Diagnostics.CodeAnalysis;
using Optional;

namespace Moda.Core.Utility.Data;

/// <summary>
///     Provides a standard approach to equality comparison via helpful, static functions.
/// </summary>
/// <remarks>
///     In addition to offering <see cref="NullSafeEquals{T}"/>, a general purpose method for
///     inline equality checking, this class also serves as a standard way to evaluate equality for
///     value and reference types while implementing <see cref="IEquatable{T}"/>.
/// 
///     The names of the functions reflect which instance method they should be called from.
///     This ensures that the correct boiler plate code is called (for example, type checking
///     and casting for <see cref="Object.Equals(Object)"/>) and prevents duplicate code that
///     would need to be maintained across each equality function, and in each type.
///      
///     The recommended pattern is as follows:
///         1. Implement <see cref="IEquatable{T}"/>
///         2. In <see cref="IEquatable{T}.Equals(T)"/>, call
///            <see cref="CheckFromIEquatable{T}(T, T, Func{T, Object}[])"/>.
///            Pass in the calling object (this) and the other object. Include all properties
///            that you wish to be compared.
///         3. Override <see cref="Object.Equals(Object)"/>
///         4. In <see cref="Object.Equals(Object)"/>, call
///            <see cref="CheckFromOverride{T}(T, Object)"/>.
///            Pass in the calling object (this) and the other object.  This method will
///            ultimately relay the call to the instances <see cref="IEquatable{T}.Equals(T)"/>
///            method.
///         5. For value types, implement the == operator. For reference types, skip to step 8.
///         6. In the == operator method, call <see cref="CheckFromOperator{T}(T, T)"/>.
///            Pass in the calling object (this) and the other object.  This method will
///            ultimately relay the call to the instances <see cref="IEquatable{T}.Equals(T)"/>
///            method.
///         7. Implement the != operator with the following:
///                 return !(objectA == objectB);
///            OR
///                 return the negated value from <see cref="CheckFromOperator{T}(T, T)"/>
///                 directly.
///         8. Override <see cref="Object.GetHashCode"/> if the object is immutable. See
///            <see cref="Hash"/>.
/// </remarks>
public static class Equality
{
    /// <summary>
    ///     Call from <see cref="IEquatable{T}.Equals(T)"/> to check if two objects are the
    ///     same or equal.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the objects
    /// </typeparam>
    /// <param name="this">
    ///     The first instance
    /// </param>
    /// <param name="other">
    ///     The second instance
    /// </param>
    /// <param name="includedProperties">
    ///     A collection of delegates used to get the value of any properties to include
    ///     during the equality check.
    /// </param>
    /// <returns>
    ///     True if the two objects are determined to be equal.
    /// </returns>
    public static Boolean CheckFromIEquatable<T>(T? @this, T? other,
        params Func<T, Object>[] includedProperties)
    {
        return NullAndReferenceCheck(@this, other)
            .ValueOr(() => AreValuesEqual(@this!, other!, includedProperties));
    }

    /// <summary>
    ///     Call from <see cref="Object.Equals(Object)"/> to check if two objects are the
    ///     same or equal.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the objects
    /// </typeparam>
    /// <param name="this">
    ///     The first instance
    /// </param>
    /// <param name="other">
    ///     The second instance
    /// </param>
    /// <remarks>
    ///     Redirects to <see cref="IEquatable{T}.Equals(T)"/>
    /// </remarks>
    public static Boolean CheckFromOverride<T>(T? @this, Object? other)
        where T : IEquatable<T>
    {
        if (other is T otherAsT)
        {
            return NullAndReferenceCheck(@this, otherAsT)
                .ValueOr(() => @this!.Equals(otherAsT));
        }
        else if (ReferenceEquals(@this, null) && ReferenceEquals(other, null))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Call from the equality operator (==) to check if two objects are the same or equal.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the objects
    /// </typeparam>
    /// <param name="objectA">
    ///     The first instance
    /// </param>
    /// <param name="objectB">
    ///     The second instance
    /// </param>
    /// <remarks>
    ///     Redirects to <see cref="IEquatable{T}.Equals(T)"/>
    /// </remarks>
    public static Boolean CheckFromOperator<T>(T? objectA, T? objectB)
        where T : IEquatable<T>
    {
        return NullAndReferenceCheck(objectA, objectB)
            .ValueOr(() => objectA!.Equals(objectB));
    }


    /// <summary>
    ///     Call this to check equality when either object may be null.
    /// </summary>
    /// <param name="objectA">
    ///     The first object
    /// </param>
    /// <param name="objectB">
    ///     The second object
    /// </param>
    /// <returns>
    ///     True if the two objects are equal.
    /// </returns>
    public static Boolean NullSafeEquals<T>(T? objectA, T? objectB)
    {
        return NullAndReferenceCheck(objectA, objectB)
            .ValueOr(() => objectA!.Equals(objectB));
    }


    /// <summary>
    ///     Boiler plate check to see if the two objects meet some initial equality criteria.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the objects.
    /// </typeparam>
    /// <param name="objectA">
    ///     The first object
    /// </param>
    /// <param name="objectB">
    ///     The second object
    /// </param>
    /// <returns>
    ///     True if the objects are the same instance, or are both null.
    ///     False if the only one of the objects are null.
    ///     <see cref="Option.None{Boolean}"/> otherwise.
    /// </returns>
    /// <remarks>
    ///     Optional pattern is used here as the method will only return a result if it can.
    ///     If the two objects are separate instances and not null, the None return will allow
    ///     further handling (such as comparing fields, etc) by the calling code.
    /// </remarks>
    private static Option<Boolean> NullAndReferenceCheck<T>(T? objectA, T? objectB)
    {
        if (!typeof(T).IsValueType)
        {
            if (ReferenceEquals(objectA, objectB))
            {
                // same instance, or both are null
                return true.Some();
            }
            else if ((ReferenceEquals(objectA, null) && !ReferenceEquals(objectB, null)) ||
                (ReferenceEquals(objectB, null) && !ReferenceEquals(objectA, null)))
            {
                // only one object is null, so they can't be equal
                return false.Some();
            }
        }

        // they are different instances, so don't make an evaluation here
        return Option.None<Boolean>();
    }


    /// <summary>
    ///     Get each value from both objects and compare them.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the objects.
    /// </typeparam>
    /// <param name="objectA">
    ///     The first object
    /// </param>
    /// <param name="objectB">
    ///     The second object
    /// </param>
    /// <param name="getters">
    ///     A collection of delegates used to get the value of any properties to include
    ///     during the equality check.
    /// </param>
    /// <returns>
    ///     True if all of the values returned by the getters are the equal for both objects.
    ///     False if any one set of values is not equal.
    /// </returns>
    private static Boolean AreValuesEqual<T>(T objectA, T objectB,
        IEnumerable<Func<T, Object>> getters)
    {
        foreach (Func<T, Object> getter in getters)
        {
            if (!NullSafeEquals(getter(objectA), getter(objectB)))
            {
                return false;
            }
        }

        return true;
    }
}