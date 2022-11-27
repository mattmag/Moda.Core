// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using Moda.Core.Tests.Support;
using Moda.Core.Utility.Data;
using Moda.Core.Utility.Geometry;
using NUnit.Framework;

namespace Moda.Core.Tests.Utility.Geometry;

[TestFixture]
public class RectangleFTests
{
    // Constructor Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void ConstructorShouldInitializeCorners()
    {
        RectangleF rectangle = new(new(5, 10), new(60, 80));
        rectangle[Corner.TopLeft].Should().Be(new Vector2(5, 10));
        rectangle[Corner.TopRight].Should().Be(new Vector2(65, 10));
        rectangle[Corner.BottomLeft].Should().Be(new Vector2(5, 90));
        rectangle[Corner.BottomRight].Should().Be(new Vector2(65, 90));
    }
    
    [Test]
    public void ConstructorShouldInitializeSides()
    {
        RectangleF rectangle = new(new(20, 30), new(150, 200));
        rectangle[Side.Top].Should().Be(LineSegment.FromPointToEnd(new(20, 30), new(170, 30)));
        rectangle[Side.Right].Should().Be(LineSegment.FromPointToEnd(new(170, 30), new(170, 230)));
        rectangle[Side.Bottom].Should().Be(LineSegment.FromPointToEnd(new(170, 230), new(20, 230)));
        rectangle[Side.Left].Should().Be(LineSegment.FromPointToEnd(new(20, 230), new(20, 30)));
    }


    [Test]
    public void ConstructorShouldInitializeCenter()
    {
        RectangleF rectangle = new(new(0, 0), new(100, 50));
        rectangle.Center.Should().Be(new Vector2(50, 25));
    }
    
    [Test]
    public void ConstructorShouldInitializeSize()
    {
        RectangleF rectangle = new(new(20, 30), new(80, 70));
        rectangle.Size.Should().Be(new Size2(80, 70));
    }
    
    [Test]
    public void CopyConstructorShouldInitializeCorners()
    {
        RectangleF rectangleA = new(new(5, 10), new(60, 80));
        RectangleF rectangleB = new(rectangleA);
        rectangleB[Corner.TopLeft].Should().Be(new Vector2(5, 10));
        rectangleB[Corner.TopRight].Should().Be(new Vector2(65, 10));
        rectangleB[Corner.BottomLeft].Should().Be(new Vector2(5, 90));
        rectangleB[Corner.BottomRight].Should().Be(new Vector2(65, 90));
    }
    
    [Test]
    public void CopyConstructorShouldInitializeSides()
    {
        RectangleF rectangleA = new(new(20, 30), new(150, 200));
        RectangleF rectangleB = new(rectangleA);
        rectangleB[Side.Top].Should().Be(LineSegment.FromPointToEnd(new(20, 30), new(170, 30)));
        rectangleB[Side.Right].Should().Be(LineSegment.FromPointToEnd(new(170, 30), new(170, 230)));
        rectangleB[Side.Bottom].Should()
            .Be(LineSegment.FromPointToEnd(new(170, 230), new(20, 230)));
        rectangleB[Side.Left].Should().Be(LineSegment.FromPointToEnd(new(20, 230), new(20, 30)));
    }


    [Test]
    public void CopyConstructorShouldInitializeCenter()
    {
        RectangleF rectangleA = new(new(0, 0), new(100, 50));
        RectangleF rectangleB = new(rectangleA);
        rectangleB.Center.Should().Be(new Vector2(50, 25));
    }
    
    [Test]
    public void CopyConstructorShouldInitializeSize()
    {
        RectangleF rectangleA = new(new(20, 30), new(80, 70));
        RectangleF rectangleB = new(rectangleA);
        rectangleB.Size.Should().Be(new Size2(80, 70));
    }


    // Indexer Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void SideIndexShouldThrowArgumentExceptionWithInvalidValue()
    {
        RectangleF rectangle = new(new(0, 0), new(100, 50));

        rectangle.Invoking(a => a[(Side)999]).Should().Throw<IndexOutOfRangeException>();
    }
    
