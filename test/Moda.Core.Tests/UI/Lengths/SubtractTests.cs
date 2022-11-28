// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using FluentAssertions;
using Moda.Core.UI;
using Moda.Core.UI.Lengths;
using Moq;
using NUnit.Framework;

namespace Moda.Core.Tests.UI.Lengths;

public class SubtractTests
{
    [Test]
    public void CalculateShouldReturnDifferenceOfLengthResults()
    {
        Mock<ILength> lengthA = new();
        lengthA.Setup(a => a.Calculate()).Returns(46.0f);
        
        Mock<ILength> lengthB = new();
        lengthB.Setup(a => a.Calculate()).Returns(16.0f);

        Subtract subtract = new(lengthA.Object, lengthB.Object);
        subtract.Calculate().Should().BeApproximately(30.0f, 0.001f);
    }
    
    [Test]
    public void CalculateShouldReflectNewLengthValues()
    {
        Single valueA = 46.0f;
        Mock<ILength> lengthA = new();
        lengthA.Setup(a => a.Calculate()).Returns(() => valueA);
        
        Single valueB = 16.0f;
        Mock<ILength> lengthB = new();
        lengthB.Setup(a => a.Calculate()).Returns(() => valueB);

        Subtract subtract = new(lengthA.Object, lengthB.Object);
        subtract.Calculate();

        valueA = 5.0f;
        valueB = 2.0f;
        subtract.Calculate().Should().BeApproximately(3.0f, 0.001f);
    }
}
