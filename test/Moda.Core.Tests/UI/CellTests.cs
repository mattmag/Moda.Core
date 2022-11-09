// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Optional;

namespace Moda.Core.UI;

public class CellTests
{
    // AppendChild() Methods
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void AppendChildShouldUpdateChildCoordinatesTaresBasedOnAlphaAbsolutes()
    {
        const Single xTare = 2.5f;
        const Single xLength = 13;
        const Single yTare = 7.1f;
        const Single yLength = 19;

        Mock<ICalculable> xAlphaLength = new();
        xAlphaLength.Setup(a => a.Calculate()).Returns(xLength);

        Mock<ICalculable> yAlphaLength = new();
        yAlphaLength.Setup(a => a.Calculate()).Returns(yLength);
        
        Cell parent = new();
        parent.XBoundary.AlphaCoordinate.Recipe = xAlphaLength.Object;
        parent.XBoundary.AlphaCoordinate.Tare = xTare.Some();
        parent.XBoundary.AlphaCoordinate.Calculate();
        
        parent.YBoundary.AlphaCoordinate.Recipe = yAlphaLength.Object;
        parent.YBoundary.AlphaCoordinate.Tare = yTare.Some();
        parent.YBoundary.AlphaCoordinate.Calculate();
        
        Cell child = new();
        
        parent.AppendChild(child);
        
        child.XBoundary.AlphaCoordinate.Tare.Should().Be((xTare + xLength).Some());
        child.XBoundary.BetaCoordinate.Tare.Should().Be((xTare + xLength).Some());
        child.YBoundary.AlphaCoordinate.Tare.Should().Be((yTare + yLength).Some());
        child.YBoundary.BetaCoordinate.Tare.Should().Be((yTare + yLength).Some());
    }




    // RemoveChild() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void RemoveChildShouldUpdateChildCoordinatesTaresToNone()
    {
        const Single xTare = 2.5f;
        const Single xLength = 13;
        const Single yTare = 7.1f;
        const Single yLength = 19;

        Mock<ICalculable> xAlphaLength = new();
        xAlphaLength.Setup(a => a.Calculate()).Returns(xLength);

        Mock<ICalculable> yAlphaLength = new();
        yAlphaLength.Setup(a => a.Calculate()).Returns(yLength);
        
        Cell parent = new();
        parent.XBoundary.AlphaCoordinate.Recipe = xAlphaLength.Object;
        parent.XBoundary.AlphaCoordinate.Tare = xTare.Some();
        parent.XBoundary.AlphaCoordinate.Calculate();
        
        parent.YBoundary.AlphaCoordinate.Recipe = yAlphaLength.Object;
        parent.YBoundary.AlphaCoordinate.Tare = yTare.Some();
        parent.YBoundary.AlphaCoordinate.Calculate();
        
        Cell child = new();
        
        parent.AppendChild(child);
        
        parent.RemoveChild(child);
        child.XBoundary.AlphaCoordinate.Tare.Should().Be(Option.None<Single>());
        child.XBoundary.BetaCoordinate.Tare.Should().Be(Option.None<Single>());
        child.YBoundary.AlphaCoordinate.Tare.Should().Be(Option.None<Single>());
        child.YBoundary.BetaCoordinate.Tare.Should().Be(Option.None<Single>());
    }
    
}
