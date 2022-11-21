// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using System.Collections.Generic;
using FluentAssertions;
using Moda.Core.Support;
using NUnit.Framework;

namespace Moda.Core.Utility.Maths.Tests;

[TestFixture]
public class RangeFTests
{
    // Constructor Tests
    //----------------------------------------------------------------------------------------------
    
    [TestCase(0, 10, 0, 10)]
    [TestCase(10, 0, 0, 10)]
    [TestCase(-5, 5, -5, 5)]
    [TestCase(5, -5, -5, 5)]
    [TestCase(-100, -50, -100, -50)]
    [TestCase(-50, -100, -100, -50)]
    public void ConstructorShouldAssignMinMaxAppropriately(Single valueA, Single valueB,
        Single expectedMin, Single expectedMax)
    {
        RangeF unitUnderTest = new(valueA, valueB);
        unitUnderTest.Minimum.Should().Be(expectedMin);
        unitUnderTest.Maximum.Should().Be(expectedMax);
    }
    
    [TestCase(0, 10, 10)]
    [TestCase(-5, 5, 10)]
    [TestCase(-100, -50, 50)]
    public void DeltaShouldBeSetToAbsoluteDifferenceOfMinAndMax(Single valueA, Single valueB,
        Single expectedDelta)
    {
        RangeF unitUnderTest = new(valueA, valueB);
        unitUnderTest.Delta.Should().Be(expectedDelta);
    }


    // Clamp Tests
    //----------------------------------------------------------------------------------------------
    
    [TestCase(10, 20, 5)]
    [TestCase(-10, 10, -20)]
    public void ClampShouldReturnLowerLimitIfValueIsLess(Single min, Single max, Single value)
    {
        RangeF unitUnderTest = new(min, max);
        unitUnderTest.Clamp(value).Should().Be(min);
    }

    [TestCase(10, 20, 30)]
    [TestCase(-100, -50, -20)]
    public void ClampShouldReturnUpperLimitIfValueIsGreater(Single min, Single max, Single value)
    {
        RangeF unitUnderTest = new(min, max);
        unitUnderTest.Clamp(value).Should().Be(max);
    }

    [TestCase(10, 20, 15)]
    [TestCase(-10, 10, 0)]
    [TestCase(-100, -50, -75)]
    public void ClampShouldReturValueIfWithinRange(Single min, Single max, Single value)
    {
        RangeF unitUnderTest = new(min, max);
        unitUnderTest.Clamp(value).Should().Be(value);
    }

    [Test]
    public void ClampShouldHandleZeroDelta()
    {
        RangeF unitUnderTest = new(10, 10);
        unitUnderTest.Clamp(15).Should().Be(10);
        unitUnderTest.Clamp(5).Should().Be(10);
    }


    // Wrap Tests
    //----------------------------------------------------------------------------------------------
    
    [TestCase(0, 10, 15, 5)]
    [TestCase(0, 10, -2, 8)]
    [TestCase(0, 360, 540, 180)]
    [TestCase(0, 360, -45, 315)]
    [TestCase(-1, 1, 1.5f, -0.5f)]
    [TestCase(-1, 1, -1.5f, 0.5f)]
    [TestCase(-1, 1, 2, 0)]
    public void WrapShouldOutsideWrapValuesAroundToWithinRange(Single min, Single max,
        Single value, Single expectedResult)
    {
        RangeF unitUnderTest = new(min, max);
        unitUnderTest.Wrap(value).Should().Be(expectedResult);
    }


    [TestCase(0, 10, 45, 5)]
    [TestCase(0, 10, -45, 5)]
    [TestCase(0, 360, 1260, 180)]
    [TestCase(0, 360, -1125, 315)]
    [TestCase(-1, 1, 8.5f, 0.5f)]
    [TestCase(-1, 1, 7.5f, -0.5f)]
    [TestCase(-1, 1, -8.25f, -0.25f)]
    [TestCase(-1, 1, -7.25f, 0.75f)]
    public void WrapShouldAccountForMultipleLaps(Single min, Single max,
        Single value, Single expectedResult)
    {
        RangeF unitUnderTest = new(min, max);
        unitUnderTest.Wrap(value).Should().Be(expectedResult);
    }

    [TestCase(0, 10, 3)]
    [TestCase(0, 360, 270)]
    [TestCase(-1, 1, -0.25f)]
    [TestCase(-1, 1, 0.25f)]
    public void WrapShouldReturnValueIfWithinRange(Single min, Single max, Single value)
    {
        RangeF unitUnderTest = new(min, max);
        unitUnderTest.Wrap(value).Should().Be(value);
    }

    [TestCase(0, 360, 360, 0)]
    [TestCase(0, 360, 720, 0)]
    [TestCase(-1, 1, 3, -1)]
    public void WrapShouldBiasMinimumWhenValueEqualsJunction(Single min, Single max,
        Single value, Single expectedResult)
    {
        RangeF unitUnderTest = new(min, max);
        unitUnderTest.Wrap(value).Should().Be(expectedResult);
    }

