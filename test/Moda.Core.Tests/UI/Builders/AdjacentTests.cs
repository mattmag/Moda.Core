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

public class AdjacentTests
{
    [TestCase(HAdjacent.LeftOf, NAdjacent.BetaToOtherAlpha)]
    [TestCase(HAdjacent.RightOf, NAdjacent.AlphaToOtherBeta)]
    public void HAdjacentToNeutralShouldConvert(HAdjacent input, NAdjacent output)
    {
        input.ToNeutral().Should().Be(output);
    }

    [Test]
    public void HAdjacentToNeutralShouldThrowExceptionForUnknownValue()
    {
        ((HAdjacent)999).Invoking(a => a.ToNeutral()).Should()
            .Throw<ArgumentOutOfRangeException>();
    }
    
    [TestCase(VAdjacent.Above, NAdjacent.BetaToOtherAlpha)]
    [TestCase(VAdjacent.Below, NAdjacent.AlphaToOtherBeta)]
    public void VAdjacentToNeutralShouldConvert(HAdjacent input, NAdjacent output)
    {
        input.ToNeutral().Should().Be(output);
    }
    
    [Test]
    public void VAdjacentToNeutralShouldThrowExceptionForUnknownValue()
    {
        ((VAdjacent)999).Invoking(a => a.ToNeutral()).Should()
            .Throw<ArgumentOutOfRangeException>();
    }


    [TestCase(NAdjacent.BetaToOtherAlpha, HAdjacent.LeftOf)]
    [TestCase(NAdjacent.AlphaToOtherBeta, HAdjacent.RightOf)]
    public void NAdjacentToHorizontalShouldConvert(NAdjacent input, HAdjacent output)
    {
        input.ToHorizontal().Should().Be(output);
    }
    
    [Test]
    public void NAdjacentToHorizontalShouldThrowExceptionForUnknownValue()
    {
        ((NAdjacent)999).Invoking(a => a.ToHorizontal()).Should()
            .Throw<ArgumentOutOfRangeException>();
    }
    
    [TestCase(NAdjacent.BetaToOtherAlpha, VAdjacent.Above)]
    [TestCase(NAdjacent.AlphaToOtherBeta, VAdjacent.Below)]
    public void NAdjacentToVerticalShouldConvert(NAdjacent input, VAdjacent output)
    {
        input.ToVertical().Should().Be(output);
    }
    
    [Test]
    public void NAdjacentToVerticalShouldThrowExceptionForUnknownValue()
    {
        ((NAdjacent)999).Invoking(a => a.ToVertical()).Should()
            .Throw<ArgumentOutOfRangeException>();
    }
}
