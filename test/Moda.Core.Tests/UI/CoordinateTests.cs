// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using FluentAssertions;
using FluentAssertions.Events;
using Moda.Core.UI;
using Moda.Core.Utility.Data;
using Moq;
using NUnit.Framework;
using Optional;

namespace Moda.Core.Tests.UI;

public class CoordinateTests
{
    // Constructor Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    [TestCase(Axis.X)]
    [TestCase(Axis.Y)]
    public void ConstructorShouldInitializeCalculation(Axis axis)
    {
        Mock<ICalculation> calculable = new();
        Cell owner = GetMockedCell();
        Coordinate coordinate = new(owner, axis, calculable.Object);
        calculable.Verify(a => a.Initialize(It.Is<Cell>(b => b == owner),
            It.Is<Axis>(b => b == axis)));
    }
    
    // RelativeValue Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void CalculateShouldUpdateRelativeValue()
    {
        Mock<ICalculation> calculable = new();
        calculable.Setup(a => a.Calculate()).Returns(4.3f);

        Coordinate coordinate = new(GetMockedCell(), Axis.X, calculable.Object);
        coordinate.Calculate();

        coordinate.RelativeValue.Should().Be(4.3f.Some());
    }
    
    [Test]
    public void ChangeToRelativeValueShouldFireEvent()
    {
        Mock<ICalculation> calculable = new();
        Single lengthValue = 4;
        // ReSharper disable once AccessToModifiedClosure
        calculable.Setup(a => a.Calculate()).Returns(() => lengthValue);
        Coordinate coordinate = new(GetMockedCell(), Axis.X, calculable.Object);
        coordinate.Calculate();
        lengthValue = 7;
        
        using IMonitor<Coordinate>? monitor = coordinate.Monitor();
        coordinate.Calculate();
        monitor.Should().Raise(nameof(Coordinate.RelativeValueChanged))
            .WithSender(coordinate)
            .WithArgs<ValueChangedArgs<Option<Single>>>(a => 
                a.OldValue == 4f.Some() && a.NewValue == 7f.Some());
    }
    
    
    [Test]
    public void NoChangeToRelativeValueShouldNotFireEvent()
    {
        Mock<ICalculation> calculable = new();
        Single lengthValue = 4;
        // ReSharper disable once AccessToModifiedClosure
        calculable.Setup(a => a.Calculate()).Returns(() => lengthValue);
        Coordinate coordinate = new(GetMockedCell(), Axis.X, calculable.Object);
        coordinate.Calculate();

        using IMonitor<Coordinate>? monitor = coordinate.Monitor();
        coordinate.Calculate();
        monitor.Should().NotRaise(nameof(Coordinate.RelativeValueChanged));
    }


    // AbsoluteValue Tests
    //----------------------------------------------------------------------------------------------

    [Test]
    public void AbsoluteValueShouldReturnNoneIfRelativeIsNone()
    {
        Coordinate coordinate = new(GetMockedCell(), Axis.X, Mock.Of<ICalculation>())
            {
                Tare = 2.3f.Some(),
            };
        coordinate.AbsoluteValue.Should().Be(Option.None<Single>());
    }

    [Test]
    public void AbsoluteValueShouldReturnNoneIfTareIsNone()
    {
        Mock<ICalculation> calculable = new();
        calculable.Setup(a => a.Calculate()).Returns(6.1f);

        Coordinate coordinate = new(GetMockedCell(), Axis.X, calculable.Object);
        coordinate.Calculate();
        coordinate.AbsoluteValue.Should().Be(Option.None<Single>());
    }
    
    
    [Test]
    public void CalculateShouldUpdateAbsoluteWithRespectToTare()
    {
        Mock<ICalculation> calculable = new();
        calculable.Setup(a => a.Calculate()).Returns(6.1f);
        
        Coordinate coordinate = new(GetMockedCell(), Axis.X, calculable.Object)
            {
                Tare = 2.5f.Some(),
            };
        coordinate.Calculate();

        coordinate.AbsoluteValue.Should().Be(8.6f.Some());
    }
    
    
    [Test]
    public void ChangeToAbsoluteValueShouldFireEvent()
    {
        Mock<ICalculation> calculable = new();
        Single lengthValue = 4;
        // ReSharper disable once AccessToModifiedClosure
        calculable.Setup(a => a.Calculate()).Returns(() => lengthValue);
        Coordinate coordinate = new(GetMockedCell(), Axis.X, calculable.Object);
        coordinate.Tare = 2f.Some();
        coordinate.Calculate();
        lengthValue = 7;
        
        using IMonitor<Coordinate>? monitor = coordinate.Monitor();
        coordinate.Calculate();
        monitor.Should().Raise(nameof(Coordinate.AbsoluteValueChanged))
            .WithSender(coordinate)
            .WithArgs<ValueChangedArgs<Option<Single>>>(a => 
                a.OldValue == 6f.Some() && a.NewValue == 9f.Some());
    }
    
