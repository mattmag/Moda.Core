// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using FluentAssertions;
using Moda.Core.UI;
using Moda.Core.UI.Lengths;
using Moq;
using NUnit.Framework;

namespace Moda.Core.Tests.UI.Lengths;

public class SizeOfChildrenTests
{
    // Calculate() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void CalculateShouldReturnMaxOfChildBetaCoordinates()
    {
        SizeOfChildren sizeOfX = new();
        SizeOfChildren sizeOfY = new();
        Cell cell = new(Mock.Of<IHoneyComb>(),
            Mock.Of<ICalculation>(), sizeOfX,
            Mock.Of<ICalculation>(), sizeOfY);

        Cell child1 = new(Mock.Of<IHoneyComb>(),
            Mock.Of<ICalculation>(), Len(3),
            Mock.Of<ICalculation>(), Len(6));
        cell.AppendChild(child1);
        
        Cell child2 = new(Mock.Of<IHoneyComb>(),
            Mock.Of<ICalculation>(), Len(5),
            Mock.Of<ICalculation>(), Len(4));
        cell.AppendChild(child2);
        
        Cell child3 = new(Mock.Of<IHoneyComb>(),
            Mock.Of<ICalculation>(), Len(1),
            Mock.Of<ICalculation>(), Len(2));
        cell.AppendChild(child3);
        
        LayoutAllCoordinates(child1);
        LayoutAllCoordinates(child2);
        LayoutAllCoordinates(child3);

        sizeOfX.Calculate().Should().Be(5);
        sizeOfY.Calculate().Should().Be(6);

    }


    [Test]
    public void CalculateShouldReflectAddedChildren()
    {
        SizeOfChildren sizeOfX = new();
        SizeOfChildren sizeOfY = new();
        Cell cell = new(Mock.Of<IHoneyComb>(),
            Mock.Of<ICalculation>(), sizeOfX,
            Mock.Of<ICalculation>(), sizeOfY);

        Cell child1 = new(Mock.Of<IHoneyComb>(),
            Mock.Of<ICalculation>(), Len(3),
            Mock.Of<ICalculation>(), Len(6));
        cell.AppendChild(child1);
        LayoutAllCoordinates(child1);
        
        sizeOfX.Calculate().Should().Be(3);
        sizeOfY.Calculate().Should().Be(6);
        
        Cell child2 = new(Mock.Of<IHoneyComb>(),
            Mock.Of<ICalculation>(), Len(100),
            Mock.Of<ICalculation>(), Len(200));
        cell.AppendChild(child2);
        LayoutAllCoordinates(child2);
        
        sizeOfX.Calculate().Should().Be(100);
        sizeOfY.Calculate().Should().Be(200);
    }
    
    [Test]
    public void CalculateShouldReflectRemovedChildren()
    {
        SizeOfChildren sizeOfX = new();
        SizeOfChildren sizeOfY = new();
        Cell cell = new(Mock.Of<IHoneyComb>(),
            Mock.Of<ICalculation>(), sizeOfX,
            Mock.Of<ICalculation>(), sizeOfY);

        Cell child1 = new(Mock.Of<IHoneyComb>(),
            Mock.Of<ICalculation>(), Len(3),
            Mock.Of<ICalculation>(), Len(6));
        cell.AppendChild(child1);
        LayoutAllCoordinates(child1);
        
        Cell child2 = new(Mock.Of<IHoneyComb>(),
            Mock.Of<ICalculation>(), Len(100),
            Mock.Of<ICalculation>(), Len(200));
        cell.AppendChild(child2);
        LayoutAllCoordinates(child2);
        
        sizeOfX.Calculate().Should().Be(100);
        sizeOfY.Calculate().Should().Be(200);
        
        cell.RemoveChild(child2);
        
        sizeOfX.Calculate().Should().Be(3);
        sizeOfY.Calculate().Should().Be(6);
    }


    // Prerequisites Tests
    //----------------------------------------------------------------------------------------------


    [Test]
    public void PrerequisitesShouldBeAllChildrenBetaCoordinates()
    {
        SizeOfChildren sizeOfX = new();
        SizeOfChildren sizeOfY = new();
        Cell cell = new(Mock.Of<IHoneyComb>(),
            Mock.Of<ICalculation>(), sizeOfX,
            Mock.Of<ICalculation>(), sizeOfY);
        
        Cell child1 = new(Mock.Of<IHoneyComb>(),
            Mock.Of<ICalculation>(), Len(3),
            Mock.Of<ICalculation>(), Len(6));
        cell.AppendChild(child1);
        
        Cell child2 = new(Mock.Of<IHoneyComb>(),
            Mock.Of<ICalculation>(), Len(5),
            Mock.Of<ICalculation>(), Len(4));
        cell.AppendChild(child2);

        sizeOfX.Prerequisites.Should().BeEquivalentTo(new[]
            {
                child1.XBoundary.BetaCoordinate, child2.XBoundary.BetaCoordinate
            });
        sizeOfY.Prerequisites.Should().BeEquivalentTo(new[]
            {
                child1.YBoundary.BetaCoordinate, child2.YBoundary.BetaCoordinate
            });
    }
    
    
    [Test]
    public void PrerequisitesShouldBeReflectChangesToChildren()
    {
        SizeOfChildren sizeOfX = new();
        SizeOfChildren sizeOfY = new();
        Cell cell = new(Mock.Of<IHoneyComb>(),
            Mock.Of<ICalculation>(), sizeOfX,
            Mock.Of<ICalculation>(), sizeOfY);
        
        Cell child1 = new(Mock.Of<IHoneyComb>(),
            Mock.Of<ICalculation>(), Len(3),
            Mock.Of<ICalculation>(), Len(6));
        cell.AppendChild(child1);
        
        Cell child2 = new(Mock.Of<IHoneyComb>(),
            Mock.Of<ICalculation>(), Len(5),
            Mock.Of<ICalculation>(), Len(4));
        cell.AppendChild(child2);
        
        sizeOfX.Prerequisites.Should().BeEquivalentTo(new[]
            {
                child1.XBoundary.BetaCoordinate, child2.XBoundary.BetaCoordinate
            });
        sizeOfY.Prerequisites.Should().BeEquivalentTo(new[]
            {
                child1.YBoundary.BetaCoordinate, child2.YBoundary.BetaCoordinate
            });
        
        Cell child3 = new(Mock.Of<IHoneyComb>(),
            Mock.Of<ICalculation>(), Len(5),
            Mock.Of<ICalculation>(), Len(4));
        cell.AppendChild(child3);
        
        cell.RemoveChild(child2);

        sizeOfX.Prerequisites.Should().BeEquivalentTo(new[]
            {
                child1.XBoundary.BetaCoordinate, child3.XBoundary.BetaCoordinate
            });
        sizeOfY.Prerequisites.Should().BeEquivalentTo(new[]
            {
                child1.YBoundary.BetaCoordinate, child3.YBoundary.BetaCoordinate
            });
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