    [Test]
    public void CornerIndexShouldThrowArgumentExceptionWithInvalidValue()
    {
        RectangleF rectangle = new(new(0, 0), new(100, 50));

        rectangle.Invoking(a => a[(Corner)999]).Should().Throw<IndexOutOfRangeException>();
    }


    // Enumeration Tests
    //----------------------------------------------------------------------------------------------


    [Test]
    public void GetSidesShouldEnumerateAllSides()
    {
        RectangleF rectangle = new(new(10, 20), new(50, 100));
        IEnumerable<KeyValuePair<Side, LineSegment>> results = rectangle.GetSides();
        results.Should().BeEquivalentTo(new[]
            {
                new LineSegment(new(10,20), new(60,20)).KeyedOn(Side.Top),
                new LineSegment(new(60,20), new(60,120)).KeyedOn(Side.Right),
                new LineSegment(new(60,120), new(10,120)).KeyedOn(Side.Bottom),
                new LineSegment(new(10,120), new(10,20)).KeyedOn(Side.Left),
            }
        );
    }


    [Test]
    public void GetCornersShouldEnumerateAllSides()
    {
        RectangleF rectangle = new(new(10, 20), new(50, 100));
        IEnumerable<KeyValuePair<Corner, Vector2>> results = rectangle.GetCorners();
        results.Should().BeEquivalentTo(new[]
            {
                new Vector2(10, 20).KeyedOn(Corner.TopLeft),
                new Vector2(60, 20).KeyedOn(Corner.TopRight),
                new Vector2(60, 120).KeyedOn(Corner.BottomRight),
                new Vector2(10, 120).KeyedOn(Corner.BottomLeft),
            }
        );
    }



    // IsOverlapping Tests
    //----------------------------------------------------------------------------------------------


    [TestCaseSource(nameof(GetOverlappingRectangles))]
    public void IsOverlappingWithOverlappingRectangleShouldReturnTrue(RectangleF rectangleA, 
        RectangleF rectangleB)
    {
        rectangleA.IsOverlapping(rectangleB).Should().BeTrue();
    }
    
    public static IEnumerable<TestCaseData> GetOverlappingRectangles() => new[]
        {
            new TestCaseData(
                new RectangleF(new(50, 50), new(50, 50)),
                new RectangleF(new(25, 25), new(50, 50)))
                .SetName("B overlaps A from upper left"),
            new TestCaseData(
                new RectangleF(new(50, 50), new(50, 50)),
                new RectangleF(new(75, 25), new(50, 50)))
                .SetName("B overlaps A from upper right"),
            new TestCaseData(
                new RectangleF(new(50, 50), new(50, 50)),
                new RectangleF(new(75, 75), new(50, 50)))
                .SetName("B overlaps A from lower right"),
            new TestCaseData(
                new RectangleF(new(50, 50), new(50, 50)),
                new RectangleF(new(25, 75), new(50, 50)))
                .SetName("B overlaps A from lower left"),
            
            new TestCaseData(
                new RectangleF(new(50, 50), new(50, 50)),
                new RectangleF(new(25, 50), new(50, 50)))
                .SetName("B overlaps A from left"),
            new TestCaseData(
                new RectangleF(new(50, 50), new(50, 50)),
                new RectangleF(new(50, 25), new(50, 50)))
                .SetName("B overlaps A from top"),
            new TestCaseData(
                new RectangleF(new(50, 50), new(50, 50)),
                new RectangleF(new(75, 50), new(50, 50)))
                .SetName("B overlaps A from right"),
            new TestCaseData(
                new RectangleF(new(50, 50), new(50, 50)),
                new RectangleF(new(50, 75), new(50, 50)))
                .SetName("B overlaps A from bottom"),
            
            new TestCaseData(
                new RectangleF(new(50, 50), new(50, 50)),
                new RectangleF(new(38, 64), new(25, 25)))
                .SetName("Smaller B overlaps A from left"),
            new TestCaseData(
                new RectangleF(new(50, 50), new(50, 50)),
                new RectangleF(new(64, 38), new(25, 25)))
                .SetName("Smaller B overlaps A from top"),
            new TestCaseData(
                new RectangleF(new(50, 50), new(50, 50)),
                new RectangleF(new(88, 64), new(25, 25)))
                .SetName("Smaller B overlaps A from right"),
            new TestCaseData(
                new RectangleF(new(50, 50), new(50, 50)),
                new RectangleF(new(64, 88), new(25, 25)))
                .SetName("Smaller B overlaps A from bottom"),
        };
    
    
    [TestCaseSource(nameof(GetAbuttingRectangles))]
    public void IsOverlappingWithAbuttingRectangleShouldReturnTrue(RectangleF rectangleA,
        RectangleF rectangleB)
    {
        rectangleA.IsOverlapping(rectangleB).Should().BeTrue();
    }


