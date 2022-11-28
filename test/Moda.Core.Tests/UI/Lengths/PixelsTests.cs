// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using FluentAssertions;
using FluentAssertions.Events;
using Moda.Core.UI;
using Moda.Core.UI.Lengths;
using NUnit.Framework;

namespace Moda.Core.Tests.UI.Lengths;

public class PixelsTests
{
    [Test]
    public void ConstructorShouldSetValue()
    {
        Pixels pixels = new(13);
        pixels.Value.Should().Be(13);
    }
    
    [Test]
    public void ValueSetterShouldSetNewValue()
    {
        // ReSharper disable once UseObjectOrCollectionInitializer
        Pixels pixels = new(13);
        pixels.Value = 27;
        pixels.Value.Should().Be(27);
    }


    [Test]
    public void ValueSetterShouldFireValueInvalidatedWhenChanged()
    {
        // ReSharper disable once UseObjectOrCollectionInitializer
        Pixels pixels = new(13);
        using IMonitor<Pixels>? monitor = pixels.Monitor();
        pixels.Value = 27;
        monitor.Should().Raise(nameof(Coordinate.ValueInvalidated))
            .WithSender(pixels);
    }
    
    [Test]
    public void ValueSetterShouldNotFireValueInvalidatedWhenNotChanged()
    {
        // ReSharper disable once UseObjectOrCollectionInitializer
        Pixels pixels = new(13);
        using IMonitor<Pixels>? monitor = pixels.Monitor();
        pixels.Value = 13;
        monitor.Should().NotRaise(nameof(Coordinate.ValueInvalidated));
    }


    [Test]
    public void CalculateShouldReturnValue()
    {
        Pixels pixels = new(7);
        pixels.Calculate().Should().BeApproximately(7.0f, 0.001f);
    }
}
