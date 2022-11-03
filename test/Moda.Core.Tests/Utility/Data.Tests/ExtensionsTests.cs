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


    // List.AddSorted() Tests
    //----------------------------------------------------------------------------------------------

    [Test]
    public void AddSortedShouldAddValueAtSortedIndex()
    {
        List<int> list = new() { 1, 2, 4 };
        list.AddSorted(3, Comparer<Int32>.Default);
        list.Should().BeEquivalentTo(new[] { 1, 2, 3, 4 }, a => a.WithStrictOrdering());
    }
    
    [Test]
    public void AddSortedShouldAddHighestValueAtEnd()
    {
        List<int> list = new() { 1, 2, 3 };
        list.AddSorted(5, Comparer<Int32>.Default);
        list.Should().BeEquivalentTo(new[] { 1, 2, 3, 5 }, a => a.WithStrictOrdering());
    }
    
    [Test]
    public void AddSortedShouldAddLowestValueAtBeginning()
    {
        List<int> list = new() { 2, 3, 4 };
        list.AddSorted(1, Comparer<Int32>.Default);
        list.Should().BeEquivalentTo(new[] { 1, 2, 3, 4 }, a => a.WithStrictOrdering());
    }
    
    [Test]
    public void AddSortedShouldUseProvidedComparer()
    {
        Comparer<ValueWrapper<Int32>> comparer =Comparer<ValueWrapper<Int32>>.Create(
            (a, b) => a.Value.CompareTo(b.Value));
            
        List<ValueWrapper<Int32>> list = new() { new(1), new(2), new(4) };
        list.AddSorted(new(3), comparer);
        list.Should().BeEquivalentTo(new ValueWrapper<Int32>[] { new(1), new(2), new(3), new(4) },
            a => a .WithStrictOrdering());
    }
    
    [Test]
    public void AddSortedShouldAddAfterExistingValue()
    {
        Comparer<ValueWrapper<Int32>> comparer =Comparer<ValueWrapper<Int32>>.Create(
            (a, b) => a.Value.CompareTo(b.Value));
        
        ValueWrapper<Int32> toAdd = new(2);
        List<ValueWrapper<Int32>> list = new() { new(1), new(2), new(3) };
        list.AddSorted(toAdd, comparer);

        // sanity check
        list.Should().BeEquivalentTo(new ValueWrapper<Int32>[] { new(1), new(2), new(2), new(3) },
            a => a.WithStrictOrdering());
        
        // actual test
        list[2].Should().BeSameAs(toAdd);
    }


    private record ValueWrapper<T>(T Value);


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
    
    
    // Stack.PushRange() Tests
    //----------------------------------------------------------------------------------------------


    [Test]
    public void PushRangeShouldPushEachItem()
    {
        Stack<int> stack = new();
        stack.Push(1);
        stack.Push(2);
        
        stack.PushRange(new[] { 3, 4, 5, 6 });

        stack.Should().ContainInOrder(6, 5, 4, 3, 2, 1);
    }
    
    

    // Support Classes
    // -----------------------------------------------------------------------------------------
    private class SimpleObject
    {
        public Int32 Value { get; set; }
    }
}