    public static IEnumerable<TestCaseData> GetAbuttingRectangles() => new[]
        {
            new TestCaseData(
                new RectangleF(new(50, 50), new(50, 50)),
                new RectangleF(new(0, 50), new(50, 50)))
                .SetName("B abuts A from left"),
            new TestCaseData(
                new RectangleF(new(50, 50), new(50, 50)),
                new RectangleF(new(50, 0), new(50, 50)))
                .SetName("B abuts A from top"),
            new TestCaseData(
                new RectangleF(new(50, 50), new(50, 50)),
                new RectangleF(new(100, 50), new(50, 50)))
                .SetName("B abuts A from right"),
            new TestCaseData(
                new RectangleF(new(50, 50), new(50, 50)),
                new RectangleF(new(50, 100), new(50, 50)))
                .SetName("B abuts A from bottom"),
            
            new TestCaseData(
                new RectangleF(new(50, 50), new(50, 50)),
                new RectangleF(new(25, 64), new(25, 25)))
                .SetName("Smaller B abuts A from left"),
            new TestCaseData(
                new RectangleF(new(50, 50), new(50, 50)),
                new RectangleF(new(64, 25), new(25, 25)))
                .SetName("Smaller B abuts A from top"),
            new TestCaseData(
                new RectangleF(new(50, 50), new(50, 50)),
                new RectangleF(new(100, 64), new(25, 25)))
                .SetName("Smaller B abuts A from right"),
            new TestCaseData(
                new RectangleF(new(50, 50), new(50, 50)),
                new RectangleF(new(64, 100), new(25, 25)))
                .SetName("Smaller B abuts A from bottom"),
        };

    
    [TestCaseSource(nameof(GetContainedRectangles))]
    public void IsOverlappingShouldReturnTrueWhenOneRectangleContainsTheOther(RectangleF rectangleA,
        RectangleF rectangleB)
    {
        rectangleA.IsOverlapping(rectangleB).Should().BeTrue();
    }


    public static IEnumerable<TestCaseData> GetContainedRectangles() => new[]
        {
            new TestCaseData(
                new RectangleF(new(0, 0), new(100, 100)),
                new RectangleF(new(25, 25), new(50, 50)))
                .SetName("A contains B"),
            new TestCaseData(
                new RectangleF(new(25, 25), new(50, 50)),
                new RectangleF(new(0, 0), new(100, 100)))
                .SetName("B contains A"),
        };

    [Test]
    public void IsOverlappingWithNonOverlappingRectangleShouldReturnFalse()
    {
        RectangleF rectangleA = new(new(10, 10), new(10, 10));
        RectangleF rectangleB = new(new(50, 50), new(10, 10));
        rectangleA.IsOverlapping(rectangleB).Should().BeFalse();
    }


    // ContainsPoint Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void ContainsPointShouldReturnTrueIfPointLiesWithinRectangle()
    {
        RectangleF rectangle = new(new(10, 10), new(100, 100));
        rectangle.ContainsPoint(new(35, 61)).Should().BeTrue();
    }

    
    [TestCaseSource(nameof(GetOutsidePoints))]
    public void ContainsPointShouldReturnFalseIfPointLiesOutsideOfRectangle(Vector2 point)
    {
        RectangleF rectangle = new(new(10, 10), new(100, 100));
        rectangle.ContainsPoint(point).Should().BeFalse();
    }


