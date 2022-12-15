// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using Moda.Core.Entity;
using Moda.Core.UI;
using Moda.Core.UI.Builders;
using Moda.Core.Utility.Data;
using Moda.Core.Utility.Geometry;
using Moq;
using NUnit.Framework;
using Optional;
using AssertionHelper = Moda.Core.Tests.Support.AssertionHelper;

namespace Moda.Core.Tests.UI;

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
            .AppendTo(hive.Root)
            .AnchorLeft().WithWidth(Mock.Of<Length>())
            .AnchorUp().WithHeight(Mock.Of<Length>()));
        
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
            .AppendTo(hive.Root)
            .AnchorLeft().WithWidth(Mock.Of<Length>())
            .AnchorUp().WithHeight(Mock.Of<Length>())
            .WithComponents(componentA, componentB));
        
        entityManager.Verify(a => a.AddEntity(It.Is<IEnumerable<Object>>(
                c => AssertionHelper.ToPredicate(() => c.Should().BeEquivalentTo(
                    new[] { cell, componentA, componentB }, ""
            )))),
            Times.Once);
    }


    [Test]
    public void NewCellShouldAppendToParent()
    {
        Mock<IEntityManager> entityManager = new();
        Hive hive = new(entityManager.Object);
        
        entityManager.Invocations.Clear();
        Cell cell1 = hive.NewCell(a => a
            .AppendTo(hive.Root)
            .AnchorLeft().WithWidth(Mock.Of<Length>())
            .AnchorUp().WithHeight(Mock.Of<Length>()));
        
        Cell cell2 = hive.NewCell(a => a
            .AppendTo(hive.Root)
            .AnchorLeft().WithWidth(Mock.Of<Length>())
            .AnchorUp().WithHeight(Mock.Of<Length>()));
        
        Cell cell3 = hive.NewCell(a => a
            .AppendTo(cell2)
            .AnchorLeft().WithWidth(Mock.Of<Length>())
            .AnchorUp().WithHeight(Mock.Of<Length>()));
        
        Cell cell4 = hive.NewCell(a => a
            .AppendTo(cell2)
            .AnchorLeft().WithWidth(Mock.Of<Length>())
            .AnchorUp().WithHeight(Mock.Of<Length>()));

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
            .AppendTo(hive.Root)
            .AnchorLeft().WithWidth(Mock.Of<Length>())
            .AnchorUp().WithHeight(Mock.Of<Length>()));
        cell1.DebugName = "cell1";
        
        Cell cell2 = hive.NewCell(a => a
            .AppendTo(cell1)
            .AnchorLeft().WithWidth(Mock.Of<Length>())
            .AnchorUp().WithHeight(Mock.Of<Length>()));
        cell2.DebugName = "cell2";
        
        Cell cell3 = hive.NewCell(a => a
            .InsertBefore(cell2)
            .AnchorLeft().WithWidth(Mock.Of<Length>())
            .AnchorUp().WithHeight(Mock.Of<Length>()));
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
            .AppendTo(hive.Root)
            .AnchorLeft().WithWidth(Mock.Of<Length>())
            .AnchorUp().WithHeight(Mock.Of<Length>()));
        cell1.DebugName = "cell1";
        
        Cell cell2 = hive.NewCell(a => a
            .AppendTo(cell1)
            .AnchorLeft().WithWidth(Mock.Of<Length>())
            .AnchorUp().WithHeight(Mock.Of<Length>()));
        cell2.DebugName = "cell2";
        
        Cell cell3 = hive.NewCell(a => a
            .AppendTo(cell1)
            .AnchorLeft().WithWidth(Mock.Of<Length>())
            .AnchorUp().WithHeight(Mock.Of<Length>()));
        cell3.DebugName = "cell3";
        
        Cell cell4 = hive.NewCell(a => a
            .InsertAfter(cell2)
            .AnchorLeft().WithWidth(Mock.Of<Length>())
            .AnchorUp().WithHeight(Mock.Of<Length>()));
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
            .AppendTo(hive.Root)
            .AnchorLeft().WithWidth(Mock.Of<Length>())
            .AnchorUp().WithHeight(Mock.Of<Length>()));
        cell1.DebugName = "cell1";
        
        Cell cell2 = hive.NewCell(a => a
            .AppendTo(hive.Root)
            .AnchorLeft().WithWidth(Mock.Of<Length>())
            .AnchorUp().WithHeight(Mock.Of<Length>()));
        cell2.DebugName = "cell2";
        
        Cell cell3 = hive.NewCell(a => a
            .AppendTo(hive.Root)
            .AnchorLeft().WithWidth(Mock.Of<Length>())
            .AnchorUp().WithHeight(Mock.Of<Length>()));
        cell3.DebugName = "cell3";
        
        Cell cell4 = hive.NewCell(a => a
            .InsertAt(hive.Root, 2)
            .AnchorLeft().WithWidth(Mock.Of<Length>())
            .AnchorUp().WithHeight(Mock.Of<Length>()));
        cell4.DebugName = "cell4";

        hive.Root.Children.Should().ContainInOrder(cell1, cell2, cell4, cell3);
    }

    

    [Test]
    public void NewCellShouldSetCoordinateCalculationsToSameInstanceAsRecipe()
    {
        Mock<IEntityManager> entityManager = new();
        Hive hive = new(entityManager.Object);

        MockRecipe recipe = new(hive.Root);

        Cell cell = hive.NewCell(_ => recipe);
        cell.XBoundary.AlphaCoordinate.Calculation.Should().BeSameAs(recipe.XAlpha);
        cell.XBoundary.BetaCoordinate.Calculation.Should().BeSameAs(recipe.XBeta);
        cell.YBoundary.AlphaCoordinate.Calculation.Should().BeSameAs(recipe.YAlpha);
        cell.YBoundary.BetaCoordinate.Calculation.Should().BeSameAs(recipe.YBeta);
    }
    
    
    [Test]
    public void NewCellShouldSetEntityID()
    {
        Mock<IEntityManager> entityManager = new();
        UInt64 expected = 862;
        entityManager.Setup(a => a.AddEntity(It.IsAny<IEnumerable<Object>>())).Returns(expected);
        Hive hive = new(entityManager.Object);

        MockRecipe recipe = new(hive.Root);

        Cell cell = hive.NewCell(_ => recipe);
        cell.EntityID.Should().Be(expected);
    }

    

    // Cell.RemoveChild() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void HiveShouldStopListeningToValueInvalidatedFromRemovedChildren()
    {
        Mock<IEntityManager> entityManager = new();
        Hive hive = new(entityManager.Object);

        MockRecipe recipe = new(hive.Root);
        Cell cell = hive.NewCell(_ => recipe);
        
        hive.Layout();
        recipe.ResetAllInvocations();
        
        hive.Root.RemoveChild(cell);
        recipe.XAlpha.RaiseInvalidated();
        recipe.XBeta.RaiseInvalidated();
        recipe.YAlpha.RaiseInvalidated();
        recipe.YBeta.RaiseInvalidated();
        
        hive.Layout();
        recipe.GetAllCalculateCalls().Should().BeEmpty();
    }
    
    [Test]
    public void HiveShouldStopListeningToPrerequisitesChangedFromRemovedChildren()
    {
        Mock<IEntityManager> entityManager = new();
        Hive hive = new(entityManager.Object);

        MockRecipe recipeA = new(hive.Root);
        Cell cellA = hive.NewCell(_ => recipeA);
        
        MockRecipe recipeB = new(hive.Root);
        Cell cellB = hive.NewCell(_ => recipeB);
        
        hive.Layout();
        recipeA.ResetAllInvocations();
        
        hive.Root.RemoveChild(cellA);
        recipeA.XAlpha.AddPrerequisites(cellB.XBoundary.AlphaCoordinate);
        recipeA.XBeta.AddPrerequisites(cellB.XBoundary.AlphaCoordinate);
        recipeA.YAlpha.AddPrerequisites(cellB.XBoundary.AlphaCoordinate);
        recipeA.YBeta.AddPrerequisites(cellB.XBoundary.AlphaCoordinate);
        
        hive.Layout();
        recipeA.GetAllCalculateCalls().Should().BeEmpty();
    }
    
    

    // ViewPort Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void ViewPortDimensionsShouldSetRootBoundaries()
    {
        Hive hive = new(Mock.Of<IEntityManager>())
        {
            ViewPortSize = new(1350, 1280),
        };

        // explicit 'layout'
        hive.Root.XBoundary.AlphaCoordinate.Calculate();
        hive.Root.XBoundary.BetaCoordinate.Calculate();
        hive.Root.YBoundary.AlphaCoordinate.Calculate();
        hive.Root.YBoundary.BetaCoordinate.Calculate();

        // test
        hive.Root.XBoundary.AlphaCoordinate.AbsoluteValue.Should().Be(0.0f.Some());
        hive.Root.XBoundary.AlphaCoordinate.RelativeValue.Should().Be(0.0f.Some());
        hive.Root.XBoundary.BetaCoordinate.AbsoluteValue.Should().Be(1350.0f.Some());
        hive.Root.XBoundary.BetaCoordinate.RelativeValue.Should().Be(1350.0f.Some());
        
        hive.Root.YBoundary.AlphaCoordinate.AbsoluteValue.Should().Be(0.0f.Some());
        hive.Root.YBoundary.AlphaCoordinate.RelativeValue.Should().Be(0.0f.Some());
        hive.Root.YBoundary.BetaCoordinate.AbsoluteValue.Should().Be(1280.0f.Some());
        hive.Root.YBoundary.BetaCoordinate.RelativeValue.Should().Be(1280.0f.Some());
    }


    [Test]
    public void ViewPortShouldReturnSetValue()
    {
        Hive hive = new(Mock.Of<IEntityManager>())
        {
            ViewPortSize = new(600, 400),
        };
        hive.ViewPortSize.Should().BeEquivalentTo(new Size2(600, 400));
    }


    // Layout() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void FirstLayoutCallShouldIncludeRoot()
    {
        Hive hive = new(Mock.Of<IEntityManager>());
        
        hive.Layout();
        
        hive.Root.XBoundary.AlphaCoordinate.AbsoluteValue.Should().NotBe(Option.None<Single>());
        hive.Root.XBoundary.AlphaCoordinate.RelativeValue.Should().NotBe(Option.None<Single>());
        hive.Root.XBoundary.BetaCoordinate.AbsoluteValue.Should().NotBe(Option.None<Single>());
        hive.Root.XBoundary.BetaCoordinate.RelativeValue.Should().NotBe(Option.None<Single>());
        
        hive.Root.YBoundary.AlphaCoordinate.AbsoluteValue.Should().NotBe(Option.None<Single>());
        hive.Root.YBoundary.AlphaCoordinate.RelativeValue.Should().NotBe(Option.None<Single>());
        hive.Root.YBoundary.BetaCoordinate.AbsoluteValue.Should().NotBe(Option.None<Single>());
        hive.Root.YBoundary.BetaCoordinate.RelativeValue.Should().NotBe(Option.None<Single>());
    }
    
    
    [Test]
    public void FirstLayoutCallShouldIncludeRootWithViewportSize()
    {
        Hive hive = new(Mock.Of<IEntityManager>())
        {
            ViewPortSize = new(1350, 1280),
        };
        
        hive.Layout();
        
        AssertBoundary(hive.Root, Result.Relative, 0, 1350, 0, 1280);
        AssertBoundary(hive.Root, Result.Absolute, 0, 1350, 0, 1280);
    }
    
    [Test]
    public void LayoutShouldUpdateRootWithNewViewportSize()
    {
        Hive hive = new(Mock.Of<IEntityManager>())
        {
            ViewPortSize = new(1350, 1280),
        };
        
        hive.Layout();

        hive.ViewPortSize = new(1350, 600);
        hive.Layout();
        AssertBoundary(hive.Root, Result.Relative, 0, 1350, 0, 600);
        AssertBoundary(hive.Root, Result.Absolute, 0, 1350, 0, 600);
        
        hive.ViewPortSize = new(400, 600);
        hive.Layout();
        AssertBoundary(hive.Root, Result.Relative, 0, 400, 0, 600);
        AssertBoundary(hive.Root, Result.Absolute, 0, 400, 0, 600);
        
        hive.ViewPortSize = new(2000, 1000);
        hive.Layout();
        AssertBoundary(hive.Root, Result.Relative, 0, 2000, 0, 1000);
        AssertBoundary(hive.Root, Result.Absolute, 0, 2000, 0, 1000);
    }


    [Test]
    public void LayoutShouldIncludeNewCells()
    {
        Hive hive = new(Mock.Of<IEntityManager>())
        {
            ViewPortSize = new(1200, 600),
        };
        
        MockRecipe mocked = new(hive.Root);
        hive.NewCell(_ => mocked);
        
        hive.Layout();
        mocked.XAlpha.VerifyLayoutPass();
        mocked.XBeta.VerifyLayoutPass();
        mocked.YAlpha.VerifyLayoutPass();
        mocked.YBeta.VerifyLayoutPass();
    }
    
    [Test]
    public void LayoutShouldNotIncludeUnchangedCoordinates()
    {
        Hive hive = new(Mock.Of<IEntityManager>())
        {
            ViewPortSize = new(1200, 600),
        };
        
        MockRecipe mocked = new(hive.Root);
        hive.NewCell(_ => mocked);
        
        hive.Layout();
        mocked.ResetAllInvocations();
        
        hive.Layout();
        mocked.XAlpha.VerifyNoLayoutPass();
        mocked.XBeta.VerifyNoLayoutPass();
        mocked.YAlpha.VerifyNoLayoutPass();
        mocked.YBeta.VerifyNoLayoutPass();
    }
    
    
    [Test]
    public void LayoutShouldIncludeInvalidatedCoordinates()
    {
        Hive hive = new(Mock.Of<IEntityManager>())
        {
            ViewPortSize = new(1200, 600),
        };
        
        MockRecipe mocked = new(hive.Root);
        hive.NewCell(_ => mocked);
        
        hive.Layout();
        mocked.ResetAllInvocations();
        
        mocked.XBeta.RaiseInvalidated();
        mocked.YAlpha.RaiseInvalidated();
        
        hive.Layout();
        mocked.XAlpha.VerifyNoLayoutPass();
        mocked.XBeta.VerifyLayoutPass();
        mocked.YAlpha.VerifyLayoutPass();
        mocked.YBeta.VerifyNoLayoutPass();
    }
    
    [Test]
    public void LayoutShouldIncludeCoordinatesWithPrerequisiteChanges()
    {
        Hive hive = new(Mock.Of<IEntityManager>())
        {
            ViewPortSize = new(1200, 600),
        };
        
        MockRecipe mockedA = new(hive.Root);
        Cell cellA = hive.NewCell(_ => mockedA);
        
        MockRecipe mockedB = new(hive.Root);
        hive.NewCell(_ => mockedB);
        
        hive.Layout();
        mockedA.ResetAllInvocations();
        mockedB.ResetAllInvocations();
        
        mockedB.XBeta.AddPrerequisites(cellA.XBoundary.AlphaCoordinate);
        mockedB.YAlpha.AddPrerequisites(cellA.YBoundary.BetaCoordinate);
        
        hive.Layout();
        mockedA.XAlpha.VerifyNoLayoutPass();
        mockedA.XBeta.VerifyNoLayoutPass();
        mockedA.YAlpha.VerifyNoLayoutPass();
        mockedA.YBeta.VerifyNoLayoutPass();
        
        mockedB.XAlpha.VerifyNoLayoutPass();
        mockedB.XBeta.VerifyLayoutPass();
        mockedB.YAlpha.VerifyLayoutPass();
        mockedB.YBeta.VerifyNoLayoutPass();
    }


    [Test]
    public void LayoutShouldProcessInOrderOfPrerequisites()
    {
        Hive hive = new(Mock.Of<IEntityManager>());
        
        Int32 callOrder = 0;
        Int32 getCallOrder() => ++callOrder;

        MockRecipe mockedA = new(hive.Root, getCallOrder);
        Cell cellA = hive.NewCell(_ => mockedA);
        
        MockRecipe mockedB = new(cellA, getCallOrder);
        
        mockedB.XAlpha.AddPrerequisites(cellA.XBoundary.AlphaCoordinate);
        mockedB.YAlpha.AddPrerequisites(cellA.YBoundary.AlphaCoordinate);
        mockedB.XBeta.AddPrerequisites(cellA.XBoundary.AlphaCoordinate,
            cellA.XBoundary.BetaCoordinate);
        mockedB.YBeta.AddPrerequisites(cellA.YBoundary.BetaCoordinate);

        hive.NewCell(_ => mockedB);
        
        hive.Layout();
        
        List<MockLength> results = mockedA.GetAllCalculateCalls()
            .Concat(mockedB.GetAllCalculateCalls())
            .OrderBy(a => a.Key)
            .Select(a => a.Value)
            .ToList();

        results.Should().ContainInOrder(mockedA.XAlpha, mockedB.XAlpha);
        results.Should().ContainInOrder(mockedA.YAlpha, mockedB.YAlpha);
        results.Should().ContainInOrder(mockedA.XAlpha, mockedB.XAlpha);
        results.Should().ContainInOrder(mockedA.XBeta, mockedB.XAlpha);
        results.Should().ContainInOrder(mockedA.YBeta, mockedB.YAlpha);
    }
    
    
    [Test]
    public void LayoutShouldProcessInOrderOfNewPrerequisites()
    {
        Hive hive = new(Mock.Of<IEntityManager>());
        
        Int32 callOrder = 0;
        Int32 getCallOrder() => ++callOrder;

        MockRecipe mockedA = new(hive.Root, getCallOrder);
        Cell cellA = hive.NewCell(_ => mockedA);
        
        MockRecipe mockedB = new(cellA, getCallOrder);
        hive.NewCell(_ => mockedB);
        
        mockedB.XAlpha.AddPrerequisites( cellA.XBoundary.AlphaCoordinate);
        mockedB.YAlpha.AddPrerequisites(cellA.YBoundary.AlphaCoordinate);
        mockedB.XBeta.AddPrerequisites(cellA.XBoundary.AlphaCoordinate,
            cellA.XBoundary.BetaCoordinate);
        mockedB.YBeta.AddPrerequisites(cellA.YBoundary.BetaCoordinate);
        
        hive.Layout();
        
        List<MockLength> results = mockedA.GetAllCalculateCalls()
            .Concat(mockedB.GetAllCalculateCalls())
            .OrderBy(a => a.Key)
            .Select(a => a.Value)
            .ToList();

        results.Should().ContainInOrder(mockedA.XAlpha, mockedB.XAlpha);
        results.Should().ContainInOrder(mockedA.YAlpha, mockedB.YAlpha);
        results.Should().ContainInOrder(mockedA.XAlpha, mockedB.XAlpha);
        results.Should().ContainInOrder(mockedA.XBeta, mockedB.XAlpha);
        results.Should().ContainInOrder(mockedA.YBeta, mockedB.YAlpha);
    }
    
    
    [Test]
    public void LayoutShouldProcessInOrderOfNewPrerequisitesInSubsequentCalls()
    {
        Hive hive = new(Mock.Of<IEntityManager>());
        
        Int32 callOrder = 0;
        Int32 getCallOrder() => ++callOrder;

        MockRecipe mockedA = new("CellA", hive.Root, getCallOrder);
        Cell cellA = hive.NewCell(_ => mockedA);
        
        MockRecipe mockedB = new("CellB", cellA, getCallOrder);
        Cell cellB = hive.NewCell(_ => mockedB);
        
        mockedB.XAlpha.AddPrerequisites(cellA.XBoundary.AlphaCoordinate);
        mockedB.YAlpha.AddPrerequisites(cellA.YBoundary.AlphaCoordinate);
        
        hive.Layout();
        mockedA.ResetAllInvocations();
        mockedB.ResetAllInvocations();
        mockedA.XAlpha.LengthToReturn++;
        mockedA.YAlpha.LengthToReturn++;
        mockedB.XAlpha.LengthToReturn++;
        mockedB.YAlpha.LengthToReturn++;
        
        mockedB.XAlpha.RemovePrerequisite(cellA.XBoundary.AlphaCoordinate);
        mockedB.YAlpha.RemovePrerequisite(cellA.YBoundary.AlphaCoordinate);
        mockedA.XAlpha.AddPrerequisites(cellB.XBoundary.AlphaCoordinate);
        mockedA.YAlpha.AddPrerequisites(cellB.YBoundary.AlphaCoordinate);

        hive.Layout();
        
        List<MockLength> results = mockedA.GetAllCalculateCalls()
            .Concat(mockedB.GetAllCalculateCalls())
            .OrderBy(a => a.Key)
            .Select(a => a.Value)
            .ToList();

        results.Should().ContainInOrder(mockedB.XAlpha, mockedA.XAlpha);
        results.Should().ContainInOrder(mockedB.YAlpha, mockedA.YAlpha);
    }
    
    [Test]
    public void LayoutShouldProcessInOrderOfComplexPrerequisites()
    {
        Hive hive = new(Mock.Of<IEntityManager>());
        
        Int32 callOrder = 0;
        Int32 getCallOrder() => ++callOrder;

        MockRecipe mockedA = new("mockedA", hive.Root, getCallOrder);
        Cell cellA = hive.NewCell(_ => mockedA);
        
        MockRecipe mockedB = new("mockedB", cellA, getCallOrder);
        Cell cellB = hive.NewCell(_ => mockedB);
        
        MockRecipe mockedC = new("mockedC", cellA, getCallOrder);
        Cell cellC = hive.NewCell(_ => mockedC);
        
        MockRecipe mockedD = new("mockedD", cellC, getCallOrder);
        Cell cellD = hive.NewCell(_ => mockedD);
        
        mockedA.YBeta.AddPrerequisites(cellB.YBoundary.BetaCoordinate, cellC.YBoundary.BetaCoordinate);
        mockedC.YBeta.AddPrerequisites(cellD.YBoundary.BetaCoordinate);
        
        mockedB.XBeta.AddPrerequisites(cellC.XBoundary.AlphaCoordinate, cellC.XBoundary.BetaCoordinate);
        
        hive.Layout();
        
        List<MockLength> results = mockedA.GetAllCalculateCalls()
            .Concat(mockedB.GetAllCalculateCalls())
            .Concat(mockedC.GetAllCalculateCalls())
            .Concat(mockedD.GetAllCalculateCalls())
            .OrderBy(a => a.Key)
            .Select(a => a.Value)
            .ToList();

        results.Should().ContainInOrder(mockedD.YBeta, mockedC.YBeta, mockedA.YBeta);
        results.Should().ContainInOrder(mockedB.YBeta, mockedA.YBeta);
        results.Should().ContainInOrder(mockedC.XAlpha, mockedB.XBeta);
        results.Should().ContainInOrder(mockedC.XBeta, mockedB.XBeta);
    }
    
    
    [Test]
    public void LayoutShouldDepthStopWhenRelativeValueHasNotChanged()
    {
        Hive hive = new(Mock.Of<IEntityManager>());
        
        Int32 callOrder = 0;
        Int32 getCallOrder() => ++callOrder;

        MockRecipe mockedA = new("mockedA", hive.Root, getCallOrder);
        Cell cellA = hive.NewCell(_ => mockedA);
        
        MockRecipe mockedB = new("mockedB", cellA, getCallOrder);
        Cell cellB = hive.NewCell(_ => mockedB);
        
        MockRecipe mockedC = new("mockedC", cellA, getCallOrder);
        Cell cellC = hive.NewCell(_ => mockedC);
        
        MockRecipe mockedD = new("mockedD", cellA, getCallOrder);
        hive.NewCell(_ => mockedD);
        
        MockRecipe mockedE = new("mockedE", cellA, getCallOrder);
        hive.NewCell(_ => mockedE);
        
        mockedB.YAlpha.AddPrerequisites(cellA.YBoundary.AlphaCoordinate);
        mockedC.YAlpha.AddPrerequisites(cellA.YBoundary.AlphaCoordinate);
        mockedD.YAlpha.AddPrerequisites(cellC.YBoundary.AlphaCoordinate,
            cellB.YBoundary.AlphaCoordinate);
        mockedE.YAlpha.AddPrerequisites(cellC.YBoundary.AlphaCoordinate);
        
        hive.Layout();
        
        mockedA.ResetAllInvocations();
        mockedB.ResetAllInvocations();
        mockedC.ResetAllInvocations();
        mockedD.ResetAllInvocations();
        mockedE.ResetAllInvocations();

        mockedA.YAlpha.LengthToReturn = 2.0f;
        mockedA.YAlpha.RaiseInvalidated();
        mockedB.YAlpha.LengthToReturn = 2.0f;
        mockedD.YAlpha.LengthToReturn = 2.0f;
        
        hive.Layout();
        
        List<MockLength> results = mockedA.GetAllCalculateCalls()
            .Concat(mockedB.GetAllCalculateCalls())
            .Concat(mockedC.GetAllCalculateCalls())
            .Concat(mockedD.GetAllCalculateCalls())
            .Concat(mockedE.GetAllCalculateCalls())
            .OrderBy(a => a.Key)
            .Select(a => a.Value)
            .ToList();
        
        // results.Should().NotContain(mockedE.YAlpha);
        results.Should().BeEquivalentTo(new []
            {
                mockedA.YAlpha, mockedB.YAlpha,
                mockedC.YAlpha, mockedD .YAlpha,
            });
    }

    
    [Test]
    public void LayoutShouldNotIncludeRemovedChildren()
    {
        Mock<IEntityManager> entityManager = new();
        Hive hive = new(entityManager.Object);

        MockRecipe recipe = new(hive.Root);
        Cell cell = hive.NewCell(_ => recipe);

        hive.Root.RemoveChild(cell);

        hive.Layout();
        recipe.GetAllCalculateCalls().Should().BeEmpty();
    }
    

    // Support
    //----------------------------------------------------------------------------------------------


    [DebuggerDisplay("{DebugName}")]
    public class MockLength : Length
    {
        private readonly Func<Int32> getCallOrder;
        private Int32 calculateCallCount;
        private readonly List<Int32> calculateCallOrders = new();


        public MockLength() : this(String.Empty, () => -1)
        {
            
        }
        
        public MockLength(String debugName, Func<Int32> getCallOrder)
        {
            DebugName = debugName;
            this.getCallOrder = getCallOrder;
        }


        public String DebugName { get; }
        public Single LengthToReturn { get; set; } = 1.0f;
        
        public IEnumerable<Int32> CalculateCallOrders => this.calculateCallOrders;

        
        
        public void AddPrerequisites(params Coordinate[] prereqs)
        {
            ModifyPrerequisites(prereqs, Enumerable.Empty<Coordinate>());;
        }
        
        public void RemovePrerequisite(params Coordinate[] prereqs)
        {
            ModifyPrerequisites(Enumerable.Empty<Coordinate>(), prereqs);
        }
        

        
        public override Single Calculate()
        {
            this.calculateCallCount++;
            this.calculateCallOrders.Add(this.getCallOrder());
            
            return this.LengthToReturn;
        }


        public void VerifyLayoutPass()
        {
            this.calculateCallCount.Should().Be(1);
            this.calculateCallCount = 0;
            this.calculateCallOrders.Clear();
        }
        
        public void VerifyNoLayoutPass()
        {
            this.calculateCallCount.Should().Be(0);
        }
        
        
        public void RaiseInvalidated()
        {
            base.RaiseValueInvalidated();
        }

        public void ClearInvocations()
        {
            this.calculateCallCount = 0;
            this.calculateCallOrders.Clear();
        }
    }
    
    [DebuggerDisplay("{DebugName}")]
    public class MockRecipe : IReadyToBuild
    {
        public String DebugName { get; }


        // public MockRecipe()
        //     : this(String.Empty, Option.None<Cell>(), () => -1)
        // {
        //
        // }
        //
        //
        // public MockRecipe(String debugName)
        //     : this(debugName, Option.None<Cell>(), () => -1)
        // {
        //
        // }
        //
        public MockRecipe(Cell parent) : this(String.Empty, parent, () => -1)
        {
        
        }
        
        public MockRecipe(String debugName, Cell parent)
            : this(debugName, parent, () => -1)
        {
        
        }


        public MockRecipe(Cell parent, Func<Int32> getCallOrder)
            : this(String.Empty, parent, getCallOrder)
        {
        }


        public MockRecipe(String debugName, Cell parent, Func<Int32> getCallOrder)
        {
            DebugName = debugName;

            this.XAlpha = new($"{debugName}.X.Alpha.", getCallOrder);
            this.XBeta = new($"{debugName}.X.Beta.", getCallOrder);
            this.YAlpha = new($"{debugName}.Y.Alpha", getCallOrder);
            this.YBeta = new($"{debugName}.Y.Beta.", getCallOrder);

            this.BoundariesRecipe = new();
            this.BoundariesRecipe.XBoundary.Alpha.Set(this.XAlpha);
            this.BoundariesRecipe.XBoundary.Beta.Set(this.XBeta);
            this.BoundariesRecipe.YBoundary.Alpha.Set(this.YAlpha);
            this.BoundariesRecipe.YBoundary.Beta.Set(this.YBeta);

            this.CompositionRecipe = new();
            this.CompositionRecipe.Parent.Set(parent);
            this.CompositionRecipe.InsertionIndex.Set(Option.None<Int32>());
            this.CompositionRecipe.Components.Set(Enumerable.Empty<Object>());
        }
        

        public MockLength XAlpha { get; }
        public MockLength XBeta { get; }
        public MockLength YAlpha { get; }
        public MockLength YBeta { get; }
        
        

        private BoundariesRecipe BoundariesRecipe { get; }
        
        private CompositionRecipe CompositionRecipe { get; }

        public void ResetAllInvocations()
        {
            this.XAlpha.ClearInvocations();
            this.XBeta.ClearInvocations();
            this.YAlpha.ClearInvocations();
            this.YBeta.ClearInvocations();
        }
        
        public CellBuilderState GetCellRecipe()
        {
            return new(this.BoundariesRecipe, this.CompositionRecipe);
        }


        public IEnumerable<KeyValuePair<Int32, MockLength>> GetAllCalculateCalls()
        {
            return GetCalculateCalls(this.XAlpha)
                .Concat(GetCalculateCalls(this.XBeta))
                .Concat(GetCalculateCalls(this.YAlpha))
                .Concat(GetCalculateCalls(this.YBeta));
        }


        private IEnumerable<KeyValuePair<Int32, MockLength>> GetCalculateCalls(MockLength length)
        {
            return length.CalculateCallOrders.Select(length.KeyedOn);
        }


        public override String ToString()
        {
            return this.DebugName;
        }
    }
    
    
    
    private enum Result
    {
        Relative,
        Absolute
    }

    private void AssertBoundary(Cell cell, Result resultToCheck,
        Single xAlpha, Single xBeta, Single yAlpha, Single yBeta)
    {
        switch (resultToCheck)
        {
            case Result.Relative:
            {
                cell.XBoundary.AlphaCoordinate.RelativeValue.Should().Be(xAlpha.Some());
                cell.XBoundary.BetaCoordinate.RelativeValue.Should().Be(xBeta.Some());
                cell.YBoundary.AlphaCoordinate.RelativeValue.Should().Be(yAlpha.Some());
                cell.YBoundary.BetaCoordinate.RelativeValue.Should().Be(yBeta.Some());
                break;
            }
            case Result.Absolute:
            {
                cell.XBoundary.AlphaCoordinate.AbsoluteValue.Should().Be(xAlpha.Some());
                cell.XBoundary.BetaCoordinate.AbsoluteValue.Should().Be(xBeta.Some());
                cell.YBoundary.AlphaCoordinate.AbsoluteValue.Should().Be(yAlpha.Some());
                cell.YBoundary.BetaCoordinate.AbsoluteValue.Should().Be(yBeta.Some());
                break;
            }
        }
    }
}
