// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using FluentAssertions;
using Moda.Core.Entity;
using Moda.Core.UI;
using Moda.Core.UI.Builders;
using Moq;
using NUnit.Framework;
using Optional;

namespace Moda.Core.Tests.UI;

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

        Mock<ICalculation> xAlphaLength = new();
        xAlphaLength.Setup(a => a.Calculate()).Returns(xLength);

        Mock<ICalculation> yAlphaLength = new();
        yAlphaLength.Setup(a => a.Calculate()).Returns(yLength);
        
        Cell parent = new(Mock.Of<IHoneyComb>(),
            xAlphaLength.Object, Mock.Of<ICalculation>(),
            yAlphaLength.Object, Mock.Of<ICalculation>());
        parent.XBoundary.AlphaCoordinate.Tare = xTare.Some();
        parent.XBoundary.AlphaCoordinate.Calculate();
        
        parent.YBoundary.AlphaCoordinate.Tare = yTare.Some();
        parent.YBoundary.AlphaCoordinate.Calculate();
        
        Cell child = new(Mock.Of<IHoneyComb>(),
            Mock.Of<ICalculation>(),Mock.Of<ICalculation>(),
            Mock.Of<ICalculation>(),Mock.Of<ICalculation>());
        
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

        Mock<ICalculation> xAlphaLength = new();
        xAlphaLength.Setup(a => a.Calculate()).Returns(xLength);

        Mock<ICalculation> yAlphaLength = new();
        yAlphaLength.Setup(a => a.Calculate()).Returns(yLength);
        
        Cell parent = new(Mock.Of<IHoneyComb>(),
            xAlphaLength.Object, Mock.Of<ICalculation>(),
            yAlphaLength.Object, Mock.Of<ICalculation>());
        parent.XBoundary.AlphaCoordinate.Tare = xTare.Some();
        parent.XBoundary.AlphaCoordinate.Calculate();
        
        parent.YBoundary.AlphaCoordinate.Tare = yTare.Some();
        parent.YBoundary.AlphaCoordinate.Calculate();
        
        Cell child = new(Mock.Of<IHoneyComb>(),
            Mock.Of<ICalculation>(),Mock.Of<ICalculation>(),
            Mock.Of<ICalculation>(),Mock.Of<ICalculation>());
        
        parent.AppendChild(child);
        
        parent.RemoveChild(child);
        child.XBoundary.AlphaCoordinate.Tare.Should().Be(Option.None<Single>());
        child.XBoundary.BetaCoordinate.Tare.Should().Be(Option.None<Single>());
        child.YBoundary.AlphaCoordinate.Tare.Should().Be(Option.None<Single>());
        child.YBoundary.BetaCoordinate.Tare.Should().Be(Option.None<Single>());
    }


    // CreateChild() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void CreateChildShouldAppendToChildren()
    {
        Cell cell = new(new Hive(Mock.Of<IEntityManager>()),
            Mock.Of<ICalculation>(), Mock.Of<ICalculation>(),
            Mock.Of<ICalculation>(), Mock.Of<ICalculation>());
        Cell child = cell.CreateChild(a => a
            .AnchorAt(Horizontal.Left)
            .WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top)
            .WithHeight(Mock.Of<Length>()));
        cell.Children.Should().BeEquivalentTo(new[] { child });
    }
    
    [Test]
    public void CreateChildShouldInsertChildrenAfterPeer()
    {
        Cell cell = new(new Hive(Mock.Of<IEntityManager>()),
            Mock.Of<ICalculation>(), Mock.Of<ICalculation>(),
            Mock.Of<ICalculation>(), Mock.Of<ICalculation>());
        
        Cell childA = cell.CreateChild(a => a
            .AnchorAt(Horizontal.Left)
            .WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top)
            .WithHeight(Mock.Of<Length>()));
        
        Cell childB = cell.CreateChild(a => a
            .AnchorAt(Horizontal.Left)
            .WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top)
            .WithHeight(Mock.Of<Length>()));
        
        Cell childC = cell.CreateChild(a => a
            .AnchorAt(Horizontal.Left)
            .WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top)
            .WithHeight(Mock.Of<Length>()));
        
        Cell childD = cell.CreateChild(a => a
            .InsertAfter(childB)
            .AnchorAt(Horizontal.Left)
            .WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top)
            .WithHeight(Mock.Of<Length>()));
        
        cell.Children.Should().BeEquivalentTo(new[] { childA, childB, childD, childC });
    }
    
    
    [Test]
    public void CreateChildShouldInsertChildrenBeforePeer()
    {
        Cell cell = new(new Hive(Mock.Of<IEntityManager>()),
            Mock.Of<ICalculation>(), Mock.Of<ICalculation>(),
            Mock.Of<ICalculation>(), Mock.Of<ICalculation>());
        
        Cell childA = cell.CreateChild(a => a
            .AnchorAt(Horizontal.Left)
            .WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top)
            .WithHeight(Mock.Of<Length>()));
        
        Cell childB = cell.CreateChild(a => a
            .AnchorAt(Horizontal.Left)
            .WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top)
            .WithHeight(Mock.Of<Length>()));
        
        Cell childC = cell.CreateChild(a => a
            .AnchorAt(Horizontal.Left)
            .WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top)
            .WithHeight(Mock.Of<Length>()));
        
        Cell childD = cell.CreateChild(a => a
            .InsertBefore(childB)
            .AnchorAt(Horizontal.Left)
            .WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top)
            .WithHeight(Mock.Of<Length>()));
        
        cell.Children.Should().BeEquivalentTo(new[] { childA, childD, childB, childC });
    }
    
    [Test]
    public void CreateChildShouldInsertChildrenAtIndex()
    {
        Cell cell = new(new Hive(Mock.Of<IEntityManager>()),
            Mock.Of<ICalculation>(), Mock.Of<ICalculation>(),
            Mock.Of<ICalculation>(), Mock.Of<ICalculation>());
        
        Cell childA = cell.CreateChild(a => a
            .AnchorAt(Horizontal.Left)
            .WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top)
            .WithHeight(Mock.Of<Length>()));
        
        Cell childB = cell.CreateChild(a => a
            .AnchorAt(Horizontal.Left)
            .WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top)
            .WithHeight(Mock.Of<Length>()));
        
        Cell childC = cell.CreateChild(a => a
            .AnchorAt(Horizontal.Left)
            .WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top)
            .WithHeight(Mock.Of<Length>()));
        
        Cell childD = cell.CreateChild(a => a
            .InsertAt(1)
            .AnchorAt(Horizontal.Left)
            .WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top)
            .WithHeight(Mock.Of<Length>()));
        
        cell.Children.Should().BeEquivalentTo(new[] { childA, childD, childB, childC });
    }
    
    // GetCoordinates() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void GetCoordinatesShouldReturnAllCoordinates()
    {
        Cell cell = new(Mock.Of<IHoneyComb>(),
            Mock.Of<ICalculation>(), Mock.Of<ICalculation>(),
            Mock.Of<ICalculation>(), Mock.Of<ICalculation>());
        cell.GetCoordinates().Should()
            .BeEquivalentTo(new[]
                    {
                        cell.XBoundary.AlphaCoordinate, cell.XBoundary.BetaCoordinate,
                        cell.YBoundary.AlphaCoordinate, cell.YBoundary.BetaCoordinate
                    },
                a => a.WithStrictOrdering());
    }


    // DebugName Tests
    //----------------------------------------------------------------------------------------------
    [Test]
    public void SetDebugNameShouldSetCoordinateDebugNames()
    {
        Cell cell = new(Mock.Of<IHoneyComb>(),
            Mock.Of<ICalculation>(), Mock.Of<ICalculation>(),
            Mock.Of<ICalculation>(), Mock.Of<ICalculation>());
        cell.DebugName = "TestCell";
        cell.DebugName.Should().Be("TestCell");
        cell.XBoundary.AlphaCoordinate.DebugName.Should().Be("TestCell.X.Alpha");
        cell.XBoundary.BetaCoordinate.DebugName.Should().Be("TestCell.X.Beta");
        cell.YBoundary.AlphaCoordinate.DebugName.Should().Be("TestCell.Y.Alpha");
        cell.YBoundary.AlphaCoordinate.DebugName.Should().Be("TestCell.Y.Alpha");
    }
}