    [Test]
    public void NoChangeToAbsoluteValueShouldNotFireEvent()
    {
        Mock<ICalculation> calculable = new();
        Single lengthValue = 4;
        // ReSharper disable once AccessToModifiedClosure
        calculable.Setup(a => a.Calculate()).Returns(() => lengthValue);
        Coordinate coordinate = new(GetMockedCell(), Axis.X, calculable.Object);
        coordinate.Tare = 2f.Some();
        coordinate.Calculate();

        using IMonitor<Coordinate>? monitor = coordinate.Monitor();
        coordinate.Calculate();
        monitor.Should().NotRaise(nameof(Coordinate.AbsoluteValueChanged));
    }
    

    // Tare Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void SettingTareShouldUpdateAbsoluteValue()
    {
        Mock<ICalculation> calculable = new();
        calculable.Setup(a => a.Calculate()).Returns(6.1f);
        
        Coordinate coordinate = new(GetMockedCell(), Axis.X, calculable.Object)
            {
                Tare = 2.5f.Some(),
            };
        coordinate.Calculate();

        coordinate.Tare = 3.1f.Some();

        coordinate.AbsoluteValue.Should().Be(9.2f.Some());
    }
    
    
    [Test]
    public void ChangeToTareShouldFireEvent()
    {
        Coordinate coordinate = new(GetMockedCell(), Axis.X, Mock.Of<ICalculation>());
        
        using IMonitor<Coordinate>? monitor = coordinate.Monitor();
        coordinate.Tare = 8f.Some();
        monitor.Should().Raise(nameof(Coordinate.TareChanged))
            .WithSender(coordinate)
            .WithArgs<ValueChangedArgs<Option<Single>>>(a => 
                a.OldValue == Option.None<Single>() && a.NewValue == 8f.Some());
    }


    // Event Forwards
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void ValueInvalidatedFromCalculationShouldForward()
    {
        Mock<ICalculation> calculable = new();
        Coordinate coordinate = new(GetMockedCell(), Axis.X, calculable.Object);
        
        using IMonitor<Coordinate>? monitor = coordinate.Monitor();
        
        calculable.Raise(a => a.ValueInvalidated += null, calculable.Object);
        
        monitor.Should().Raise(nameof(Coordinate.ValueInvalidated))
            .WithSender(coordinate);
    }
    
    [Test]
    public void PrerequisitesChangedFromCalculationShouldForward()
    {
        Mock<ICalculation> calculable = new();
        Coordinate coordinate = new(GetMockedCell(), Axis.X, calculable.Object);
        
        using IMonitor<Coordinate>? monitor = coordinate.Monitor();

        CollectionChangedArgs<Coordinate> args = new(
            new []
            {
                new Coordinate(GetMockedCell(), Axis.X, Mock.Of<ICalculation>())
            },
            new []
            {
                new Coordinate(GetMockedCell(), Axis.X, Mock.Of<ICalculation>()),
                new Coordinate(GetMockedCell(), Axis.X, Mock.Of<ICalculation>())
            });
    
        calculable.Raise(a => a.PrerequisitesChanged += null, calculable.Object, args);

        monitor.Should().Raise(nameof(Coordinate.PrerequisitesChanged))
            .WithSender(coordinate)
            .WithArgs<CollectionChangedArgs<Coordinate>>(a => a == args);
    }


    // Calculation Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void SetCalculationShouldSetNewValue()
    {
        ICalculation calculable1 = Mock.Of<ICalculation>();
        ICalculation calculable2 = Mock.Of<ICalculation>();
        Coordinate coordinate = new(GetMockedCell(), Axis.X, calculable1);
        
        coordinate.Calculation = calculable2;

        coordinate.Calculation.Should().Be(calculable2);
    }
    
    [Test]
    public void SetCalculationShouldRaiseCalculationChanged()
    {
        ICalculation calculable1 = Mock.Of<ICalculation>();
        ICalculation calculable2 = Mock.Of<ICalculation>();
        Coordinate coordinate = new(GetMockedCell(), Axis.X, calculable1);
        
        
        using IMonitor<Coordinate>? monitor = coordinate.Monitor();
        
        coordinate.Calculation = calculable2;
        
        monitor.Should().Raise(nameof(Coordinate.CalculationChanged))
            .WithSender(coordinate)
            .WithArgs<ValueChangedArgs<ICalculation>>(a =>
                a.OldValue == calculable1 && a.NewValue == calculable2);
        

        coordinate.Calculation.Should().Be(calculable2);
    }
    
