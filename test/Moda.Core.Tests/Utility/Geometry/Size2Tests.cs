// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using System.Collections.Generic;
using System.Numerics;
using FluentAssertions;
using Moda.Core.Tests.Support;
using Moda.Core.Utility.Geometry;
using NUnit.Framework;

namespace Moda.Core.Tests.Utility.Geometry;

[TestFixture]
public class Size2Tests
{
    [Test]
    public void ConstructorShouldInitializeValues()
    {
        Size2 size = new(10, 20);
        size.Width.Should().Be(10);
        size.Height.Should().Be(20);
    }


    [Test]
    public void ToVectorShouldSetXAndYToWidthAndHeight()
    {
        Size2 size = new(13, 27);
        Vector2 result = size.ToVector2();
        result.X.Should().Be(13);
        result.Y.Should().Be(27);
    }


    [Test]
    public void AdditionOperatorShouldAddWidthsAndHeights()
    {
        Size2 sizeA = new(10, 15);
        Size2 sizeB = new(3, 6);
        Size2 result = sizeA + sizeB;
        result.Width.Should().Be(13);
        result.Height.Should().Be(21);
    }
    
    [Test]
    public void SubtractionOperatorShouldSubtractWidthsAndHeights()
    {
        Size2 sizeA = new(100, 150);
        Size2 sizeB = new(30, 60);
        Size2 result = sizeA - sizeB;
        result.Width.Should().Be(70);
        result.Height.Should().Be(90);
    }
    
    // Equality Tests
    //----------------------------------------------------------------------------------------------
    
    [TestCaseSource(nameof(GetEqualityData))]
    public void EqualityTests(Size2 objectA, Size2 objectB,
        Boolean areEqual)
    {
        EqualityTesting.TestEqualsAndGetHashCode(objectA, objectB, areEqual);
    }
    
    [TestCaseSource(nameof(GetEqualityData))]
    public void EqualityOperatorTests(Size2 objectA, Size2 objectB,
        Boolean areEqual)
    {
        (objectA == objectB).Should().Be(areEqual);
        (objectA != objectB).Should().Be(!areEqual);
    }
    
    public static IEnumerable<TestCaseData> GetEqualityData() => new TestCaseData[]
        {
            new(new Size2(10, 20), new Size2(10, 20), true),
            new(new Size2(1.178f, 3.456f), new Size2(1.178f, 3.456f), true),
            new(new Size2(10, 20), new Size2(10, 30), false),
            new(new Size2(5, 20), new Size2(10, 20), false),
        };
}
