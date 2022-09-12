// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Moda.Core.Utility.Data.Tests;

[TestFixture]
public class ExtensionsTests
{
    // Dictionary.GetOrAdd() Tests
    // ---------------------------------------------------------------------------------------------

    // existing
    [Test]
    public void GetOrAddShouldReturnExisting()
    {
        Dictionary<Int32, String> dictionary = new()
        {
            [1] = "one",
            [2] = "two",
            [3] = "three"
        };

        dictionary.GetOrAdd(2, () => String.Empty).Should().Be("two");
    }

    // new, with factory
    [Test]
    public void GetOrAddShouldReturnCreatedValueIfMissing()
    {
        Dictionary<Int32, String> dictionary = new()
        {
            [1] = "one",
            [2] = "two",
            [3] = "three"
        };
        dictionary.GetOrAdd(4, () => "four").Should().Be("four");
    }

    [Test]
    public void GetOrAddShouldAddCreatedValueToCollectionIfMissing()
    {
        Dictionary<Int32, String> dictionary = new()
        {
            [1] = "one",
            [2] = "two",
            [3] = "three"
        };
        dictionary.GetOrAdd(4, () => "four");
        dictionary[4].Should().Be("four");
    }

    // new, default
    [Test]
    public void GetOrAddShouldReturnCreatedDefaultValueIfMissing()
    {
        Dictionary<Int32, SimpleObject> dictionary = new()
        {
            [1] = new SimpleObject { Value = 1 },
            [2] = new SimpleObject { Value = 2 },
            [3] = new SimpleObject { Value = 3 },
        };
        dictionary.GetOrAdd(4).Value.Should().Be(0);
    }

    [Test]
    public void GetOrAddShouldAddCreatedDefaultValueToCollectionIfMissing()
    {
        Dictionary<Int32, SimpleObject> dictionary = new()
        {
            [1] = new SimpleObject { Value = 1 },
            [2] = new SimpleObject { Value = 2 },
            [3] = new SimpleObject { Value = 3 },
        };
        dictionary.GetOrAdd(4);
        dictionary[4].Value.Should().Be(0);
    }

    // Support Classes
    // -----------------------------------------------------------------------------------------
    private class SimpleObject
    {
        public Int32 Value { get; set; }
    }
}
