// // This file is part of the Moda.Core project.
// // 
// // This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// // If a copy of the MPL was not distributed with this file, You can obtain one at
// // https://mozilla.org/MPL/2.0/
//
// using FluentAssertions;
// using NUnit.Framework;
//
// namespace Moda.Core.UI.Builders;
//
// public class CellBuilderTests
// {
//     // Constructor Tests
//     //----------------------------------------------------------------------------------------------
//     
//     [Test]
//     public void ConstructorShouldSetProperties()
//     {
//         BoundariesRecipe recipe = new();
//         CellBuilder builder = new(recipe);
//         builder.BoundariesRecipe.Should().BeSameAs(recipe);
//     }
//
//
//     // AnchorAt() Tests
//     //----------------------------------------------------------------------------------------------
//     
//     [Test]
//     public void AnchorAtHorizontalShouldPassRecipe()
//     {
//         BoundariesRecipe recipe = new();
//         CellBuilder builder = new(recipe);
//         AnchoredHorizontalBuilder result = builder.AnchorAt(Horizontal.Right);
//         result.BoundariesRecipe.Should().BeSameAs(recipe);
//     }
//     
//     [Test]
//     [TestCase(Horizontal.Left, ExpectedResult = Neutral.Alpha)]
//     [TestCase(Horizontal.Center, ExpectedResult = Neutral.Center)]
//     [TestCase(Horizontal.Right, ExpectedResult = Neutral.Beta)]
//     public Neutral AnchorAtHorizontalShouldPassAnchor(Horizontal anchor)
//     {
//         BoundariesRecipe recipe = new();
//         CellBuilder builder = new(recipe);
//         AnchoredHorizontalBuilder result = builder.AnchorAt(anchor);
//         return result.Anchor;
//     }
//     
//     [Test]
//     [TestCase(Vertical.Top, ExpectedResult = Neutral.Alpha)]
//     [TestCase(Vertical.Middle, ExpectedResult = Neutral.Center)]
//     [TestCase(Vertical.Bottom, ExpectedResult = Neutral.Beta)]
//     public Neutral AnchorAtVerticalShouldPassAnchor(Vertical anchor)
//     {
//         BoundariesRecipe recipe = new();
//         CellBuilder builder = new(recipe);
//         AnchoredVerticalBuilder result = builder.AnchorAt(anchor);
//         return result.Anchor;
//     }
//     
// }
