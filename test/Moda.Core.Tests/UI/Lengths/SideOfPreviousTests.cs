// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using Moda.Core.UI;
using Moda.Core.UI.Builders;
using Moda.Core.UI.Lengths;
using Moda.Core.Utility.Geometry;
using Moq;
using NUnit.Framework;
using Optional;
using Optional.Unsafe;

namespace Moda.Core.Tests.UI.Lengths;

public class SideOfPreviousTests
{
    // Calculate() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void CalculateShouldThrowExceptionIfNotInitialized()
    {
        SideOfPrevious sideOfPrevious = new(NCoordinate.Alpha, Option.None<ILength>());
        sideOfPrevious.Invoking(a => a.Calculate()).Should()
            .Throw<OptionValueMissingException>();
    }
    
    [TestCaseSource(nameof(GetCalculateTestCases))]
    public void CalculateShouldReturnSideOfPreviousPeerPlusOffset(
        Axis axis, NCoordinate side, Option<ILength> offset, Single result)
    {
        TestCalculate(axis, side, offset, result, false);
    }
    
    
    [TestCaseSource(nameof(GetCalculateTestCases))]
    public void CalculateShouldThrowExceptionWhenNoPreviousPeer(Axis axis, NCoordinate side,
        Option<ILength> offset, Single _)
    {
        Cell parent = new(Mock.Of<IHoneyComb>(), Mock.Of<ILength>(), Mock.Of<ILength>(),
            Mock.Of<ILength>(), Mock.Of<ILength>());
        Cell child = new(Mock.Of<IHoneyComb>(), Mock.Of<ILength>(), Mock.Of<ILength>(),
            Mock.Of<ILength>(), Mock.Of<ILength>());
        parent.AppendChild(child);
        
        LayoutAllCoordinates(parent);
        SideOfPrevious sideOfPrevious = new(side, offset);
        sideOfPrevious.Initialize(child, axis);
        sideOfPrevious.Invoking(a => a.Calculate()).Should()
            .Throw<OptionValueMissingException>();
    }


    [Test]
    public void CalculateShouldThrowExceptionWhenCoordinateIsUnknown()
    {
        Cell parent = new(Mock.Of<IHoneyComb>(), Mock.Of<ILength>(), Mock.Of<ILength>(),
            Mock.Of<ILength>(), Mock.Of<ILength>());
        Cell child1 = new(Mock.Of<IHoneyComb>(), Mock.Of<ILength>(), Mock.Of<ILength>(),
            Mock.Of<ILength>(), Mock.Of<ILength>());
        parent.AppendChild(child1);
        Cell child2 = new(Mock.Of<IHoneyComb>(), Mock.Of<ILength>(), Mock.Of<ILength>(),
            Mock.Of<ILength>(), Mock.Of<ILength>());
        parent.AppendChild(child2);
        
        LayoutAllCoordinates(parent);
        SideOfPrevious sideOfPrevious = new((NCoordinate)999, Option.None<ILength>());
        sideOfPrevious.Initialize(child2, Axis.X);
        sideOfPrevious.Invoking(a => a.Calculate()).Should()
            .Throw<ArgumentOutOfRangeException>();
    }


    // TryCalculate() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void TryCalculateShouldThrowExceptionIfNotInitialized()
    {
        SideOfPrevious sideOfPrevious = new(NCoordinate.Alpha, Option.None<ILength>());
        sideOfPrevious.Invoking(a => a.TryCalculate()).Should()
            .Throw<OptionValueMissingException>();
    }
    
    [TestCaseSource(nameof(GetCalculateTestCases))]
    public void TryCalculateShouldReturnSideOfPreviousPeerPlusOffset(
        Axis axis, NCoordinate side, Option<ILength> offset, Single result)
    {
        TestCalculate(axis, side, offset, result, true);
    }