    public static IEnumerable<TestCaseData> GetOutsidePoints() => new[]
        {
            new TestCaseData(new Vector2(0, 0)).SetName("Top Left"),
            new TestCaseData(new Vector2(0, 50)).SetName("Top"),
            new TestCaseData(new Vector2(0, 120)).SetName("Top Right"),
            new TestCaseData(new Vector2(120, 50)).SetName("Right"),
            new TestCaseData(new Vector2(120, 120)).SetName("Bottom Right"),
            new TestCaseData(new Vector2(50, 120)).SetName("Bottom"),
            new TestCaseData(new Vector2(0, 120)).SetName("Bottom Left"),
            new TestCaseData(new Vector2(0, 50)).SetName("Left"),
        };


    // Inflate Tests
    //----------------------------------------------------------------------------------------------

    [Test]
    public void InflateShouldIncreaseSizeBySpecifiedAmounts()
    {
        RectangleF rectangle = new(new(20, 30), new(40, 60));
        RectangleF inflated = rectangle.Inflate(10, 5);
        inflated.Size.Width.Should().Be(50);
        inflated.Size.Height.Should().Be(65);
    }
    
    
    [Test]
    public void InflateShouldIncreaseSizeBySpecifiedAmountsWhenNegative()
    {
        RectangleF rectangle = new(new(20, 30), new(40, 60));
        RectangleF inflated = rectangle.Inflate(-10, -5);
        inflated.Size.Width.Should().Be(30);
        inflated.Size.Height.Should().Be(55);
    }
    
    [Test]
    public void InflateShouldKeepRectangleCenterPosition()
    {
        RectangleF rectangle = new(new(20, 30), new(40, 60));
        RectangleF inflated = rectangle.Inflate(10, 5);
        inflated.Center.Should().Be(rectangle.Center);
    }
    
    [Test]
    public void InflateShouldAdjustBoundsAboutCenter()
    {
        RectangleF rectangle = new(new(20, 30), new(40, 60));
        RectangleF inflated = rectangle.Inflate(10, 5);
        inflated[Corner.TopLeft].Should().Be(new Vector2(15, 27.5f));
        inflated[Corner.TopRight].Should().Be(new Vector2(65, 27.5f));
        inflated[Corner.BottomRight].Should().Be(new Vector2(65, 92.5f));
        inflated[Corner.BottomLeft].Should().Be(new Vector2(15, 92.5f));
    }


    // Translate Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void TranslateShouldMoveBoundsRelativelyByTheSpecifiedAmount()
    {
        RectangleF rectangle = new(new(20, 30), new(40, 60));
        RectangleF translated = rectangle.Translate(new(10, 5));
        translated[Corner.TopLeft].Should().Be(new Vector2(30, 35));
        translated[Corner.TopRight].Should().Be(new Vector2(70, 35));
        translated[Corner.BottomRight].Should().Be(new Vector2(70, 95));
        translated[Corner.BottomLeft].Should().Be(new Vector2(30, 95));
    }
    
    [Test]
    public void TranslateShouldMoveBoundsRelativelyByTheSpecifiedAmountWhenNegative()
    {
        RectangleF rectangle = new(new(20, 30), new(40, 60));
        RectangleF translated = rectangle.Translate(new(-10, -5));
        translated[Corner.TopLeft].Should().Be(new Vector2(10, 25));
        translated[Corner.TopRight].Should().Be(new Vector2(50, 25));
        translated[Corner.BottomRight].Should().Be(new Vector2(50, 85));
        translated[Corner.BottomLeft].Should().Be(new Vector2(10, 85));
    }


    // CenterAt Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void CenterAtShouldTranslateAboutCenter()
    {
        RectangleF rectangle = new(new(20, 30), new(40, 60));
        RectangleF translated = rectangle.CenterAt(new(120, 180));
        translated.Center.Should().Be(new Vector2(120, 180));
    }
    
    [Test]
    public void CenterAtShouldNotAffectSize()
    {
        RectangleF rectangle = new(new(20, 30), new(40, 60));
        RectangleF translated = rectangle.CenterAt(new(120, 180));
        translated.Size.Should().Be(new Size2(40, 60));
    }


    // IntersectionWith Tests
    //----------------------------------------------------------------------------------------------


