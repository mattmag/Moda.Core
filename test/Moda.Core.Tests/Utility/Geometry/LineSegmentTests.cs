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
using Moda.Core.Utility.Maths;
using NUnit.Framework;
using Optional;

namespace Moda.Core.Tests.Utility.Geometry;

public class LineSegmentTests
{
    private static readonly Single TOLERANCE = 0.001f;
    
    // IntersectionWith() Tests
    //----------------------------------------------------------------------------------------------
    
    [TestCaseSource(nameof(GetNonIntersectingLineSegments))]
    public void IntersectionWithShouldReturnNoneIfNoIntersection(LineSegment segmentA,
        LineSegment segmentB)
    {
        segmentA.IntersectionWith(segmentB).Should().Be(Option.None<Intersection>());
    }


    public static IEnumerable<TestCaseData> GetNonIntersectingLineSegments()
    {
        yield return new(
            new LineSegment(new(12, 0), new(12, 12)),
            new LineSegment(new(24, 0), new(24, 12)));
        yield return new(
            new LineSegment(new(201, 178), new(354, 311)),
            new LineSegment(new(473, 238), new(590, 158)));
        yield return new(
            new LineSegment(new(-51, -77), new(-111, -33)),
            new LineSegment(new(-99, -69), new(-136, -111)));
        yield return new(
            new LineSegment(new(641, 474), new(733, 375)),
            new LineSegment(new(769, 380), new(625, 505)));
        yield return new(
            new LineSegment(new(5, 5), new(10, 5)),
            new LineSegment(new(12, 5), new(22, 5)));
        yield return new(
            new LineSegment(new(5, 5), new(10, 5)),
            new LineSegment(new(22, 5), new(12, 5)));
    }


    [TestCaseSource(nameof(GetIntersectingLineSegments))]
    public void IntersectionWithShouldReturnAccurateResultIfIntersectionExists(LineSegment segmentA,
        LineSegment segmentB, Single expectedResult)
    {
        segmentA.IntersectionWith(segmentB)
            .Filter(a => a is Intersection.Intersects)
            .Map(a => (Intersection.Intersects)a)
            .Match(a => a.At.Should().BeApproximately(expectedResult, TOLERANCE),
                () => Assert.Fail("Unexpected intersection result"));
    }



    public static IEnumerable<TestCaseData> GetIntersectingLineSegments()
    {
        yield return new(
            new LineSegment(new(12, 0), new(12, 12)),
            new LineSegment(new(0, 6), new(16, 6)),
            0.5f);
        yield return new(
            new LineSegment(new(147, 162), new(243, 77)),
            new LineSegment(new(197, 65), new(261, 116)),
            0.847f);
        yield return new(
            new LineSegment(new(570, 273), new(630, 243)), 
            new LineSegment(new(593, 280), new(539, 160)),
            0.270f);
        yield return new(
            new LineSegment(new(149, 436), new(290, 318)),
            new LineSegment(new(156, 443), new(274, 303)),
            0.310f);
    }
    
    
    [TestCaseSource(nameof(GetIntersectingLineSegmentsExtremes))]
    public void IntersectionWithShouldReturnIntersectionAtExtremes(LineSegment segmentA,
        LineSegment segmentB, Single expectedResult)
    {
        segmentA.IntersectionWith(segmentB)
            .Filter(a => a is Intersection.Intersects)
            .Map(a => (Intersection.Intersects)a)
            .Match(a => a.At.Should().BeApproximately(expectedResult, TOLERANCE),
                () => Assert.Fail("Unexpected intersection result"));
    }
    