    [Test]
    public void WrapShouldHandleZeroDelta()
    {
        RangeF unitUnderTest = new(10, 10);
        unitUnderTest.Wrap(15).Should().Be(10);
        unitUnderTest.Wrap(5).Should().Be(10);
    }


    // Scale Tests
    //----------------------------------------------------------------------------------------------
    
    [TestCaseSource(nameof(GetScaleData))]
    public void ScaleShouldAdjustValueFromOldRangeToNewRange(Single value, RangeF scaleFrom,
        RangeF scaleTo, Single expectedResult)
    {
        scaleTo.Scale(value, scaleFrom).Should().Be(expectedResult);
    }

    [TestCaseSource(nameof(GetOutsideScaleData), new Object[] { false })]
    public void ScaleShouldAdjustValuesOutsideOfRange(Single value, RangeF scaleFrom,
        RangeF scaleTo, Single expectedResult)
    {
        scaleTo.Scale(value, scaleFrom).Should().Be(expectedResult);
    }

    [Test]
    public void ScaleShouldHandleZeroTargetDelta()
    {
        RangeF unitUnderTest = new (10, 10);
        unitUnderTest.Scale(5, new RangeF(0, 10)).Should().Be(10);
    }


    // Fit Tests
    //----------------------------------------------------------------------------------------------
    
    [TestCaseSource(nameof(GetScaleData))]
    public void FitShouldScale(Single value, RangeF scaleFrom, RangeF scaleTo,
        Single expectedResult)
    {
        scaleTo.Fit(value, scaleFrom).Should().Be(expectedResult);
    }

    [TestCaseSource(nameof(GetOutsideScaleData), new Object[] { true })]
    public void FitShouldScaleThenClamp(Single value, RangeF scaleFrom, RangeF scaleTo,
        Single expectedResult)
    {
        scaleTo.Fit(value, scaleFrom).Should().Be(expectedResult);
    }


    // Map Tests
    //----------------------------------------------------------------------------------------------
    
    [TestCaseSource(nameof(GetScaleData))]
    public void MapShouldScale(Single value, RangeF scaleFrom, RangeF scaleTo,
        Single expectedResult)
    {
        scaleTo.Map(value, scaleFrom).Should().Be(expectedResult);
    }

    [TestCaseSource(nameof(GetMapData))]
    public void MapShouldScaleThenWrap(Single value, RangeF scaleFrom, RangeF scaleTo,
        Single expectedResult)
    {
        scaleTo.Map(value, scaleFrom).Should().Be(expectedResult);
    }


    // Contains Tests
    //----------------------------------------------------------------------------------------------

    [TestCaseSource(nameof(GetContainedPoints))]
    public void ContainsShouldReturnTrueWhenValueLiesInsideRange(RangeF range, Single value)
    {
        range.Contains(value).Should().BeTrue();
    }
    
    [TestCaseSource(nameof(GetOutsidePoints))]
    public void ContainsShouldReturnFalseValueLiesOutsideRange(RangeF range, Single value)
    {
        range.Contains(value).Should().BeFalse();
    }


    // Overlaps Tests
    //----------------------------------------------------------------------------------------------

    [TestCaseSource(nameof(GetOverlappingRanges))]
    public Boolean OverlapsShouldReturnTrueWhenRangesOverlap(RangeF rangeA, RangeF rangeB)
    {
        return rangeA.Overlaps(rangeB);
    }
    
    [TestCaseSource(nameof(GetNonOverlappingRanges))]
    public Boolean OverlapsShouldReturnFalseWhenRangesOverlap(RangeF rangeA, RangeF rangeB)
    {
        return rangeA.Overlaps(rangeB);
    }

    // Equality Test
    //----------------------------------------------------------------------------------------------
    
    [TestCaseSource(nameof(GetEqualityData))]
    public void EqualityTests(RangeF objectA, RangeF objectB, Boolean areEqual)
    {
        EqualityTesting.TestEqualsAndGetHashCode(objectA, objectB, areEqual);
    }
    
    [TestCaseSource(nameof(GetEqualityData))]
    public void EqualityOperatorTests(RangeF objectA, RangeF objectB, Boolean areEqual)
    {
        (objectA == objectB).Should().Be(areEqual);
        (objectA != objectB).Should().Be(!areEqual);
    }


    //  Support
    //----------------------------------------------------------------------------------------------

    public static IEnumerable<TestCaseData> GetScaleData() => new TestCaseData[]
        {
            new (5, new RangeF(0, 10), new RangeF(0, 100), 50),
            new (5, new RangeF(0, 10), new RangeF(0, 100), 50),
            new (5, new RangeF(0, 10), new RangeF(10, 20), 15),
            new (0, new RangeF(-5, 5), new RangeF(0, 100), 50),
            new (10, new RangeF(10, 20), new RangeF(0, 100), 0),
            new (15, new RangeF(10, 20), new RangeF(0, 100), 50),
        };

