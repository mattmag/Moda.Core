// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using FluentAssertions;
using Moda.Core.Entity;
using Moda.Core.Lengths;
using Moda.Core.UI.Builders;
using Moda.Core.Utility.Data;
using Moda.Core.Utility.Geometry;
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
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<ILength>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<ILength>())
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
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<ILength>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<ILength>())
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
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<ILength>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<ILength>())
            .AppendTo(hive.Root));
        
        Cell cell2 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<ILength>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<ILength>())
            .AppendTo(hive.Root));
        
        Cell cell3 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<ILength>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<ILength>())
            .AppendTo(cell2));
        
        Cell cell4 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<ILength>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<ILength>())
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
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<ILength>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<ILength>())
            .AppendTo(hive.Root));
        cell1.DebugName = "cell1";
        
        Cell cell2 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<ILength>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<ILength>())
            .AppendTo(cell1));
        cell2.DebugName = "cell2";
        
        Cell cell3 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<ILength>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<ILength>())
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
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<ILength>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<ILength>())
            .AppendTo(hive.Root));
        cell1.DebugName = "cell1";
        
        Cell cell2 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<ILength>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<ILength>())
            .AppendTo(cell1));
        cell2.DebugName = "cell2";
        
        Cell cell3 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<ILength>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<ILength>())
            .AppendTo(cell1));
        cell3.DebugName = "cell3";
        
        Cell cell4 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<ILength>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<ILength>())
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
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<ILength>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<ILength>())
            .AppendTo(hive.Root));
        cell1.DebugName = "cell1";
        
        Cell cell2 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<ILength>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<ILength>())
            .AppendTo(hive.Root));
        cell2.DebugName = "cell2";
        
        Cell cell3 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<ILength>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<ILength>())
            .AppendTo(hive.Root));
        cell3.DebugName = "cell3";
        
        Cell cell4 = hive.NewCell(a => a
            .AnchorAt(Horizontal.Left).WithWidth(Mock.Of<ILength>())
            .AnchorAt(Vertical.Top).WithHeight(Mock.Of<ILength>())
            .InsertAt(hive.Root, 2));
        cell4.DebugName = "cell4";

        hive.Root.Children.Should().ContainInOrder(cell1, cell2, cell4, cell3);
    }

    

    [Test]
    public void NewCellShouldSetCoordinateCalculationsToSameInstanceAsRecipe()
    {
        Mock<IEntityManager> entityManager = new();
        Hive hive = new(entityManager.Object);

        MockRecipe recipe = new();

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

        MockRecipe recipe = new();

        Cell cell = hive.NewCell(_ => recipe);
        cell.EntityID.Should().Be(expected);
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
        
        MockRecipe mocked = new(hive.Root.Some());
        Cell cell = hive.NewCell(a => mocked);
        
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
        
        MockRecipe mocked = new(hive.Root.Some());
        Cell cell = hive.NewCell(a => mocked);
        
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
        
        MockRecipe mocked = new(hive.Root.Some());
        Cell cell = hive.NewCell(a => mocked);
        
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
    public void LayoutShouldProcessInOrderOfPrerequisites()
    {
        Hive hive = new(Mock.Of<IEntityManager>());
        
        Int32 callOrder = 0;
        Int32 getCallOrder() => ++callOrder;

        MockRecipe mockedA = new(hive.Root.Some(), getCallOrder);
        Cell cellA = hive.NewCell(a => mockedA);
        
        MockRecipe mockedB = new(cellA.Some(), getCallOrder);
        
        mockedB.XAlpha.AddPrerequisites( cellA.XBoundary.AlphaCoordinate);
        mockedB.YAlpha.AddPrerequisites(cellA.YBoundary.AlphaCoordinate);
        mockedB.XBeta.AddPrerequisites(cellA.XBoundary.AlphaCoordinate,
            cellA.XBoundary.BetaCoordinate);
        mockedB.YBeta.AddPrerequisites(cellA.YBoundary.BetaCoordinate);

        Cell cellB = hive.NewCell(a => mockedB);
        
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

        MockRecipe mockedA = new(hive.Root.Some(), getCallOrder);
        Cell cellA = hive.NewCell(a => mockedA);
        
        MockRecipe mockedB = new(cellA.Some(), getCallOrder);
        Cell cellB = hive.NewCell(a => mockedB);
        
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
    public void LayoutShouldProcessInOrderOfComplexPrerequisites()
    {
        Hive hive = new(Mock.Of<IEntityManager>());
        
        Int32 callOrder = 0;
        Int32 getCallOrder() => ++callOrder;

        MockRecipe mockedA = new("mockedA", hive.Root.Some(), getCallOrder);
        Cell cellA = hive.NewCell(a => mockedA);
        
        MockRecipe mockedB = new("mockedB", cellA.Some(), getCallOrder);
        Cell cellB = hive.NewCell(a => mockedB);
        
        MockRecipe mockedC = new("mockedC", cellA.Some(), getCallOrder);
        Cell cellC = hive.NewCell(a => mockedC);
        
        MockRecipe mockedD = new("mockedD", cellC.Some(), getCallOrder);
        Cell cellD = hive.NewCell(a => mockedD);
        
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

        MockRecipe mockedA = new("mockedA", hive.Root.Some(), getCallOrder);
        Cell cellA = hive.NewCell(a => mockedA);
        
        MockRecipe mockedB = new("mockedB", cellA.Some(), getCallOrder);
        Cell cellB = hive.NewCell(a => mockedB);
        
        MockRecipe mockedC = new("mockedC", cellA.Some(), getCallOrder);
        Cell cellC = hive.NewCell(a => mockedC);
        
        MockRecipe mockedD = new("mockedD", cellA.Some(), getCallOrder);
        Cell cellD = hive.NewCell(a => mockedD);
        
        MockRecipe mockedE = new("mockedE", cellA.Some(), getCallOrder);
        Cell cellE = hive.NewCell(a => mockedE);
        
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

    

    // Support
    //----------------------------------------------------------------------------------------------


    [DebuggerDisplay("{DebugName}")]
    public class MockLength : ILength
    {
        private readonly Func<Int32> getCallOrder;
        private Int32 calculateCallCount = 0;
        private List<Int32> calculateCallOrders = new();
        private List<Coordinate> prerequisites = new();


        public MockLength() : this(string.Empty, () => -1)
        {
            
        }
        
        public MockLength(string debugName, Func<Int32> getCallOrder)
        {
            DebugName = debugName;
            this.getCallOrder = getCallOrder;
        }


        public String DebugName { get; }
        public Single LengthToReturn { get; set; } = 1.0f;
        
        public IEnumerable<Int32> CalculateCallOrders => this.calculateCallOrders;


        public IEnumerable<Coordinate> Prerequisites => this.prerequisites;

        
        public void AddPrerequisites(params Coordinate[] prereqs)
        {
            this.prerequisites.AddRange(prereqs);
            this.PrerequisitesChanged?.Invoke(this, new(prereqs, Enumerable.Empty<Coordinate>()));
        }
        
        public void RemovePrerequisite(params Coordinate[] prereqs)
        {
            foreach (Coordinate prereq in prereqs)
            {
                this.prerequisites.Remove(prereq);
            }
            this.PrerequisitesChanged?.Invoke(this, new(Enumerable.Empty<Coordinate>(),  prereqs ));
        }
        
        public event EventHandler? ValueInvalidated;
        public event EventHandler<CollectionChangedArgs<Coordinate>>? PrerequisitesChanged;
        
        
        public Single Calculate()
        {
            calculateCallCount++;
            this.calculateCallOrders.Add(this.getCallOrder());
            
            return this.LengthToReturn;
        }


        public void VerifyLayoutPass()
        {
            this.calculateCallCount.Should().Be(1);
            this.calculateCallCount = 0;
            calculateCallOrders.Clear();
        }
        
        public void VerifyNoLayoutPass()
        {
            this.calculateCallCount.Should().Be(0);
        }
        
        
        public void RaiseInvalidated()
        {
            this.ValueInvalidated?.Invoke(this, EventArgs.Empty);
        }

        public void ClearInvocations()
        {
            this.calculateCallCount = 0;
            calculateCallOrders.Clear();
        }
    }
    
    [DebuggerDisplay("{DebugName}")]
    public class MockRecipe : IReadyForConstruction
    {
        public String DebugName { get; }
        private readonly Func<Int32> getCallOrder;

        public MockRecipe()
            : this(string.Empty, Option.None<Cell>(), () => -1)
        {
        
        }
        
        
        public MockRecipe(string debugName)
            : this(debugName, Option.None<Cell>(), () => -1)
        {
        
        }
        
        public MockRecipe(Option<Cell> parent) : this(string.Empty, parent, () => -1)
        {
        
        }
        
        public MockRecipe(string debugName, Option<Cell> parent)
            : this(debugName, parent, () => -1)
        {
        
        }


        public MockRecipe(Option<Cell> parent, Func<Int32> getCallOrder)
            : this(string.Empty, parent, getCallOrder)
        {
        }


        public MockRecipe(string debugName, Option<Cell> parent, Func<Int32> getCallOrder)
        {
            DebugName = debugName;
            this.getCallOrder = getCallOrder;
            
            this.XAlpha = new($"{debugName}.X.Alpha.", getCallOrder);
            this.XBeta = new($"{debugName}.X.Beta.", getCallOrder);
            this.YAlpha = new($"{debugName}.Y.Alpha", getCallOrder);
            this.YBeta = new($"{debugName}.Y.Beta.", getCallOrder);
            
            this.BoundariesRecipe = new();
            this.BoundariesRecipe.XBoundary.Alpha = this.XAlpha.Some<ILength>();
            this.BoundariesRecipe.XBoundary.Beta = this.XBeta.Some<ILength>();
            this.BoundariesRecipe.YBoundary.Alpha = this.YAlpha.Some<ILength>();
            this.BoundariesRecipe.YBoundary.Beta = this.YBeta.Some<ILength>();

            this.CompositionRecipe = new();
            this.CompositionRecipe.Parent = parent;
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
        
        public CellRecipe GetRecipe()
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


        public IEnumerable<KeyValuePair<Int32, MockLength>> GetCalculateCalls(MockLength length)
        {
            foreach (Int32 callOrder in length.CalculateCallOrders)
            {
                yield return length.KeyedOn(callOrder);
            }
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
