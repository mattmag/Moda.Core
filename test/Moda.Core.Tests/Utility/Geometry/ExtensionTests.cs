// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using System.Collections.Generic;
using System.Numerics;
using FluentAssertions;
using Moda.Core.Utility.Geometry;
using NUnit.Framework;

namespace Moda.Core.Tests.Utility.Geometry;

[TestFixture]
public class ExtensionTests
{
    // Vector2.CrossProduct() Tests
    //----------------------------------------------------------------------------------------------
    
    [TestCaseSource(nameof(Get2DCrossProductTestCases))]
    public void Vector2DCrossProductShouldProduceCorrectResults(Vector2 vectorA, Vector2 vectorB,
        Single expectedResult)
    {
        vectorA.CrossProduct(vectorB).Should().Be(expectedResult);
    }
    
    [TestCaseSource(nameof(Get2DCrossProductTestCasesFor3DComparison))]
    public void Vector2DCrossProductShouldProduceSameResultsAsVector3Z(Vector2 vectorA, Vector2 vectorB)
    {
        vectorA.CrossProduct(vectorB).Should()
            .Be(Vector3.Cross(new(vectorA, 0), new(vectorB, 0)).Z);
    }


    public static IEnumerable<TestCaseData> Get2DCrossProductTestCases()
    {
        yield return new(new Vector2(2, 3), new Vector2(4, 5), -2);
        yield return new(new Vector2(-3, 16), new Vector2(85, -19), -1303);
        yield return new(new Vector2(65, 100), new Vector2(90, 80), -3800);
        yield return new(new Vector2(-8, -3), new Vector2(-5, -6), 33);
    }


    public static IEnumerable<TestCaseData> Get2DCrossProductTestCasesFor3DComparison()
    {
        yield return new(new Vector2(78, 63), new Vector2(2, -5));
        yield return new(new Vector2(49, 38), new Vector2(53, 86));
        yield return new(new Vector2(-29, 42), new Vector2(-41, -93));
        yield return new(new Vector2(909, -901), new Vector2(-460, 741));
    }


    // Side.Opposite() Tests
    //----------------------------------------------------------------------------------------------


    [Test]
    public void SideOppositeShouldReturnOppositeSide()
    {
        Side.Left.Opposite().Should().Be(Side.Right);
        Side.Right.Opposite().Should().Be(Side.Left);
        Side.Top.Opposite().Should().Be(Side.Bottom);
        Side.Bottom.Opposite().Should().Be(Side.Top);
    }


    [Test]
    public void SideOppositeShouldThrowArgumentExceptionForUnkownValue()
    {
        ((Side)988).Invoking(a => a.Opposite()).Should().Throw<ArgumentException>();
    }
}