    [TestCaseSource(nameof(GetCalculateTestCases))]
    public void TryCalculateShouldReturnNoneIfNoPreviousPeer(Axis axis, NCoordinate side, 
        Option<ILength> offset, Single _)
    {
        Cell parent = new(Mock.Of<IHoneyComb>(), Mock.Of<ILength>(), Mock.Of<ILength>(),
            Mock.Of<ILength>(), Mock.Of<ILength>());
        Cell child = new(Mock.Of<IHoneyComb>(), Mock.Of<ILength>(), Mock.Of<ILength>(),
            Mock.Of<ILength>(), Mock.Of<ILength>());
        parent.AppendChild(child);
        
        LayoutAllCoordinates(parent);
        SideOfPrevious sideOfPrevious = new(side, offset);
        sideOfPrevious.Initialize(child, axis);
        sideOfPrevious.TryCalculate().Should().Be(Option.None<Single>());
    }


    [Test]
    public void TryCalculateShouldThrowExceptionWhenCoordinateIsUnknown()
    {
        Cell parent = new(Mock.Of<IHoneyComb>(), Mock.Of<ILength>(), Mock.Of<ILength>(),
            Mock.Of<ILength>(), Mock.Of<ILength>());
        Cell child1 = new(Mock.Of<IHoneyComb>(), Mock.Of<ILength>(), Mock.Of<ILength>(),
            Mock.Of<ILength>(), Mock.Of<ILength>());
        parent.AppendChild(child1);
        Cell child2 = new(Mock.Of<IHoneyComb>(), Mock.Of<ILength>(), Mock.Of<ILength>(),
            Mock.Of<ILength>(), Mock.Of<ILength>());
        parent.AppendChild(child2);
        
        LayoutAllCoordinates(parent);
        SideOfPrevious sideOfPrevious = new((NCoordinate)999, Option.None<ILength>());
        sideOfPrevious.Initialize(child2, Axis.X);
        sideOfPrevious.Invoking(a => a.Calculate()).Should()
            .Throw<ArgumentOutOfRangeException>();
    }


    // Support
    //----------------------------------------------------------------------------------------------
    
    private static IEnumerable<TestCaseData> GetCalculateTestCases => new TestCaseData[]
        {
            new(Axis.X, NCoordinate.Alpha, Option.None<ILength>(), 30),
            new(Axis.X, NCoordinate.Beta, Option.None<ILength>(), 70),
            new(Axis.Y, NCoordinate.Alpha, Option.None<ILength>(), 15),
            new(Axis.Y, NCoordinate.Beta, Option.None<ILength>(), 25),
            new(Axis.X, NCoordinate.Alpha, Len(6).Some(), 36),
            new(Axis.X, NCoordinate.Beta, Len(6).Some(), 76),
            new(Axis.Y, NCoordinate.Alpha, Len(6).Some(), 21),
            new(Axis.Y, NCoordinate.Beta, Len(6).Some(), 31),
        };

    private void TestCalculate(Axis axis, NCoordinate side, Option<ILength> offset, Single result,
        bool useTryCalculate)
    {
        Cell parent = new(Mock.Of<IHoneyComb>(), Len(0), Len(1000), Len(0), Len(1000));
        Cell child1 = new(Mock.Of<IHoneyComb>(), Len(20), Len(80), Len(10), Len(30));
        parent.AppendChild(child1);
        Cell child2 = new(Mock.Of<IHoneyComb>(), Len(30), Len(70), Len(15), Len(25));
        parent.AppendChild(child2);
        
        Cell child3 = new(Mock.Of<IHoneyComb>(), Mock.Of<ILength>(), Mock.Of<ILength>(),
            Mock.Of<ILength>(), Mock.Of<ILength>());
        parent.AppendChild(child3);

        LayoutAllCoordinates(parent);
        LayoutAllCoordinates(child1);
        LayoutAllCoordinates(child2);
        
        SideOfPrevious sideOfPrevious = new(side, offset);
        sideOfPrevious.Initialize(child3, axis);
        if (useTryCalculate)
        {
            sideOfPrevious.TryCalculate().Should().Be(result.Some());
        }
        else
        {
            sideOfPrevious.Calculate().Should().Be(result);
        }
    }


    // Support
    //----------------------------------------------------------------------------------------------
    
    private static void LayoutAllCoordinates(Cell cell)
    {
        foreach (Coordinate coordinate in cell.GetCoordinates())
        {
            coordinate.Calculate();
        }
    }
    
    private static ILength Len(Single value)
    {
        Mock<ILength> length = new();
        length.Setup(a => a.Calculate()).Returns(value);
        return length.Object;
    }
}
