// // This file is part of the Moda.Core project.
// // 
// // This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// // If a copy of the MPL was not distributed with this file, You can obtain one at
// // https://mozilla.org/MPL/2.0/
//
// using FluentAssertions;
// using Moda.Core.Lengths;
// using NUnit.Framework;
// using Optional;
// using Optional.Unsafe;
//
// namespace Moda.Core.UI.Builders;
//
// public class AnchoredHorizontalBuilderTests
// {
//     // Constructor Tests
//     //----------------------------------------------------------------------------------------------
//     
//     [Test]
//     public void ConstructorShouldSetBoundaryRecipe()
//     {
//         BoundariesRecipe recipe = new();
//         AnchoredHorizontalBuilder builder = new(recipe, Horizontal.Left);
//         builder.BoundariesRecipe.Should().BeSameAs(recipe);
//     }
//     
//     [Test]
//     [TestCase(Horizontal.Left, ExpectedResult = Neutral.Alpha)]
//     [TestCase(Horizontal.Center, ExpectedResult = Neutral.Center)]
//     [TestCase(Horizontal.Right, ExpectedResult = Neutral.Beta)]
//     public Neutral ConstructorShouldSetAndConvertAnchor(Horizontal anchor)
//     {
//         BoundariesRecipe recipe = new();
//         AnchoredHorizontalBuilder builder = new(recipe, anchor);
//         return builder.Anchor;
//     }
//
//
//     // WithWidth() Tests
//     //----------------------------------------------------------------------------------------------
//     
//     [Test]
//     public void WithWidthShouldSetXBoundaryCoordinates()
//     {
//         Horizontal anchor = Horizontal.Left;
//         AnchoredHorizontalBuilder builder = new(new(), anchor);
//         builder.WithWidth(new Pixels(200));
//
//         AxisRecipe axisRecipe = builder.BoundariesRecipe.GetAxisRecipe(Axis.X);
//         Option<ILength> alpha = axisRecipe.Alpha;
//         alpha.HasValue.Should().BeTrue();
//         alpha.ValueOrFailure().Calculate().Should().Be(0);
//         
//         Option<ILength> beta = axisRecipe.Beta;
//         beta.HasValue.Should().BeTrue();
//         beta.ValueOrFailure().Calculate().Should().Be(200);
//     }
// }