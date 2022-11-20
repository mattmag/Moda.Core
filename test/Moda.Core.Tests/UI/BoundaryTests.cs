// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using FluentAssertions;
using Moda.Core.Utility.Maths;
using Moq;
using NUnit.Framework;
using Optional;

namespace Moda.Core.UI;

public class BoundaryTests
{
    // Length Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void RelativeRangeShouldReturnNoneIfBothAbsoluteCoordinatesAreNone()
    {
        Boundary boundary = new(Mock.Of<ICalculable>(), Mock.Of<ICalculable>());
        boundary.RelativeRange.Should().Be(Option.None<RangeF>());
    }
    
    [Test]
    public void RelativeRangeShouldReturnNoneIfAlphaAbsoluteIsNone()
    {
        Mock<ICalculable> lengthB = new();
        lengthB.Setup(a => a.Calculate()).Returns(3.5f);
        
        Boundary boundary = new(Mock.Of<ICalculable>(), lengthB.Object);
        
        boundary.BetaCoordinate.Calculate();
        
        boundary.RelativeRange.Should().Be(Option.None<RangeF>());
    }
    
    [Test]
    public void RelativeRangeShouldReturnNoneIfBetaAbsoluteIsNone()
    {
        Mock<ICalculable> lengthA = new();
        lengthA.Setup(a => a.Calculate()).Returns(3.5f);

        Boundary boundary = new(lengthA.Object, Mock.Of<ICalculable>());
        
        boundary.AlphaCoordinate.Calculate();
        
        boundary.RelativeRange.Should().Be(Option.None<RangeF>());
    }
    
    [Test]
    public void RelativeRangeShouldReturnAlphaAndBeta()
    {
        Mock<ICalculable> lengthA = new();
        lengthA.Setup(a => a.Calculate()).Returns(3.5f);

        Mock<ICalculable> lengthB = new();
        lengthB.Setup(a => a.Calculate()).Returns(10.5f);

        Boundary boundary = new(lengthA.Object, lengthB.Object);
        
        boundary.AlphaCoordinate.Calculate();
        boundary.BetaCoordinate.Calculate();
        
        boundary.RelativeRange.Should().Be(new RangeF(3.5f, 10.5f).Some());
    }


    // AbsoluteRange Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void AbsoluteRangeShouldReturnNoneIfBothAbsoluteCoordinatesAreNone()
    {
        Boundary boundary = new(Mock.Of<ICalculable>(), Mock.Of<ICalculable>());
        boundary.AbsoluteRange.Should().Be(Option.None<RangeF>());
    }
    
    [Test]
    public void AbsoluteRangeShouldReturnNoneIfAlphaAbsoluteIsNone()
    {
        Mock<ICalculable> lengthB = new();
        lengthB.Setup(a => a.Calculate()).Returns(3.5f);
        
        Boundary boundary = new(Mock.Of<ICalculable>(), lengthB.Object)
        {
            BetaCoordinate =
            {
                Tare = 2f.Some(),
            },
        };

        boundary.BetaCoordinate.Calculate();
        
        boundary.AbsoluteRange.Should().Be(Option.None<RangeF>());
    }
    
    [Test]
    public void AbsoluteRangeShouldReturnNoneIfBetaAbsoluteIsNone()
    {
        Mock<ICalculable> lengthA = new();
        lengthA.Setup(a => a.Calculate()).Returns(3.5f);
        
        Boundary boundary = new(lengthA.Object, Mock.Of<ICalculable>())
        {
            AlphaCoordinate =
            {
                Tare = 2f.Some(),
            },
        };
        
        boundary.AlphaCoordinate.Calculate();
        
        boundary.AbsoluteRange.Should().Be(Option.None<RangeF>());
    }
    
    [Test]
    public void AbsoluteRangeShouldReturnAlphaAndBeta()
    {
        Mock<ICalculable> lengthA = new();
        lengthA.Setup(a => a.Calculate()).Returns(3.5f);

        Mock<ICalculable> lengthB = new();
        lengthB.Setup(a => a.Calculate()).Returns(10.5f);
        
        Boundary boundary = new(lengthA.Object, lengthB.Object)
        {
            AlphaCoordinate =
            {
                Tare = 2f.Some(),
            },
            BetaCoordinate =
            {
                Tare = 2f.Some(),
            },
        };
        
        boundary.AlphaCoordinate.Calculate();
        boundary.BetaCoordinate.Calculate();
        
        boundary.AbsoluteRange.Should().Be(new RangeF(5.5f, 12.5f).Some());
    }


    // GetCoordinates() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void GetCoordinatesShouldReturnAlphaAndBeta()
    {
        Boundary boundary = new(Mock.Of<ICalculable>(), Mock.Of<ICalculable>());
        boundary.GetCoordinates().Should()
            .BeEquivalentTo(new[] { boundary.AlphaCoordinate, boundary.BetaCoordinate },
                a => a.WithStrictOrdering());
    }
    
    
}