    public static IEnumerable<TestCaseData> GetOutsideScaleData(Boolean isClamped) =>
        new TestCaseData[]
        {
            new(20, new RangeF(0, 10), new RangeF(0, 100), isClamped ? 100 : 200),
            new(-10, new RangeF(0, 10), new RangeF(10, 20), isClamped ? 10 : 0),
            new(10, new RangeF(-5, 5), new RangeF(0, 100), isClamped ? 100 : 150),
            new(5, new RangeF(10, 20), new RangeF(0, 100), isClamped ? 0 : -50),
            new(25, new RangeF(10, 20), new RangeF(0, 100), isClamped ? 100 : 150),
            new(150, new RangeF(0, 100), new RangeF(-1, 1), isClamped ? 1 : 2),
            new(-50, new RangeF(0, 100), new RangeF(-1, 1), isClamped ? -1 : -2),
            new(150, new RangeF(0, 100), new RangeF(10, 20), isClamped ? 20 : 25),
            new(-50, new RangeF(0, 100), new RangeF(10, 20), isClamped ? 10 : 5),
        };

    public static IEnumerable<TestCaseData> GetMapData() => new TestCaseData[]
        {
            new(15, new RangeF(0, 10), new RangeF(0, 100), 50),
            new(-5, new RangeF(0, 10), new RangeF(10, 20), 15),
            new(10, new RangeF(-5, 5), new RangeF(0, 100), 50),
            new(5, new RangeF(10, 20), new RangeF(0, 100), 50),
            new(25, new RangeF(10, 20), new RangeF(0, 100), 50),
            new(150, new RangeF(0, 100), new RangeF(-1, 1), 0),
            new(-50, new RangeF(0, 100), new RangeF(-1, 1), 0),
            new(150, new RangeF(0, 100), new RangeF(10, 20), 15),
            new(-50, new RangeF(0, 100), new RangeF(10, 20), 15),
        };


    public static IEnumerable<TestCaseData> GetContainedPoints() => new[]
        {
            new TestCaseData(new RangeF(10, 20), 10),
            new TestCaseData(new RangeF(10, 20), 15),
            new TestCaseData(new RangeF(10, 20), 20),
            new TestCaseData(new RangeF(-5, 20), 0),
            new TestCaseData(new RangeF(-10, -5), -5),
            new TestCaseData(new RangeF(-10, -5), -7),
            new TestCaseData(new RangeF(-10, -5), -10),
            new TestCaseData(new RangeF(100, 500), 499),
        };
    
    public static IEnumerable<TestCaseData> GetOutsidePoints() => new[]
        {
            new TestCaseData(new RangeF(10, 20), 5),
            new TestCaseData(new RangeF(10, 20), 25),
            new TestCaseData(new RangeF(10, 20), 100),
            new TestCaseData(new RangeF(-5, 20), -7),
            new TestCaseData(new RangeF(-5, 20), 25),
            new TestCaseData(new RangeF(-10, -5), -12),
            new TestCaseData(new RangeF(-10, -5), -2),
            new TestCaseData(new RangeF(100, 500), 1000),
            new TestCaseData(new RangeF(100, 500), 57),
        };
    
    public static IEnumerable<TestCaseData> GetOverlappingRanges() => new[]
        {
            new TestCaseData(new RangeF(10, 20), new RangeF(15, 20))
                .Returns(true).SetName("Overlap right"),
            new TestCaseData(new RangeF(50, 100), new RangeF(10, 60))
                .Returns(true).SetName("Overlap left"),
            new TestCaseData(new RangeF(0, 100), new RangeF(20, 40))
                .Returns(true).SetName("A contains B"),
            new TestCaseData(new RangeF(10, 20), new RangeF(0, 50))
                .Returns(true).SetName("B contains A"),
            new TestCaseData(new RangeF(10, 20), new RangeF(20, 30))
                .Returns(true).SetName("Touching right"),
            new TestCaseData(new RangeF(50, 100), new RangeF(0, 50))
                .Returns(true).SetName("Touching left"),
        };
    
    public static IEnumerable<TestCaseData> GetNonOverlappingRanges() => new[]
        {
            new TestCaseData(new RangeF(10, 20), new RangeF(30, 40))
                .Returns(false),
            new TestCaseData(new RangeF(21, 40), new RangeF(10, 20))
                .Returns(false),
        };

    public static IEnumerable<TestCaseData> GetEqualityData() => new TestCaseData[]
        {
            new(new RangeF(5, 10), new RangeF(5, 10), true),
            new(new RangeF(5, 10), new RangeF(5, 20), false),
            new(new RangeF(5, 10), new RangeF(1, 10), false),
        };

}
