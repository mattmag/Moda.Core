// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using FluentAssertions;
using FluentAssertions.Events;
using Moda.Core.UI;
using Moda.Core.UI.Lengths;
using Moq;
using NUnit.Framework;

namespace Moda.Core.Tests.UI.Lengths;

public class PercentOfParentTests
{
    // Constructor Tests
    //----------------------------------------------------------------------------------------------
   
    [Test]
    public void ConstructorShouldSetPercent()
    {
        PercentOfParent percentOf = new(80);
        percentOf.Percent.Should().Be(80);
    }
    
    
    // Percent Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void ValueSetterShouldSetNewValue()
    {
        PercentOfParent percentOf = new(80);
        percentOf.Percent = 45;
        percentOf.Percent.Should().Be(45);
    }
    
    [Test]
    public void ValueSetterShouldFireValueInvalidatedWhenChanged()
    {
        // ReSharper disable once UseObjectOrCollectionInitializer
        PercentOfParent percentOf = new(13);
        using IMonitor<PercentOfParent>? monitor = percentOf.Monitor();
        percentOf.Percent = 27;
        monitor.Should().Raise(nameof(Coordinate.ValueInvalidated))
            .WithSender(percentOf);
    }
    
    [Test]
    public void ValueSetterShouldNotFireValueInvalidatedWhenNotChanged()
    {
        // ReSharper disable once UseObjectOrCollectionInitializer
        PercentOfParent percentOf = new(13);
        using IMonitor<PercentOfParent>? monitor = percentOf.Monitor();
        percentOf.Percent = 13;
        monitor.Should().NotRaise(nameof(Coordinate.ValueInvalidated));
    }
    
    
    // Calculate() Tests
    //----------------------------------------------------------------------------------------------
    
    [TestCase(100, 150, 280)]
    [TestCase(50, 75, 140)]
    [TestCase(25, 37.5f, 70)]
    [TestCase(200, 300, 560)]
    public void CalculateShouldReturnPercentOfParentAxisDelta(Single percent, Single expectedX,
        Single expectedY)
    {
        Cell parent = new(Mock.Of<IHoneyComb>(), Len(50), Len(200), Len(20), Len(300));
        
        PercentOfParent percentOfX = new(percent);
        PercentOfParent percentOfY = new(percent);
        Cell child = new(Mock.Of<IHoneyComb>(), Len(0), percentOfX, Len(0), percentOfY);
        parent.AppendChild(child);
        
        LayoutAllCoordinates(parent);
        percentOfX.Calculate().Should().Be(expectedX);
        percentOfY.Calculate().Should().Be(expectedY);
    }
    
    
    [Test]
    public void CalculateShouldReflectNewParent()
    {
        Cell parent1 = new(Mock.Of<IHoneyComb>(), Len(50), Len(200), Len(20), Len(300));
        Cell parent2 = new(Mock.Of<IHoneyComb>(), Len(10), Len(60), Len(5), Len(20));
        
        PercentOfParent percentOfX = new(100);
        PercentOfParent percentOfY = new(100);
        Cell child = new(Mock.Of<IHoneyComb>(), Len(0), percentOfX, Len(0), percentOfY);
        parent1.AppendChild(child);
        
        LayoutAllCoordinates(parent1);
        LayoutAllCoordinates(parent2);

        percentOfX.Calculate();
        percentOfY.Calculate();
        
        parent2.AppendChild(child);
        
        percentOfX.Calculate().Should().Be(50);
        percentOfY.Calculate().Should().Be(15);
    }


    // Prerequisites Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void PrerequisitesShouldBeParentCoordinatesForAxis()
    {
        Cell parent = new(Mock.Of<IHoneyComb>(), Len(50), Len(200), Len(20), Len(300));

        PercentOfParent percentOfX = new(100);
        PercentOfParent percentOfY = new(100);
        Cell child = new(Mock.Of<IHoneyComb>(), Len(0), percentOfX, Len(0), percentOfY);
        
        parent.AppendChild(child);
        
        percentOfX.Prerequisites.Should().BeEquivalentTo(parent.XBoundary.GetCoordinates());
        percentOfY.Prerequisites.Should().BeEquivalentTo(parent.YBoundary.GetCoordinates());
    }


    [Test]
    public void ChangeToParentShouldUpdatePrerequisites()
    {
        Cell parent1 = new(Mock.Of<IHoneyComb>(), Len(50), Len(200), Len(20), Len(300));
        Cell parent2 = new(Mock.Of<IHoneyComb>(), Len(10), Len(60), Len(5), Len(20));

        PercentOfParent percentOfX = new(100);
        PercentOfParent percentOfY = new(100);
        Cell child = new(Mock.Of<IHoneyComb>(), Len(0), percentOfX, Len(0), percentOfY);
        parent1.AppendChild(child);

        parent2.AppendChild(child);
        percentOfX.Prerequisites.Should().BeEquivalentTo(parent2.XBoundary.GetCoordinates());
        percentOfY.Prerequisites.Should().BeEquivalentTo(parent2.YBoundary.GetCoordinates());
    }



    // Support
    //----------------------------------------------------------------------------------------------


    private void LayoutAllCoordinates(Cell cell)
    {
        foreach (Coordinate coordinate in cell.GetCoordinates())
        {
            coordinate.Calculate();
        }
    }
    
    private ILength Len(Single value)
    {
        Mock<ILength> length = new();
        length.Setup(a => a.Calculate()).Returns(value);
        return length.Object;
    }
    
}