    public static IEnumerable<TestCaseData> GetIntersectingLineSegmentsExtremes()
    {
        yield return new(
            new LineSegment(new(5, 5), new(10, 5)),
            new LineSegment(new(10, 20), new(10, 5)),
            1f);
        yield return new(
            new LineSegment(new(10, 5), new(5, 5)),
            new LineSegment(new(10, 20), new(10, 5)),
            0f);
        yield return new(
            new LineSegment(new(307, 103), new(455, 238)),
            new LineSegment(new(435, 180), new(455, 238)),
            1f);
    }
    
    
    [TestCaseSource(nameof(GetIntersectingCollinearLineSegments))]
    public void IntersectionWithShouldReturnIntersectionForCollinearSegments(LineSegment segmentA,
        LineSegment segmentB, RangeF expectedResult)
    {
        segmentA.IntersectionWith(segmentB)
            .Filter(a => a is Intersection.Overlaps)
            .Map(a => (Intersection.Overlaps)a)
            .Match(a =>
                {
                    a.Range.Minimum.Should().BeApproximately(expectedResult.Minimum, TOLERANCE);
                    a.Range.Maximum.Should().BeApproximately(expectedResult.Maximum, TOLERANCE);
                },
                () => Assert.Fail("Unexpected intersection result"));
    }
    
    public static IEnumerable<TestCaseData> GetIntersectingCollinearLineSegments()
    {
        yield return new(
            new LineSegment(new(5, 5), new(15, 5)),
            new LineSegment(new(30, 5), new(10, 5)),
            new RangeF(0.5f, 1));
        yield return new(
            new LineSegment(new(15, 5), new(5, 5)),
            new LineSegment(new(10, 5), new(30, 5)),
            new RangeF(0, 0.5f));
        yield return new(
            new LineSegment(new(0, 10), new(100, 10)),
            new LineSegment(new(20, 10), new(65, 10)),
            new RangeF(0.20f, 0.65f));
         yield return new(
            new LineSegment(new(20, 10), new(65, 10)),
            new LineSegment(new(0, 10), new(100, 10)),
            new RangeF(0f, 1f));
    }


    // Factory Method Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void FromPointWithDeltaShouldSetProperties()
    {
        LineSegment segment = LineSegment.FromPointWithDelta(new(2,4), new(10,14));
        segment.Point.Should().Be(new Vector2(2, 4));
        segment.End.Should().Be(new Vector2(12, 18));
        segment.Delta.Should().Be(new Vector2(10, 14));
    }
    
    [Test]
    public void FromPointWithEndShouldSetProperties()
    {
        LineSegment segment = LineSegment.FromPointToEnd(new(2,4), new(10,14));
        segment.Point.Should().Be(new Vector2(2, 4));
        segment.End.Should().Be(new Vector2(10, 14));
        segment.Delta.Should().Be(new Vector2(8, 10));
    }


    // Equality Tests
    //----------------------------------------------------------------------------------------------

    [TestCaseSource(nameof(GetEqualityData))]
    public void EqualityTests(LineSegment objectA, LineSegment objectB, Boolean areEqual)
    {
        EqualityTesting.TestEqualsAndGetHashCode(objectA, objectB, areEqual);
    }
    
    [TestCaseSource(nameof(GetEqualityData))]
    public void EqualityOperatorTests(LineSegment objectA, LineSegment objectB, Boolean areEqual)
    {
        (objectA == objectB).Should().Be(areEqual);
        (objectA != objectB).Should().Be(!areEqual);
    }
    
    public static IEnumerable<TestCaseData> GetEqualityData() => new TestCaseData[]
        {
            new(new LineSegment(new(5, 10), new(20, 64)),
                new LineSegment(new(5, 10), new(20, 64)),
                true),
            new(new LineSegment(new(5.26f, 10.1f), new(20.89f, 64.4f)),
                new LineSegment(new(5.26f, 10.1f), new(20.89f, 64.4f)),
                true),
            new(new LineSegment(new(6, 5), new(7, 3)),
                new LineSegment(new(6, 5), new(7, 5)),
                false),
            new(new LineSegment(new(900, -100), new(750, -800)),
                new LineSegment(new(-5, -100), new(-60, -800)),
                false),
        };


}
