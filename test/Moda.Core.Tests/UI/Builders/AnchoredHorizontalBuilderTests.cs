// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using FluentAssertions;
using Moda.Core.UI;
using Moda.Core.UI.Builders;
using Moda.Core.UI.Lengths;
using NUnit.Framework;
using Optional;
using Optional.Unsafe;

namespace Moda.Core.Tests.UI.Builders;

public class AnchoredHorizontalBuilderTests
{
    // Constructor Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    [TestCase(Horizontal.Left, ExpectedResult = Neutral.Alpha)]
    [TestCase(Horizontal.Center, ExpectedResult = Neutral.Center)]
    [TestCase(Horizontal.Right, ExpectedResult = Neutral.Beta)]
    public Neutral ConstructorShouldSetAndConvertAnchor(Horizontal anchor)
    {
        CellBuilder cellBuilder = new();
        AnchoredHorizontalBuilder axisBuilder = new(cellBuilder, anchor);
        return axisBuilder.Anchor;
    }


    // WithWidth() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void WithWidthShouldSetXBoundaryCoordinates()
    {
        Horizontal anchor = Horizontal.Left;
        CellBuilder cellBuilder = new();
        AnchoredHorizontalBuilder axisBuilder = new(cellBuilder, anchor);
        axisBuilder.WithWidth(new Pixels(200));

        AxisRecipe axisRecipe = cellBuilder.BoundariesRecipe.GetAxisRecipe(Axis.X);
        Option<Length> alpha = axisRecipe.Alpha;
        alpha.HasValue.Should().BeTrue();
        alpha.ValueOrFailure().Calculate().Should().Be(0);
        
        Option<Length> beta = axisRecipe.Beta;
        beta.HasValue.Should().BeTrue();
        beta.ValueOrFailure().Calculate().Should().Be(200);
    }
}