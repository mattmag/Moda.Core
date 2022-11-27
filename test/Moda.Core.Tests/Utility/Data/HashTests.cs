// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using FluentAssertions;
using Moda.Core.Utility.Data;
using Moq;
using NUnit.Framework;

namespace Moda.Core.Tests.Utility.Data;

[TestFixture]
public class HashTests
{
    //  Standard Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void StandardHashShouldReturnSameResultForEqualSets()
    {
        Int32 valueA = Hash.Standard(32, "ABC", 18.0f);
        Int32 valueB = Hash.Standard(32, "ABC", 18.0f);
        valueA.Should().Be(valueB);
    }

    [Test]
    public void StandardHashShouldReturnDifferentResultsForUnequalSets()
    {
        Int32 valueA = Hash.Standard(32, "ABC", 18.0f);
        Int32 valueB = Hash.Standard(16, "DEF", 13.0f);
        valueA.Should().NotBe(valueB);
    }

    [Test]
    public void StandardHashShouldReturnDifferentResultsForEqualButOutOfOrderSets()
    {
        Int32 valueA = Hash.Standard(32, "ABC", 18.0f);
        Int32 valueB = Hash.Standard("ABC", 18.0f, 32);
        valueA.Should().NotBe(valueB);
    }

    [Test]
    public void StandardHashShouldHandleNullValuesForEqualSets()
    {
        Int32 valueA = Hash.Standard(32, null, 18.0f);
        Int32 valueB = Hash.Standard(32, null, 18.0f);
        valueA.Should().Be(valueB);
    }

    [Test]
    public void StandardHashShouldHandleNullValuesForUnequalSets()
    {
        Int32 valueA = Hash.Standard(32, null, 18.0f);
        Int32 valueB = Hash.Standard(16, null, 13.0f);
        valueA.Should().NotBe(valueB);
    }

    [Test]
    public void StandardHashShouldCallGetHashCode()
    {
        Mock<Object> mock = new();
        mock.Setup(a => a.GetHashCode()).Verifiable();
        
        Hash.Standard(32, mock.Object, 18.0f);

        mock.Verify(a => a.GetHashCode(), Times.Once);
    }

    //  Commutative Tests
    //------------------------------------------------------------------------------------------
    
    [Test]
    public void CommutativeHashShouldReturnSameResultForEqualSets()
    {
        Int32 valueA = Hash.Commutative(32, "ABC", 18.0f);
        Int32 valueB = Hash.Commutative(32, "ABC", 18.0f);
        valueA.Should().Be(valueB);
    }

    [Test]
    public void CommutativeHashShouldReturnDifferentResultsForUnequalSets()
    {
        Int32 valueA = Hash.Commutative(32, "ABC", 18.0f);
        Int32 valueB = Hash.Commutative(16, "DEF", 13.0f);
        valueA.Should().NotBe(valueB);
    }

    [Test]
    public void CommutativeHashShouldReturnSameResultsForEqualButOutOfOrderSets()
    {
        Int32 valueA = Hash.Commutative(32, "ABC", 18.0f);
        Int32 valueB = Hash.Commutative("ABC", 18.0f, 32);
        valueA.Should().Be(valueB);
    }

    [Test]
    public void CommutativeHashShouldHandleNullValuesForEqualSets()
    {
        Int32 valueA = Hash.Commutative(32, null, 18.0f);
        Int32 valueB = Hash.Commutative(32, null, 18.0f);
        valueA.Should().Be(valueB);
    }

    [Test]
    public void CommutativeHashShouldHandleNullValuesForUnequalSets()
    {
        Int32 valueA = Hash.Commutative(32, null, 18.0f);
        Int32 valueB = Hash.Commutative(16, null, 13.0f);
        valueA.Should().NotBe(valueB);
    }

    [Test]
    public void CommutativeHashShouldCallGetHashCode()
    {
        Mock<Object> mock = new Mock<Object>();
        mock.Setup(a => a.GetHashCode()).Verifiable();
        
        Hash.Commutative(32, mock.Object, 18.0f);

        mock.Verify(a => a.GetHashCode(), Times.Once);
    }
}
