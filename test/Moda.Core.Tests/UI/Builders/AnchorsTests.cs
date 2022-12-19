// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using FluentAssertions;
using Moda.Core.UI.Builders;
using NUnit.Framework;

namespace Moda.Core.Tests.UI.Builders;

public class AnchorsTests
{
    [TestCase(HAnchor.Left, NAnchor.Alpha)]
    [TestCase(HAnchor.Center, NAnchor.Center)]
    [TestCase(HAnchor.Right, NAnchor.Beta)]
    public void HAnchorToNeutralShouldConvert(HAnchor input, NAnchor output)
    {
        input.ToNeutral().Should().Be(output);
    }

    [Test]
    public void HAnchorToNeutralShouldThrowExceptionForUnknownValue()
    {
        ((HAnchor)999).Invoking(a => a.ToNeutral()).Should()
            .Throw<ArgumentOutOfRangeException>();
    }
    
    [TestCase(VAnchor.Up, NAnchor.Alpha)]
    [TestCase(VAnchor.Middle, NAnchor.Center)]
    [TestCase(VAnchor.Down, NAnchor.Beta)]
    public void VAnchorToNeutralShouldConvert(HAnchor input, NAnchor output)
    {
        input.ToNeutral().Should().Be(output);
    }
    
    [Test]
    public void VAnchorToNeutralShouldThrowExceptionForUnknownValue()
    {
        ((VAnchor)999).Invoking(a => a.ToNeutral()).Should()
            .Throw<ArgumentOutOfRangeException>();
    }


    [TestCase(NAnchor.Alpha, HAnchor.Left)]
    [TestCase(NAnchor.Center, HAnchor.Center)]
    [TestCase(NAnchor.Beta, HAnchor.Right)]
    public void NAnchorToHorizontalShouldConvert(NAnchor input, HAnchor output)
    {
        input.ToHorizontal().Should().Be(output);
    }
    
    [Test]
    public void NAnchorToHorizontalShouldThrowExceptionForUnknownValue()
    {
        ((NAnchor)999).Invoking(a => a.ToHorizontal()).Should()
            .Throw<ArgumentOutOfRangeException>();
    }
    
    [TestCase(NAnchor.Alpha, VAnchor.Up)]
    [TestCase(NAnchor.Center, VAnchor.Middle)]
    [TestCase(NAnchor.Beta, VAnchor.Down)]
    public void NAnchorToVerticalShouldConvert(NAnchor input, VAnchor output)
    {
        input.ToVertical().Should().Be(output);
    }
    
    [Test]
    public void NAnchorToVerticalShouldThrowExceptionForUnknownValue()
    {
        ((NAnchor)999).Invoking(a => a.ToVertical()).Should()
            .Throw<ArgumentOutOfRangeException>();
    }
}
