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
using Optional;

namespace Moda.Core.Tests.UI.Lengths;

public class LengthOrFallbackTests
{
    [Test]
    public void CalculateShouldUsePreferredValueWhenAvailable()
    {
        Mock<IOptionalLength> preferred = new();
        preferred.Setup(a => a.TryCalculate()).Returns(17.0f.Some());
        LengthOrFallback lof = new(preferred.Object, Mock.Of<ILength>());
        lof.Calculate().Should().Be(17.0f);
    }
    
    [Test]
    public void CalculateShouldUseFallbackValueWhenPreferredIsNotAvailable()
    {
        Mock<IOptionalLength> preferred = new();
        preferred.Setup(a => a.TryCalculate()).Returns(Option.None<Single>());

        Mock<ILength> fallback = new();
        fallback.Setup(a => a.Calculate()).Returns(22.0f);
        
        LengthOrFallback lof = new(preferred.Object, fallback.Object);
        lof.Calculate().Should().Be(22.0f);
    }
}
