// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using FluentAssertions;
using FluentAssertions.Events;
using Moda.Core.Utility.Data;
using Moq;
using NUnit.Framework;
using Optional;

namespace Moda.Core.UI;

public class CoordinateTests
{
    // RelativeValue Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void CalculateShouldUpdateRelativeValue()
    {
        Mock<ICalculable> recipe = new();
        recipe.Setup(a => a.Calculate()).Returns(4.3f);

        Coordinate coordinate = new()
            {
                Recipe = recipe.Object,
            };
        coordinate.Calculate();

        coordinate.RelativeValue.Should().Be(4.3f.Some());
    }
    
    [Test]
    public void ChangeToRelativeValueShouldFireEvent()
    {
        Mock<ICalculable> recipe = new();
        Single lengthValue = 4;
        // ReSharper disable once AccessToModifiedClosure
        recipe.Setup(a => a.Calculate()).Returns(() => lengthValue);
        Coordinate coordinate = new()
            {
                Recipe = recipe.Object,
            };
        coordinate.Calculate();
        lengthValue = 7;
        
        using IMonitor<Coordinate>? monitor = coordinate.Monitor();
        coordinate.Calculate();
        monitor.Should().Raise(nameof(Coordinate.RelativeValueChanged))
            .WithSender(coordinate)
            .WithArgs<ValueChangedArgs<Option<Single>>>(a => 
                a.OldValue == 4f.Some() && a.NewValue == 7f.Some());
    }


    // AbsoluteValue Tests
    //----------------------------------------------------------------------------------------------

    [Test]
    public void AbsoluteValueShouldReturnNoneIfRelativeIsNone()
    {
        Coordinate coordinate = new()
            {
                Tare = 2.3f.Some(),
            };
        coordinate.AbsoluteValue.Should().Be(Option.None<Single>());
    }

    [Test]
    public void AbsoluteValueShouldReturnNoneIfTareIsNone()
    {
        Mock<ICalculable> recipe = new();
        recipe.Setup(a => a.Calculate()).Returns(6.1f);

        Coordinate coordinate = new()
            {
                Recipe = recipe.Object,
            };
        coordinate.Calculate();
        coordinate.AbsoluteValue.Should().Be(Option.None<Single>());
    }
    
    
    [Test]
    public void CalculateShouldUpdateAbsoluteWithRespectToTare()
    {
        Mock<ICalculable> recipe = new();
        recipe.Setup(a => a.Calculate()).Returns(6.1f);
        
        Coordinate coordinate = new()
            {
                Recipe = recipe.Object,
                Tare = 2.5f.Some(),
            };
        coordinate.Calculate();

        coordinate.AbsoluteValue.Should().Be(8.6f.Some());
    }
    
    
    [Test]
    public void ChangeToAbsoluteValueShouldFireEvent()
    {
        Mock<ICalculable> recipe = new();
        Single lengthValue = 4;
        // ReSharper disable once AccessToModifiedClosure
        recipe.Setup(a => a.Calculate()).Returns(() => lengthValue);
        Coordinate coordinate = new()
            {
                Recipe = recipe.Object,
            };
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


    // Tare Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void SettingTareShouldUpdateAbsoluteValue()
    {
        Mock<ICalculable> recipe = new();
        recipe.Setup(a => a.Calculate()).Returns(6.1f);
        
        Coordinate coordinate = new()
            {
                Recipe = recipe.Object,
                Tare = 2.5f.Some(),
            };
        coordinate.Calculate();

        coordinate.Tare = 3.1f.Some();

        coordinate.AbsoluteValue.Should().Be(9.2f.Some());
    }
    
    
    [Test]
    public void ChangeToTareShouldFireEvent()
    {
        Coordinate coordinate = new();
        
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
    public void ValueInvalidatedFromRecipeShouldForward()
    {
        Mock<ICalculable> recipe = new();

        Coordinate coordinate = new()
            {
                Recipe = recipe.Object
            };
        using IMonitor<Coordinate>? monitor = coordinate.Monitor();
        
        recipe.Raise(a => a.ValueInvalidated += null, EventArgs.Empty);
        
        monitor.Should().Raise(nameof(Coordinate.ValueInvalidated))
            .WithSender(coordinate);
    }
    
    [Test]
    public void PrerequisitesChangedFromRecipeShouldForward()
    {
        Mock<ICalculable> recipe = new();

        Coordinate coordinate = new()
            {
                Recipe = recipe.Object
            };
        using IMonitor<Coordinate>? monitor = coordinate.Monitor();

        CollectionChangedArgs<Coordinate> args = new(
            new []
            {
                new Coordinate()
            },
            new []
            {
                new Coordinate(), new Coordinate()
            });
    
        recipe.Raise(a => a.PrerequisitesChanged += null, recipe, args);

        monitor.Should().Raise(nameof(Coordinate.PrerequisitesChanged))
            .WithSender(coordinate)
            .WithArgs<CollectionChangedArgs<Coordinate>>(a => a == args);
    }
    
}
