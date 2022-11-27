// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using FluentAssertions;
using NUnit.Framework;

namespace Moda.Core.Tests.Support;

/// <summary>
///     Provides helper methods for testing value equality.
/// </summary>
public static class EqualityTesting
{
    /// <summary>
    ///     Ensure that the <see cref="object.Equals(object)"/> and
    ///     <see cref="IEquatable{T}.Equals(T)"/> methods return the expected results when
    ///     comparing the two provided objects.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the objects being compared.
    /// </typeparam>
    /// <param name="objectA">
    ///     The first object.
    /// </param>
    /// <param name="objectB">
    ///     The second object.
    /// </param>
    /// <param name="areEqual">
    ///     Indicates if the objects are expected to be equal or not.
    /// </param>
    /// <remarks>
    ///     Equals and GetHashCode are tested in the same method to limit user error while
    ///     writing tests (i.e., covering one but not the other)
    ///     
    ///     Operator equality (== and !=) are not covered here due to limitations of the
    ///     language (generics cant constrain to static operators and runtime overloading
    ///     with boxed types).
    /// </remarks>
    public static void TestEqualsAndGetHashCode<T>(T objectA, T objectB, Boolean areEqual)
        where T : IEquatable<T>
    {
        Assume.That(objectA, Is.Not.Null);
        Assume.That(objectB, Is.Not.Null);
        
        if (areEqual)
        {
            ObjectEqualsShouldReturnTrueIfObjectsAreEqual(objectA, objectB);
            IEquatableEqualsShouldReturnTrueIfObjectsAreEqual(objectA, objectB);
            GetHashCodeShouldReturnSameValueForEqualInstances(objectA, objectB);
        }
        else
        {
            ObjectEqualsShouldReturnFalseIfObjectsAreNotEqual(objectA, objectB);
            IEquatableEqualsShouldReturnFalseIfObjectsAreNotEqual(objectA, objectB);
            GetHashCodeShouldLikelyReturnDifferentValueForUneqlEqualInstances(objectA, objectB);
        }
    }
    

    private static void ObjectEqualsShouldReturnTrueIfObjectsAreEqual(Object objectA,
        Object objectB)
    {
        objectA.Equals(objectB).Should().BeTrue();
    }

    private static void ObjectEqualsShouldReturnFalseIfObjectsAreNotEqual(Object objectA,
        Object objectB)
    {
        objectA.Equals(objectB).Should().BeFalse();
    }

    private static void IEquatableEqualsShouldReturnTrueIfObjectsAreEqual<T>(T objectA,
        T objectB)
        where T : IEquatable<T>
    {
        objectA.Equals(objectB).Should().BeTrue();
    }

    private static void IEquatableEqualsShouldReturnFalseIfObjectsAreNotEqual<T>(T objectA,
        T objectB)
        where T : IEquatable<T>
    {
        objectA.Equals(objectB).Should().BeFalse();
    }

    private static void GetHashCodeShouldReturnSameValueForEqualInstances<T>(T objectA,
        T objectB)
    {
        objectA!.GetHashCode().Should().Be(objectB!.GetHashCode());
    }
    

    /// <remarks>
    ///     Note that this is just a guideline - there is no requirement that two different
    ///     instances that are not equal need to generate a unique hash code - the hash
    ///     function should just do its best to return two different values for two different
    ///     instances.
    ///     
    ///     This test is mostly in place just to ensure that GetHashCode isn't returning 0 by
    ///     default or something silly like that, as that case would not be picked up by
    ///     <see cref="GetHashCodeShouldReturnSameValueForEqualInstances{T}"/>. 
    ///     
    ///     If you have a relatively good hash function and this test fails, you likely just
    ///     found a collision  and can alter your instance data to something that won't provide
    ///     equal hashes.
    /// </remarks>
    private static void GetHashCodeShouldLikelyReturnDifferentValueForUneqlEqualInstances<T>(
        T objectA, T objectB)
    {
        objectA!.GetHashCode().Should().NotBe(objectB!.GetHashCode());
    }
}
