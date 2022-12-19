// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Events;
using Moda.Core.Tests.Support;
using Moda.Core.UI;
using Moda.Core.UI.Lengths;
using Moda.Core.Utility.Data;
using Moq;
using NUnit.Framework;
using Optional;
using Optional.Unsafe;

namespace Moda.Core.Tests.UI;

public class LengthTests
{
    [Test]
    public void CellShouldBeUninitialized()
    {
        MockLength length = new();
        length.AssertOwner(Option.None<Cell>());
        length.AssertAxis(Option.None<Axis>());
    }
    
    [Test]
    public void InitializeShouldSetCell()
    {
        MockLength length = new();
        Cell cell = new(Mock.Of<IHoneyComb>(), Mock.Of<ICalculation>(), 
            Mock.Of<ICalculation>(),  Mock.Of<ICalculation>(),  Mock.Of<ICalculation>());
        length.Initialize(cell, Axis.X);
        length.AssertOwner(cell.Some());
    }
    
    [TestCase(Axis.X)]
    [TestCase(Axis.Y)]
    public void InitializeShouldSetCell(Axis axis)
    {
        MockLength length = new();
        Cell cell = new(Mock.Of<IHoneyComb>(), Mock.Of<ICalculation>(), 
            Mock.Of<ICalculation>(),  Mock.Of<ICalculation>(),  Mock.Of<ICalculation>());
        length.Initialize(cell, axis);
        length.AssertAxis(axis.Some());
    }


    [Test]
    public void ModifyPrerequisitesShouldAddNewPrereqs()
    {
        MockLength length = new();
        Coordinate coordinateA = GetMockedCoordinate();
        Coordinate coordinateB = GetMockedCoordinate();
        
        length.ModifyPrerequisites(
            new[] { coordinateA, coordinateB },
            Enumerable.Empty<Coordinate>());
        
        length.Prerequisites.Should().BeEquivalentTo(new[] { coordinateA, coordinateB });
    }
    
    [Test]
    public void ModifyPrerequisitesShouldOnlyAddNewPrereqs()
    {
        MockLength length = new();
        Coordinate coordinateA = GetMockedCoordinate();
        Coordinate coordinateB = GetMockedCoordinate();
        
        length.ModifyPrerequisites(
            new[] { coordinateA, coordinateB },
            Enumerable.Empty<Coordinate>());
        
        length.ModifyPrerequisites(
            new[] { coordinateA },
            Enumerable.Empty<Coordinate>());
        
        length.Prerequisites.Should().BeEquivalentTo(new[] { coordinateA, coordinateB });
    }

    [Test]
    public void ModifyPrerequisitesShouldRemoveSpecifiedPrereqs()
    {
        MockLength length = new();
        Coordinate coordinateA = GetMockedCoordinate();
        Coordinate coordinateB = GetMockedCoordinate();
        Coordinate coordinateC = GetMockedCoordinate();
        
        length.ModifyPrerequisites(
            new[] { coordinateA, coordinateB, coordinateC },
            Enumerable.Empty<Coordinate>());
        
        length.ModifyPrerequisites(
            Enumerable.Empty<Coordinate>(),
            new[] { coordinateB, coordinateA });
        
        length.Prerequisites.Should().BeEquivalentTo(new[] { coordinateC });
    }
    
    
    [Test]
    public void ModifyPrerequisitesRaisePrerequisitesChangedWithActualChanges()
    {
        MockLength length = new();
        Coordinate coordinateA = GetMockedCoordinate();
        Coordinate coordinateB = GetMockedCoordinate();
        Coordinate coordinateC = GetMockedCoordinate();
        Coordinate coordinateD = GetMockedCoordinate();
        Coordinate coordinateE = GetMockedCoordinate();
        
        length.ModifyPrerequisites(
            new[] { coordinateA, coordinateB, coordinateC },
            Enumerable.Empty<Coordinate>());

        IMonitor<MockLength> monitor = length.Monitor();
        
        length.ModifyPrerequisites(
            new[] { coordinateA, coordinateD },
            new[] { coordinateC, coordinateE });
        
        monitor.Should().Raise(nameof(Length.PrerequisitesChanged))
            .WithSender(length)
            .WithAssertedArgs<CollectionChangedArgs<Coordinate>>(a =>
                {
                    a.ItemsAdded.Should().BeEquivalentTo(new[] { coordinateD });
                    a.ItemsRemoved.Should().BeEquivalentTo(new[] { coordinateC });
                });
    }


    // Support
    //----------------------------------------------------------------------------------------------
    

    public class MockLength : Length
    {
        public override Single Calculate()
        {
            throw new NotImplementedException();
        }


        public void AssertOwner(Option<Cell> cell)
        {
            _owner.Should().Be(cell);
            cell.Match(
                a => Owner.Should().BeSameAs(a),
                () => this.Invoking(a => a.Owner).Should().Throw<OptionValueMissingException>());
        }
        
        public void AssertAxis(Option<Axis> axis)
        {
            _axis.Should().Be(axis);
            axis.Match(
                a => Axis.Should().Be(a),
                () => this.Invoking(a => a.Axis).Should().Throw<OptionValueMissingException>());
        }


        public new void ModifyPrerequisites(IEnumerable<Coordinate> add,
            IEnumerable<Coordinate> remove)
        {
            base.ModifyPrerequisites(add, remove);
        }
    }
    
    private static Coordinate GetMockedCoordinate() =>
        new(GetMockedCell(), Axis.X, Mock.Of<ICalculation>());
    
    private static Cell GetMockedCell() => new(Mock.Of<IHoneyComb>(), Mock.Of<ICalculation>(),
        Mock.Of<ICalculation>(), Mock.Of<ICalculation>(), Mock.Of<ICalculation>());
}
