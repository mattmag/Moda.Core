// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moda.Core.Entity;
using Moda.Core.Lengths;
using Moda.Core.UI.Builders;
using Moq;
using NUnit.Framework;
using Optional;
using Optional.Unsafe;
using AssertionHelper = Moda.Core.Support.AssertionHelper;

namespace Moda.Core.UI;

public class HiveTests
{
    // NewCell() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void NewCellsShouldBeAddedToEntityManager()
    {
        Mock<IEntityManager> entityManager = new();
        Hive hive = new(entityManager.Object);
        
        entityManager.Invocations.Clear();
        Cell cell = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<Length>())
            .AppendTo(hive.Root));
        
        entityManager.Verify(a => a.AddEntity(It.Is<IEnumerable<Object>>(
                c => AssertionHelper.ToPredicate(() => c.Should().BeEquivalentTo(new[] { cell }, ""
            )))),
            Times.Once);
    }
    
    [Test]
    public void NewCellsShouldBeAddedToEntityManagerWithComponents()
    {
        Mock<IEntityManager> entityManager = new();
        Hive hive = new(entityManager.Object);

        Object componentA = new();
        Object componentB = new();
        
        entityManager.Invocations.Clear();
        Cell cell = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<Length>())
            .WithComponents(componentA, componentB)
            .AppendTo(hive.Root));
        
        entityManager.Verify(a => a.AddEntity(It.Is<IEnumerable<Object>>(
                c => AssertionHelper.ToPredicate(() => c.Should().BeEquivalentTo(
                    new[] { cell, componentA, componentB }, ""
            )))),
            Times.Once);
    }


    [Test]
    public void NewCellShouldAppendParent()
    {
        Mock<IEntityManager> entityManager = new();
        Hive hive = new(entityManager.Object);
        
        entityManager.Invocations.Clear();
        Cell cell1 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<Length>())
            .AppendTo(hive.Root));
        
        Cell cell2 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<Length>())
            .AppendTo(hive.Root));
        
        Cell cell3 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<Length>())
            .AppendTo(cell2));
        
        Cell cell4 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<Length>())
            .AppendTo(cell2));

        hive.Root.Children.Should().ContainInOrder(cell1, cell2);
        cell2.Children.Should().ContainInOrder(cell3, cell4);
    }
    
    [Test]
    public void NewCellShouldInsertAtParentBeforePeer()
    {
        Mock<IEntityManager> entityManager = new();
        Hive hive = new(entityManager.Object);
        
        entityManager.Invocations.Clear();
        Cell cell1 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<Length>())
            .AppendTo(hive.Root));
        cell1.DebugName = "cell1";
        
        Cell cell2 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<Length>())
            .AppendTo(cell1));
        cell2.DebugName = "cell2";
        
        Cell cell3 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<Length>())
            .InsertBefore(cell2));
        cell3.DebugName = "cell3";

        cell1.Children.Should().ContainInOrder(cell3, cell2);
    }
    
    [Test]
    public void NewCellShouldInsertAtParentAfterPeer()
    {
        Mock<IEntityManager> entityManager = new();
        Hive hive = new(entityManager.Object);
        
        entityManager.Invocations.Clear();
        Cell cell1 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<Length>())
            .AppendTo(hive.Root));
        cell1.DebugName = "cell1";
        
        Cell cell2 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<Length>())
            .AppendTo(cell1));
        cell2.DebugName = "cell2";
        
        Cell cell3 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<Length>())
            .AppendTo(cell1));
        cell3.DebugName = "cell3";
        
        Cell cell4 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<Length>())
            .InsertAfter(cell2));
        cell4.DebugName = "cell4";

        cell1.Children.Should().ContainInOrder(cell2, cell4, cell3);
    }
    
    
    [Test]
    public void NewCellShouldInsertAtIndex()
    {
        Mock<IEntityManager> entityManager = new();
        Hive hive = new(entityManager.Object);
        
        entityManager.Invocations.Clear();
        Cell cell1 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<Length>())
            .AppendTo(hive.Root));
        cell1.DebugName = "cell1";
        
        Cell cell2 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<Length>())
            .AppendTo(hive.Root));
        cell2.DebugName = "cell2";
        
        Cell cell3 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<Length>())
            .AppendTo(hive.Root));
        cell3.DebugName = "cell3";
        
        Cell cell4 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<Length>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<Length>())
            .InsertAt(hive.Root, 2));
        cell4.DebugName = "cell4";

        hive.Root.Children.Should().ContainInOrder(cell1, cell2, cell4, cell3);
    }

    private class MockRecipe : IReadyForConstruction
    {
        public BoundariesRecipe Boundaries { get; } = new();
        public CompositionRecipe Composition { get; } = new();
        
        public CellRecipe GetRecipe()
        {
            return new CellRecipe(this.Boundaries, this.Composition);
        }
    }

    [Test]
    public void NewCellShouldSetCoordinateCalculationsToSameInstanceAsRecipe()
    {
        Mock<IEntityManager> entityManager = new();
        Hive hive = new(entityManager.Object);

        MockRecipe recipe = new();
        recipe.Boundaries.XBoundary.Alpha = new Pixels(10).Some<Length>();
        recipe.Boundaries.XBoundary.Beta = new Pixels(20).Some<Length>();
        recipe.Boundaries.YBoundary.Alpha = new Pixels(30).Some<Length>();
        recipe.Boundaries.YBoundary.Beta = new Pixels(40).Some<Length>();

        Cell cell = hive.NewCell(a => recipe);
        cell.XBoundary.AlphaCoordinate.Calculation.Should()
            .BeSameAs(recipe.Boundaries.XBoundary.Alpha.ValueOrFailure());
        cell.XBoundary.BetaCoordinate.Calculation.Should()
            .BeSameAs(recipe.Boundaries.XBoundary.Beta.ValueOrFailure());
        cell.YBoundary.AlphaCoordinate.Calculation.Should()
            .BeSameAs(recipe.Boundaries.YBoundary.Alpha.ValueOrFailure());
        cell.YBoundary.BetaCoordinate.Calculation.Should()
            .BeSameAs(recipe.Boundaries.YBoundary.Beta.ValueOrFailure());

    }
}
