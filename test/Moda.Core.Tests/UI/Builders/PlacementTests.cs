// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using FluentAssertions;
using Moda.Core.UI;
using Moda.Core.UI.Builders;
using Moq;
using NUnit.Framework;

namespace Moda.Core.Tests.UI.Builders;

public class PlacementTests
{
    [TestCase(NCoordinate.Alpha, 5, 10, 5, 15)]
    [TestCase(NCoordinate.Beta, 25, 10, 15, 25)]
    public void CalculateShouldProduceBothCoordinates(NCoordinate coordinate,
        Single location, Single length, Single expectedAlpha, Single expectedBeta)
    {
        Mock<ILength> locationAsLength = new();
        locationAsLength.Setup(a => a.Calculate()).Returns(location);

        Mock<ILength> lengthAsLength = new();
        lengthAsLength.Setup(a => a.Calculate()).Returns(length);
        
        Placement<ILength> placement = new(coordinate, _ => locationAsLength.Object);
        placement.Calculate(lengthAsLength.Object, out ILength alpha, out ILength beta);
        
        alpha.Calculate().Should().Be(expectedAlpha);
        beta.Calculate().Should().Be(expectedBeta);
    }


    [Test]
    public void ToBaseShouldCarryOverValues()
    {
        Mock<ILength> lengthAsLength = new();
        lengthAsLength.Setup(a => a.Calculate()).Returns(14);
        CalculatePlacement<ILength> calc = new(_ => lengthAsLength.Object);

        Placement<ILength> original = new(NCoordinate.Beta, calc);
        Placement<ILength> result = original.ToBase();
        
        result.Solves.Should().Be(NCoordinate.Beta);
        result.Calculation.Invoke(Mock.Of<ILength>()).Should().BeSameAs(lengthAsLength.Object);
    }
}