    [Test]
    public void ValueInvalidatedFromNewCalculationShouldForward()
    {
        ICalculation calculable1 = Mock.Of<ICalculation>();
        Mock<ICalculation> calculable2 = new();
        Coordinate coordinate = new(GetMockedCell(), Axis.X, calculable1);
        
        coordinate.Calculation = calculable2.Object;

        using IMonitor<Coordinate>? monitor = coordinate.Monitor();
        
        calculable2.Raise(a => a.ValueInvalidated += null, calculable2.Object);
        
        monitor.Should().Raise(nameof(Coordinate.ValueInvalidated))
            .WithSender(coordinate);
        

    }
    
    [Test]
    public void PrerequisitesChangedFromNewCalculationShouldForward()
    {
        ICalculation calculable1 = Mock.Of<ICalculation>();
        Mock<ICalculation> calculable2 = new();
        Coordinate coordinate = new(GetMockedCell(), Axis.X, calculable1);
        
        coordinate.Calculation = calculable2.Object;
        
        using IMonitor<Coordinate>? monitor = coordinate.Monitor();

        CollectionChangedArgs<Coordinate> args = new(
            new []
            {
                new Coordinate(GetMockedCell(), Axis.X, Mock.Of<ICalculation>())
            },
            new []
            {
                new Coordinate(GetMockedCell(), Axis.X, Mock.Of<ICalculation>()),
                new Coordinate(GetMockedCell(), Axis.X, Mock.Of<ICalculation>())
            });
    
        calculable2.Raise(a => a.PrerequisitesChanged += null, calculable2.Object, args);

        monitor.Should().Raise(nameof(Coordinate.PrerequisitesChanged))
            .WithSender(coordinate)
            .WithArgs<CollectionChangedArgs<Coordinate>>(a => a == args);
    }
    
    [Test]
    public void ValueInvalidatedFromOldCalculationShouldNotForward()
    {
        Mock<ICalculation> calculable1 = new();
        Mock<ICalculation> calculable2 = new();
        Coordinate coordinate = new(GetMockedCell(), Axis.X, calculable1.Object);
        
        coordinate.Calculation = calculable2.Object;

        using IMonitor<Coordinate>? monitor = coordinate.Monitor();
        
        calculable1.Raise(a => a.ValueInvalidated += null, EventArgs.Empty);

        monitor.Should().NotRaise(nameof(Coordinate.ValueInvalidated));


    }
    
    [Test]
    public void PrerequisitesChangedFromOldCalculationShouldNotForward()
    {
        Mock<ICalculation> calculable1 = new();
        Mock<ICalculation> calculable2 = new();
        Coordinate coordinate = new(GetMockedCell(), Axis.X, calculable1.Object);
        
        coordinate.Calculation = calculable2.Object;
        
        using IMonitor<Coordinate>? monitor = coordinate.Monitor();

        CollectionChangedArgs<Coordinate> args = new(
            new []
            {
                new Coordinate(GetMockedCell(), Axis.X, Mock.Of<ICalculation>())
            },
            new []
            {
                new Coordinate(GetMockedCell(), Axis.X, Mock.Of<ICalculation>()),
                new Coordinate(GetMockedCell(), Axis.X, Mock.Of<ICalculation>())
            });
    
        calculable1.Raise(a => a.PrerequisitesChanged += null, calculable1, args);

        monitor.Should().NotRaise(nameof(Coordinate.PrerequisitesChanged));
    }

    
    [Test]
    [TestCase(Axis.X)]
    [TestCase(Axis.Y)]
    public void SetCalculationShouldInitializeNewValue(Axis axis)
    {
        Cell owner = GetMockedCell();
        Coordinate coordinate = new(owner, axis, Mock.Of<ICalculation>());
        
        Mock<ICalculation> calculable = new();
        coordinate.Calculation = calculable.Object;
        calculable.Verify(a => a.Initialize(It.Is<Cell>(b => b == owner),
            It.Is<Axis>(b => b == axis)));
    }

    


    // Support
    //----------------------------------------------------------------------------------------------
    private static Cell GetMockedCell() => new(Mock.Of<IHoneyComb>(), Mock.Of<ICalculation>(),
        Mock.Of<ICalculation>(), Mock.Of<ICalculation>(), Mock.Of<ICalculation>());
    
}