    [Test]
    public void IntersectionWithShouldReturnAllIntersectedSides()
    {
        RectangleF rectangle = new(new(10, 10), new(100, 100));
        LineSegment segment = LineSegment.FromPointWithDelta(new(0, 60), new(120, 0));
        rectangle.IntersectionWith(segment).Select(a => a.Side).Should()
            .BeEquivalentTo(new[] { Side.Left, Side.Right });
    }
    
    [Test]
    public void IntersectionWithShouldReturnSidesInOrderOfIntersect()
    {
        RectangleF rectangle = new(new(10, 10), new(100, 100));
        LineSegment segment = LineSegment.FromPointWithDelta(new(120, 60), new(-120, 0));
        rectangle.IntersectionWith(segment).Select(a => a.Side).Should()
            .BeEquivalentTo(new[] { Side.Right, Side.Left },
                a => a.WithStrictOrdering());
    }


    [Test]
    public void IntersectionWithShouldReturnTimeOfEachIntersection()
    {
        RectangleF rectangle = new(new(20, 20), new(40, 40));
        LineSegment segment = LineSegment.FromPointWithDelta(new(0, 40), new(80, 0));
        rectangle.IntersectionWith(segment).Select(a => a.Intersection.FirstImpact()).Should()
            .BeEquivalentTo(new[] { 0.25, 0.75 }, a => a.WithStrictOrdering());
    }
    
    [Test]
    public void IntersectionWithCollinearSideShouldReturnAllIntersections()
    {
        RectangleF rectangle = new(new(20, 20), new(40, 40));
        LineSegment segment = LineSegment.FromPointWithDelta(new(0, 20), new(80, 0));
        rectangle.IntersectionWith(segment).Select(a => a.Side).Should()
            .BeEquivalentTo(new[] { Side.Left, Side.Top, Side.Right }, a => a.WithStrictOrdering());
    }
    
    [Test]
    public void IntersectionWithCollinearSideShouldReturnTimesAndRangesOfAllIntersections()
    {
        RectangleF rectangle = new(new(20, 20), new(40, 40));
        LineSegment segment = LineSegment.FromPointWithDelta(new(0, 20), new(80, 0));
        rectangle.IntersectionWith(segment).Select(a => a.Intersection).Should()
            .BeEquivalentTo(new Intersection[]
                {
                    new Intersection.Intersects(0.25f),
                    new Intersection.Overlaps(new (0.25f, 0.75f)),
                    new Intersection.Intersects(0.75f),
                }, a => a.WithStrictOrdering().RespectingRuntimeTypes());
    }
    

    

    // Equality Tests
    //----------------------------------------------------------------------------------------------
    
    [TestCaseSource(nameof(GetEqualityData))]
    public void EqualityTests(RectangleF objectA, RectangleF objectB,
        Boolean areEqual)
    {
        EqualityTesting.TestEqualsAndGetHashCode(objectA, objectB, areEqual);
    }
    
    [TestCaseSource(nameof(GetEqualityData))]
    public void EqualityOperatorTests(RectangleF objectA, RectangleF objectB,
        Boolean areEqual)
    {
        (objectA == objectB).Should().Be(areEqual);
        (objectA != objectB).Should().Be(!areEqual);
    }
    
    public static IEnumerable<TestCaseData> GetEqualityData() => new TestCaseData[]
        {
            new(new RectangleF(new(10,15), new(30,40)),
                new RectangleF(new(10,15), new(30,40)),
                true),
            new(new RectangleF(new(3.2f,0.5f), new(0.8f,0.1f)),
                new RectangleF(new(3.2f,0.5f), new(0.8f,0.1f)),
                true),
            new(new RectangleF(new(10,13), new(30,40)),
                new RectangleF(new(10,15), new(30,40)),
                false),
            new(new RectangleF(new(10,15), new(30,40)),
                new RectangleF(new(13,15), new(30,40)),
                false),
            new(new RectangleF(new(10,15), new(900,40)),
                new RectangleF(new(10,15), new(30,40)),
                false),
            new(new RectangleF(new(10,15), new(30,40)),
                new RectangleF(new(10,15), new(30,6000)),
                false),
        };

}
