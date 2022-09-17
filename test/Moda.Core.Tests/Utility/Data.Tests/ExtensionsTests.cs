// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Optional;

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


    // Collection.AddIfSome() Tests
    //----------------------------------------------------------------------------------------------
    [Test]
    public void AddIfSomeShouldAddIfValueExist()
    {
        List<int> list = new() { 1, 2, 3 };
        
        list.AddIfSome(4.Some());
        
        list.Should().Contain(4);
    }
    
    [Test]
    public void AddIfSomeShouldAddNothingIfValueIsEmpty()
    {
        List<int> list = new() { 1, 2, 3 };
        
        list.AddIfSome(Option.None<int>());

        list.Should().BeEquivalentTo(new[] { 1, 2, 3 });
    }
    
    // Item.IfSomeAddTo() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void IfSomeAddToShouldAddIfValueExist()
    {
        List<int> list = new() { 1, 2, 3 };
        
        4.Some().IfSomeAddTo(list);
        
        list.Should().Contain(4);
    }
    
    [Test]
    public void IfSomeAddToShouldAddNothingIfValueIsEmpty()
    {
        List<int> list = new() { 1, 2, 3 };
        
        Option.None<int>().IfSomeAddTo(list);

        list.Should().BeEquivalentTo(new[] { 1, 2, 3 });
    }


    // Dictionary.TryGetMultiple() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void TryGetMultipleShouldReturnFoundValues()
    {
        Dictionary<Int32, String> dictionary = new()
            {
                { 1, "one" },
                { 2, "two" },
                { 3, "three" },
                { 4, "four" },
            };
        dictionary.TryGetMultiple(1, 3).Should().BeEquivalentTo(
            new Dictionary<Int32, String>
            {
                { 1, "one" },
                { 3, "three" },
            }
        );
    }
    
    [Test]
    public void TryGetMultipleShouldNotReturnValuesNotFound()
    {
        Dictionary<Int32, String> dictionary = new()
            {
                { 1, "one" },
                { 2, "two" },
                { 3, "three" },
                { 4, "four" },
            };
        dictionary.TryGetMultiple(1, 3, 5).Should().BeEquivalentTo(
                new Dictionary<Int32, String>
                {
                    { 1, "one" },
                    { 3, "three" },
                }
            );
    }


    // Dictionary.AddRange Tests()
    //----------------------------------------------------------------------------------------------

    [Test]
    public void AddRangeShouldAddItems()
    {
        Dictionary<Int32, String> dictionary = new();
        
        dictionary.AddRange(
                new KeyValuePair<Int32, String>(1, "one"),
                new KeyValuePair<Int32, String>(2, "two"),
                new KeyValuePair<Int32, String>(3, "three")
            );
        dictionary.Should().BeEquivalentTo(
                new Dictionary<Int32, String>
                {
                    { 1, "one" },
                    { 2, "two" },
                    { 3, "three" },
                }
            );
    }
    
    [Test]
    public void AddRangeShouldThrowAggregateExceptionWhenKeysAlreadyExist()
    {
        Dictionary<Int32, String> dictionary = new()
            {
                { 1, "one" },
                { 2, "two" },
                { 3, "three" },
            };
        
        dictionary.Invoking(a => a.AddRange(
                new KeyValuePair<Int32, String>(2, "two"),
                new KeyValuePair<Int32, String>(3, "three")
            ))
            .Should().Throw<KeysAlreadyExistException>()
            .And.KeysAsStrings.Should().BeEquivalentTo("2", "3");
    }


    // Dictionary.AddOrUpdateRange() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void AddOrUpdateRangeShouldAddNewValues()
    {
        Dictionary<Int32, String> dictionary = new();
        
        dictionary.AddOrUpdateRange(
                new KeyValuePair<Int32, String>(1, "one"),
                new KeyValuePair<Int32, String>(2, "two"),
                new KeyValuePair<Int32, String>(3, "three")
            );
        dictionary.Should().BeEquivalentTo(
                new Dictionary<Int32, String>
                {
                    { 1, "one" },
                    { 2, "two" },
                    { 3, "three" },
                }
            );
    }
    
    
    [Test]
    public void AddOrUpdateRangeShouldUpdateExisting()
    {
        Dictionary<Int32, String> dictionary = new Dictionary<Int32, String>
            {
                { 1, "one" },
                { 3, "three" },
            };
        
        dictionary.AddOrUpdateRange(
                new KeyValuePair<Int32, String>(2, "two"),
                new KeyValuePair<Int32, String>(3, "three-updated")
            );
        dictionary.Should().BeEquivalentTo(
                new Dictionary<Int32, String>
                {
                    { 1, "one" },
                    { 2, "two" },
                    { 3, "three-updated" },
                }
            );
    }


    // KeyValuePair<TKey, TValue>.KeyedOn() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void KeyedOnShouldCreateNewInstance()
    {
        "one".KeyedOn(1).Should().Be(new KeyValuePair<Int32, String>(1, "one"));
    }
    

    // Support Classes
    // -----------------------------------------------------------------------------------------
    private class SimpleObject
    {
        public Int32 Value { get; set; }
    }
}
