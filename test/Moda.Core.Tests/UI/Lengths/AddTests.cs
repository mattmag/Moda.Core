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

namespace Moda.Core.Tests.UI.Lengths;

public class AddTests
{
    [Test]
    public void CalculateShouldReturnSumOfLengthResults()
    {
        Mock<ILength> lengthA = new();
        lengthA.Setup(a => a.Calculate()).Returns(13.0f);
        
        Mock<ILength> lengthB = new();
        lengthB.Setup(a => a.Calculate()).Returns(22.0f);

        Add add = new(lengthA.Object, lengthB.Object);
        add.Calculate().Should().BeApproximately(35.0f, 0.001f);
    }
    
    [Test]
    public void CalculateShouldReflectNewLengthValues()
    {
        Single valueA = 13.0f;
        Mock<ILength> lengthA = new();
        lengthA.Setup(a => a.Calculate()).Returns(() => valueA);
        
        Single valueB = 22.0f;
        Mock<ILength> lengthB = new();
        lengthB.Setup(a => a.Calculate()).Returns(() => valueB);

        Add add = new(lengthA.Object, lengthB.Object);
        add.Calculate();

        valueA = 2.0f;
        valueB = 3.0f;
        add.Calculate().Should().BeApproximately(5.0f, 0.001f);
    }
}
