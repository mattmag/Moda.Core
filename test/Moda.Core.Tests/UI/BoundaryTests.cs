// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using FluentAssertions;
using Moda.Core.UI;
using Moda.Core.Utility.Maths;
using Moq;
using NUnit.Framework;
using Optional;

namespace Moda.Core.Tests.UI;

public class BoundaryTests
{
    // Length Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void RelativeRangeShouldReturnNoneIfBothAbsoluteCoordinatesAreNone()
    {
        Boundary boundary = new(GetMockedCell(), Axis.X,
            Mock.Of<ICalculation>(), Mock.Of<ICalculation>());
        boundary.RelativeRange.Should().Be(Option.None<RangeF>());
    }
    
    [Test]
    public void RelativeRangeShouldReturnNoneIfAlphaAbsoluteIsNone()
    {
        Mock<ICalculation> lengthB = new();
        lengthB.Setup(a => a.Calculate()).Returns(3.5f);
        
        Boundary boundary = new(GetMockedCell(), Axis.X,
            Mock.Of<ICalculation>(), lengthB.Object);
        
        boundary.BetaCoordinate.Calculate();
        
        boundary.RelativeRange.Should().Be(Option.None<RangeF>());
    }
    
    [Test]
    public void RelativeRangeShouldReturnNoneIfBetaAbsoluteIsNone()
    {
        Mock<ICalculation> lengthA = new();
        lengthA.Setup(a => a.Calculate()).Returns(3.5f);

        Boundary boundary = new(GetMockedCell(), Axis.X,
            lengthA.Object, Mock.Of<ICalculation>());
        
        boundary.AlphaCoordinate.Calculate();
        
        boundary.RelativeRange.Should().Be(Option.None<RangeF>());
    }
    
    [Test]
    public void RelativeRangeShouldReturnAlphaAndBeta()
    {
        Mock<ICalculation> lengthA = new();
        lengthA.Setup(a => a.Calculate()).Returns(3.5f);

        Mock<ICalculation> lengthB = new();
        lengthB.Setup(a => a.Calculate()).Returns(10.5f);

        Boundary boundary = new(GetMockedCell(), Axis.X,
            lengthA.Object, lengthB.Object);
        
        boundary.AlphaCoordinate.Calculate();
        boundary.BetaCoordinate.Calculate();
        
        boundary.RelativeRange.Should().Be(new RangeF(3.5f, 10.5f).Some());
    }


    // AbsoluteRange Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void AbsoluteRangeShouldReturnNoneIfBothAbsoluteCoordinatesAreNone()
    {
        Boundary boundary = new(GetMockedCell(), Axis.X,
            Mock.Of<ICalculation>(), Mock.Of<ICalculation>());
        boundary.AbsoluteRange.Should().Be(Option.None<RangeF>());
    }
    
    [Test]
    public void AbsoluteRangeShouldReturnNoneIfAlphaAbsoluteIsNone()
    {
        Mock<ICalculation> lengthB = new();
        lengthB.Setup(a => a.Calculate()).Returns(3.5f);
        
        Boundary boundary = new(GetMockedCell(), Axis.X,
            Mock.Of<ICalculation>(), lengthB.Object)
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
        Mock<ICalculation> lengthA = new();
        lengthA.Setup(a => a.Calculate()).Returns(3.5f);
        
        Boundary boundary = new(GetMockedCell(), Axis.X,
            lengthA.Object, Mock.Of<ICalculation>())
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
        Mock<ICalculation> lengthA = new();
        lengthA.Setup(a => a.Calculate()).Returns(3.5f);

        Mock<ICalculation> lengthB = new();
        lengthB.Setup(a => a.Calculate()).Returns(10.5f);
        
        Boundary boundary = new(GetMockedCell(), Axis.X,
            lengthA.Object, lengthB.Object)
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
        Boundary boundary = new(GetMockedCell(), Axis.X,
            Mock.Of<ICalculation>(), Mock.Of<ICalculation>());
        boundary.GetCoordinates().Should()
            .BeEquivalentTo(new[] { boundary.AlphaCoordinate, boundary.BetaCoordinate },
                a => a.WithStrictOrdering());
    }
    
    // Support
    //----------------------------------------------------------------------------------------------
    private static Cell GetMockedCell() => new(Mock.Of<IHoneyComb>(), Mock.Of<ICalculation>(),
        Mock.Of<ICalculation>(), Mock.Of<ICalculation>(), Mock.Of<ICalculation>());

    
}
