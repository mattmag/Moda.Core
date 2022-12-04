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

public class SizeOfParentTests
{
    // Calculate() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void CalculateShouldReturnDistanceBetweenParentCoordinatesOnSameAxis()
    {
        Cell parent = new(Mock.Of<IHoneyComb>(), Len(50), Len(200), Len(20), Len(300));
        
        SizeOfParent sizeOfX = new();
        SizeOfParent sizeOfY = new();
        Cell child = new(Mock.Of<IHoneyComb>(), Len(0), sizeOfX, Len(0), sizeOfY);
        parent.AppendChild(child);
        
        LayoutAllCoordinates(parent);
        sizeOfX.Calculate().Should().Be(150);
        sizeOfY.Calculate().Should().Be(280);
    }
    
    
    [Test]
    public void CalculateShouldReflectNewParent()
    {
        Cell parent1 = new(Mock.Of<IHoneyComb>(), Len(50), Len(200), Len(20), Len(300));
        Cell parent2 = new(Mock.Of<IHoneyComb>(), Len(10), Len(60), Len(5), Len(20));
        
        SizeOfParent sizeOfX = new();
        SizeOfParent sizeOfY = new();
        Cell child = new(Mock.Of<IHoneyComb>(), Len(0), sizeOfX, Len(0), sizeOfY);
        parent1.AppendChild(child);
        
        LayoutAllCoordinates(parent1);
        LayoutAllCoordinates(parent2);

        sizeOfX.Calculate();
        sizeOfY.Calculate();
        
        parent2.AppendChild(child);
        
        sizeOfX.Calculate().Should().Be(50);
        sizeOfY.Calculate().Should().Be(15);
    }


    // Prerequisites TEsts
    //----------------------------------------------------------------------------------------------


    [Test]
    public void PrerequisitesShouldBeParentCoordinatesForAxis()
    {
        Cell parent = new(Mock.Of<IHoneyComb>(), Len(50), Len(200), Len(20), Len(300));

        SizeOfParent sizeOfX = new();
        SizeOfParent sizeOfY = new();
        Cell child = new(Mock.Of<IHoneyComb>(), Len(0), sizeOfX, Len(0), sizeOfY);
        
        parent.AppendChild(child);
        
        sizeOfX.Prerequisites.Should().BeEquivalentTo(parent.XBoundary.GetCoordinates());
        sizeOfY.Prerequisites.Should().BeEquivalentTo(parent.YBoundary.GetCoordinates());
    }


    [Test]
    public void ChangeToParentShouldUpdatePrerequisites()
    {
        Cell parent1 = new(Mock.Of<IHoneyComb>(), Len(50), Len(200), Len(20), Len(300));
        Cell parent2 = new(Mock.Of<IHoneyComb>(), Len(10), Len(60), Len(5), Len(20));

        SizeOfParent sizeOfX = new();
        SizeOfParent sizeOfY = new();
        Cell child = new(Mock.Of<IHoneyComb>(), Len(0), sizeOfX, Len(0), sizeOfY);
        parent1.AppendChild(child);

        parent2.AppendChild(child);
        sizeOfX.Prerequisites.Should().BeEquivalentTo(parent2.XBoundary.GetCoordinates());
        sizeOfY.Prerequisites.Should().BeEquivalentTo(parent2.YBoundary.GetCoordinates());
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
    
    private Length Len(Single value)
    {
        Mock<Length> length = new();
        length.Setup(a => a.Calculate()).Returns(value);
        return length.Object;
    }
    
}
