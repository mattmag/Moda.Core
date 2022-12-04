// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using FluentAssertions;
using Moda.Core.UI.Builders;
using NUnit.Framework;
using Optional.Unsafe;

namespace Moda.Core.Tests.UI.Builders;

public class IngredientTests
{
    // Get() Tests
    //----------------------------------------------------------------------------------------------
    [Test]
    public void GetShouldReturnSetValue()
    {
        Ingredient<Single> ingredient = new();
        ingredient.Set(13.0f);
        ingredient.Get().Should().Be(13.0f);
    }


    [Test]
    public void GetWithoutSetShouldThrowException()
    {
        Ingredient<Single> ingredient = new();
        ingredient.Invoking(a => a.Get()).Should().Throw<OptionValueMissingException>();
    }


    [Test]
    public void GetWithoutSetShouldReturnDefaultValueIfAny()
    {
        Ingredient<Single> ingredient = new(7.0f);
        ingredient.Get().Should().Be(7.0f);
    }


    // Set() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void SetAgainShouldThrowException()
    {
        Ingredient<Single> ingredient = new();
        ingredient.Set(13.0f);
        ingredient.Invoking(a => a.Set(22.0f)).Should().Throw<InvalidOperationException>();
    }
}
