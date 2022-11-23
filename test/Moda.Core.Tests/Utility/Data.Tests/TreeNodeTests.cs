// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Events;
using Moda.Core.Support;
using Moq;
using NUnit.Framework;
using Optional;

namespace Moda.Core.Utility.Data.Tests;

public class TreeNodeTests
{
    // AppendChild() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void AppendChildShouldUpdateParent()
    {
        TestNode parent = new();
        TestNode child = new();
        parent.AppendChild(child);
        child.Parent.Should().Be(parent.Some());
    }
    
    [Test]
    public void AppendChildThatAlreadyExistsShouldThrowException()
    {
        TestNode parent = new();
        TestNode child = new();
        parent.AppendChild(child);
        parent.Invoking(p => p.AppendChild(child)).Should().Throw<ChildAlreadyExistsException>();
    }
    
    [Test]
    public void AppendChildShouldFireEventFromChildWithNewParent()
    {
        TestNode parent = new();
        TestNode child = new();
        
        using IMonitor<TestNode>? monitor = child.Monitor();
        
        parent.AppendChild(child);

        monitor.Should().Raise(nameof(TestNode.ParentChanged))
            .WithSender(child)
            .WithArgs<ValueChangedArgs<Option<TestNode>>>(a =>
                a.OldValue == Option.None<TestNode>() && a.NewValue == parent.Some());
    }
    
    [Test]
    public void AppendChildShouldFireChildrenChangedEvent()
    {
        TestNode parent = new();
        TestNode child = new();
        
        using IMonitor<TestNode>? monitor = parent.Monitor();
        
        parent.AppendChild(child);

        monitor.Should().Raise(nameof(TestNode.ChildrenChanged))
            .WithSender(parent)
            .WithAssertedArgs<CollectionChangedArgs<TestNode>>(a =>
                {
                    a.ItemsAdded.Should().BeEquivalentTo(new[] { child });
                    a.ItemsRemoved.Should().BeEmpty();
                });

    }
    
    [Test]
    public void AppendChildShouldAddToChildren()
    {
        TestNode parent = new();
        TestNode child = new();
        parent.AppendChild(child);

        parent.Children.Should().BeEquivalentTo(new[] { child });
    }
    
    [Test]
    public void AppendChildShouldAppendAtEnd()
    {
        TestNode parent = new();
        
        TestNode child1 = new();
        parent.AppendChild(child1);
        TestNode child2 = new();
        parent.AppendChild(child2);
        TestNode child3 = new();
        parent.AppendChild(child3);

        parent.Children.Should().BeEquivalentTo(new[] { child1, child2, child3 });
    }


    [Test]
    public void AppendChildShouldRemoveFromOtherParentIfExists()
    {
        TestNode parent1 = new();
        TestNode childA = new();
        parent1.AppendChild(childA);
        TestNode childB = new();
        parent1.AppendChild(childB);
        TestNode childC = new();
        parent1.AppendChild(childC);
        
        TestNode parent2 = new();
        parent2.AppendChild(childB);

        parent1.Children.Should().BeEquivalentTo(new[] { childA, childC });
        parent2.Children.Should().BeEquivalentTo(new[] { childB });
    }


    [Test]
    public void AppendChildShouldFireOneParentChangedEventWhenTransferringToNewParent()
    {
        TestNode parent1 = new();
        TestNode child = new();
        parent1.AppendChild(child);
        
        TestNode parent2 = new();
        
        using IMonitor<TestNode>? monitor = child.Monitor();
        Int32 parentChangedCount = 0;
        child.ParentChanged += (_, _) => parentChangedCount++; 
        
        parent2.AppendChild(child);

        parentChangedCount.Should().Be(1);
        monitor.Should().Raise(nameof(TestNode.ParentChanged))
            .WithSender(child)
            .WithArgs<ValueChangedArgs<Option<TestNode>>>(a =>
                a.OldValue == parent1.Some() && a.NewValue == parent2.Some());
    }


    [Test]
    public void AppendChildShouldCallOnChildAdded()
    {
        Mock<TestNodeWithOverrides> parent = new();
        TestNodeWithOverrides child = new();
        parent.CallBase = true;
        parent.Setup(p => p.HandleOnChildAdded(It.IsAny<TestNodeWithOverrides>()));
        
        parent.Object.AppendChild(child);
        
        parent.Verify(p => p.HandleOnChildAdded(It.Is<TestNodeWithOverrides>(c => c == child)), 
            Times.Once);
    }
    
    
    // InsertChild() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void InsertChildShouldUpdateParent()
    {
        TestNode parent = new();
        TestNode child = new();
        parent.InsertChild(child, 0);
        child.Parent.Should().Be(parent.Some());
    }
    
    [Test]
    public void InsertChildThatAlreadyExistsShouldThrowException()
    {
        TestNode parent = new();
        TestNode child = new();
        parent.InsertChild(child, 0);
        parent.Invoking(p => p.InsertChild(child, 0)).Should().Throw<ChildAlreadyExistsException>();
    }
    
    [Test]
    public void InsertChildShouldFireEventFromChildWithNewParent()
    {
        TestNode parent = new();
        TestNode child = new();
        
        using IMonitor<TestNode>? monitor = child.Monitor();
        
        parent.InsertChild(child, 0);

        monitor.Should().Raise(nameof(TestNode.ParentChanged))
            .WithSender(child)
            .WithArgs<ValueChangedArgs<Option<TestNode>>>(a =>
                a.OldValue == Option.None<TestNode>() && a.NewValue == parent.Some());
    }
    
    [Test]
    public void InsertChildShouldFireChildrenChangedEvent()
    {
        TestNode parent = new();
        TestNode child = new();
        
        using IMonitor<TestNode>? monitor = parent.Monitor();
        
        parent.InsertChild(child, 0);

        monitor.Should().Raise(nameof(TestNode.ChildrenChanged))
            .WithSender(parent)
            .WithAssertedArgs<CollectionChangedArgs<TestNode>>(a =>
                {
                    a.ItemsAdded.Should().BeEquivalentTo(new[] { child });
                    a.ItemsRemoved.Should().BeEmpty();
                });

    }
    
    [Test]
    public void InsertChildShouldAddToChildren()
    {
        TestNode parent = new();
        TestNode child = new();
        parent.InsertChild(child, 0);

        parent.Children.Should().BeEquivalentTo(new[] { child });
    }
    

    [Test]
    public void InsertChildShouldRemoveFromOtherParentIfExists()
    {
        TestNode parent1 = new();
        TestNode childA = new();
        parent1.InsertChild(childA, 0);
        TestNode childB = new();
        parent1.InsertChild(childB, 0);
        TestNode childC = new();
        parent1.InsertChild(childC, 0);
        
        TestNode parent2 = new();
        parent2.InsertChild(childB, 0);

        parent1.Children.Should().BeEquivalentTo(new[] { childA, childC });
        parent2.Children.Should().BeEquivalentTo(new[] { childB });
    }


    [Test]
    public void InsertChildShouldFireOneParentChangedEventWhenTransferringToNewParent()
    {
        TestNode parent1 = new();
        TestNode child = new();
        parent1.AppendChild(child);
        
        TestNode parent2 = new();
        
        using IMonitor<TestNode>? monitor = child.Monitor();
        Int32 parentChangedCount = 0;
        child.ParentChanged += (_, _) => parentChangedCount++; 
        
        parent2.InsertChild(child, 0);

        parentChangedCount.Should().Be(1);
        monitor.Should().Raise(nameof(TestNode.ParentChanged))
            .WithSender(child)
            .WithArgs<ValueChangedArgs<Option<TestNode>>>(a =>
                a.OldValue == parent1.Some() && a.NewValue == parent2.Some());
    }


    [Test]
    public void InsertChildShouldCallOnChildAdded()
    {
        Mock<TestNodeWithOverrides> parent = new();
        TestNodeWithOverrides child = new();
        parent.CallBase = true;
        parent.Setup(p => p.HandleOnChildAdded(It.IsAny<TestNodeWithOverrides>()));
        
        parent.Object.InsertChild(child, 0);
        
        parent.Verify(p => p.HandleOnChildAdded(It.Is<TestNodeWithOverrides>(c => c == child)));
    }
    
    
    [Test]
    public void InsertChildShouldInsertAtIndex()
    {
        TestNode parent = new();
        
        TestNode child1 = new();
        parent.AppendChild(child1);
        TestNode child2 = new();
        parent.AppendChild(child2);
        TestNode child3 = new();
        parent.AppendChild(child3);
        
        TestNode child4 = new();
        parent.InsertChild(child4, 2);

        parent.Children.Should().BeEquivalentTo(new[] { child1, child2, child4, child3 });
    }
    
    
    [Test]
    public void InsertChildAt0ShouldInsertAtBeginning()
    {
        TestNode parent = new();
        
        TestNode child1 = new();
        parent.AppendChild(child1);
        TestNode child2 = new();
        parent.AppendChild(child2);
        TestNode child3 = new();
        parent.AppendChild(child3);
        
        TestNode child4 = new();
        parent.InsertChild(child4, 0);

        parent.Children.Should().BeEquivalentTo(new[] { child4, child1, child2, child3 });
    }
    
    [Test]
    public void InsertChildAtCountShouldInsertAtEnd()
    {
        TestNode parent = new();
        
        TestNode child1 = new();
        parent.AppendChild(child1);
        TestNode child2 = new();
        parent.AppendChild(child2);
        TestNode child3 = new();
        parent.AppendChild(child3);
        
        TestNode child4 = new();
        parent.InsertChild(child4, 3);

        parent.Children.Should().BeEquivalentTo(new[] { child1, child2, child3, child4 });
    }


    [Test]
    public void InsertChildAtNegativeIndexShouldThrowArgumentOutOfRangeException()
    {
        TestNode parent = new();
        
        TestNode child1 = new();
        parent.AppendChild(child1);
        TestNode child2 = new();
        parent.AppendChild(child2);
        
        TestNode child3 = new();

        parent.Invoking(a => a.InsertChild(child3, -3))
            .Should().Throw<ArgumentOutOfRangeException>();
    }
    
    [Test]
    public void InsertChildAtIndexLargerThanCountShouldThrowArgumentOutOfRangeException()
    {
        TestNode parent = new();
        
        TestNode child1 = new();
        parent.AppendChild(child1);
        TestNode child2 = new();
        parent.AppendChild(child2);
        
        TestNode child3 = new();

        parent.Invoking(a => a.InsertChild(child3, 5))
            .Should().Throw<ArgumentOutOfRangeException>();
    }
    
    [Test]
    public void InsertChildAtInvalidIndexShouldNotSetParent()
    {
        TestNode parent = new();
        TestNode child = new();
        
        try
        {
            parent.InsertChild(child, -1);
        }
        catch
        {
        }
        
        child.Parent.Should().Be(Option.None<TestNode>());
    }
    
    [Test]
    public void InsertChildAtInvalidIndexShouldNotFireParentChangedEvent()
    {
        TestNode parent = new();
        TestNode child = new();
        
        using IMonitor<TestNode>? monitor = child.Monitor();
        
        try
        {
            parent.InsertChild(child, -1);
        }
        catch
        {
        }

        monitor.Should().NotRaise(nameof(TestNode.ParentChanged));
    }


    [Test]
    public void InsertChildAtInvalidIndexShouldNotFireChildrenChangedEvent()
    {
        TestNode parent = new();
        TestNode child = new();

        using IMonitor<TestNode>? monitor = parent.Monitor();

        try
        {
            parent.InsertChild(child, -1);
        }
        catch
        {
        }


        monitor.Should().NotRaise(nameof(TestNode.ChildrenChanged));
    }
    
    [Test]
    public void InsertChildAtInvalidIndexShouldNotAddToChildren()
    {
        TestNode parent = new();
        TestNode child1 = new();
        parent.AppendChild(child1);
        
        TestNode child2 = new();
        try
        {
            parent.InsertChild(child2, -1);
        }
        catch
        {
        }

        parent.Children.Should().BeEquivalentTo(new[] { child1 });
    }
    
    [Test]
    public void InsertChildAtInvalidIndexShouldNotRemoveFromOtherParentIfExists()
    {
        TestNode parent1 = new();
        TestNode childA = new();
        parent1.AppendChild(childA);
        TestNode childB = new();
        parent1.AppendChild(childB);
        TestNode childC = new();
        parent1.AppendChild(childC);
        
        TestNode parent2 = new();
        try
        {
            parent2.InsertChild(childB, -1);
        }
        catch
        {
        }

        parent1.Children.Should().BeEquivalentTo(new[] { childA, childC, childB });
        parent2.Children.Should().BeEmpty();
    }

    [Test]
    public void InsertChildAtInvalidIndexShouldNotCallOnChildAdded()
    {
        Mock<TestNodeWithOverrides> parent = new();
        TestNodeWithOverrides child = new();
        parent.CallBase = true;
        parent.Setup(p => p.HandleOnChildAdded(It.IsAny<TestNodeWithOverrides>()));

        try
        {
            parent.Object.InsertChild(child, -1);
        }
        catch
        {
        }
        
        parent.Verify(p => p.HandleOnChildAdded(It.Is<TestNodeWithOverrides>(c => c == child)),
            Times.Never);
    }

    // RemoveChild() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void RemoveChildShouldUpdateParentToNone()
    {
        TestNode parent = new();
        TestNode child = new();
        parent.AppendChild(child);
        
        parent.RemoveChild(child);
        child.Parent.Should().Be(Option.None<TestNode>());
    }
    
    [Test]
    public void RemoveChildShouldRemoveFromChildren()
    {
        TestNode parent = new();
        TestNode childA = new();
        parent.AppendChild(childA);
        TestNode childB = new();
        parent.AppendChild(childB);
        TestNode childC = new();
        parent.AppendChild(childC);
        
        parent.RemoveChild(childB);
        parent.Children.Should().BeEquivalentTo(new[] { childA, childC });
    }
    
    [Test]
    public void RemoveChildShouldFireEventFromChildWithNoParent()
    {
        TestNode parent = new();
        TestNode child = new();
        parent.AppendChild(child);
        
        using IMonitor<TestNode>? monitor = child.Monitor();
        
        parent.RemoveChild(child);
        
        monitor.Should().Raise(nameof(TestNode.ParentChanged))
            .WithSender(child)
            .WithArgs<ValueChangedArgs<Option<TestNode>>>(a =>
                a.OldValue == parent.Some() && a.NewValue == Option.None<TestNode>());
    }
    
    [Test]
    public void RemoveChildShouldFireChildrenChangedEvent()
    {
        TestNode parent = new();
        TestNode child = new();
        parent.AppendChild(child);
        
        using IMonitor<TestNode>? monitor = parent.Monitor();
        
        parent.RemoveChild(child);

        monitor.Should().Raise(nameof(TestNode.ChildrenChanged))
            .WithSender(parent)
            .WithAssertedArgs<CollectionChangedArgs<TestNode>>(a =>
                {
                    a.ItemsAdded.Should().BeEmpty();
                    a.ItemsRemoved.Should().BeEquivalentTo(new[] { child });
                });

    }
    
    [Test]
    public void RemoveChildThatDoesNotExistShouldThrowException()
    {
        TestNode parent1 = new();
        TestNode childA = new();
        
        parent1.Invoking(p => p.RemoveChild(childA)).Should().Throw<ChildDoesNotExistException>();
    }
    
    [Test]
    public void RemoveWrongChildShouldNotUpdateParentToNone()
    {
        TestNode parent1 = new();
        TestNode childA = new();
        parent1.AppendChild(childA);
        
        TestNode parent2 = new();
        TestNode childB = new();
        parent2.AppendChild(childB);
        
        parent1.Invoking(p => p.RemoveChild(childB)).Should().Throw<ChildDoesNotExistException>();
        childB.Parent.Should().Be(parent2.Some());
    }
    
    [Test]
    public void RemoveWrongChildShouldNotRaiseParentChangedEvent()
    {
        TestNode parent1 = new();
        TestNode childA = new();
        parent1.AppendChild(childA);
        
        TestNode parent2 = new();
        TestNode childB = new();
        parent2.AppendChild(childB);
        
        using IMonitor<TestNode>? monitor = childB.Monitor();
        parent1.Invoking(p => p.RemoveChild(childB)).Should().Throw<ChildDoesNotExistException>();
        monitor.Should().NotRaise(nameof(TestNode.ParentChanged));
    }
    
    [Test]
    public void RemoveChildShouldCallOnChildRemoved()
    {
        Mock<TestNodeWithOverrides> parent = new();
        TestNodeWithOverrides child = new();
        parent.Object.AppendChild(child);
        parent.CallBase = true;
        parent.Setup(p => p.HandleOnChildRemoved(It.IsAny<TestNodeWithOverrides>()));
        
        parent.Object.RemoveChild(child);
        
        parent.Verify(p => p.HandleOnChildRemoved(It.Is<TestNodeWithOverrides>(c => c == child)));
    }
    
    
    // Enumerator Tests
    //----------------------------------------------------------------------------------------------

    [Test]
    public void EnumeratingShouldIncludeSelfFirst()
    {
        TestNode parent = new();
        TestNode child1 = new();
        parent.AppendChild(child1);
        TestNode child2 = new();
        parent.AppendChild(child2);

        List<TestNode> result = parent.ToList();
        result.First().Should().BeSameAs(parent);
    }
    
    [Test]
    public void EnumeratingShouldIncludeChildren()
    {
        TestNode parent = new();
        TestNode child1 = new();
        parent.AppendChild(child1);
        TestNode child2 = new();
        parent.AppendChild(child2);

        List<TestNode> result = parent.ToList();
        result.Should().BeEquivalentTo(new[] { parent, child1, child2 });
    }
    
    
    [Test]
    public void EnumeratingShouldIncludeDeepChildren()
    {
        TestNode parent = new();
        TestNode childA1 = new();
        parent.AppendChild(childA1);
        TestNode childA2 = new();
        parent.AppendChild(childA2);
        TestNode childB1 = new();
        childA1.AppendChild(childB1);
        TestNode childB2 = new();
        childA1.AppendChild(childB2);
        TestNode childB3 = new();
        childA2.AppendChild(childB3);
        TestNode childC1 = new();
        childB1.AppendChild(childC1);
        
        List<TestNode> result = parent.ToList();
        result.Should().BeEquivalentTo(new[]
            {
                parent, childA1, childA2, childB1, childB2, childB3, childC1,
            });
    }
    
    [Test]
    public void EnumeratingShouldReturnBreadthFirst()
    {
        TestNode parent = new();
        TestNode childA1 = new("A1");
        parent.AppendChild(childA1);
        TestNode childA2 = new("A2");
        parent.AppendChild(childA2);
        TestNode childB1 = new("B1");
        childA1.AppendChild(childB1);
        TestNode childB2 = new("B2");
        childA1.AppendChild(childB2);
        TestNode childB3 = new("B3");
        childA2.AppendChild(childB3);
        TestNode childC1 = new("C1");
        childB1.AppendChild(childC1);
        
        List<TestNode> result = parent.ToList();
        result.Should().BeEquivalentTo(new[]
            {
                parent, childA1, childA2, childB1, childB2, childB3, childC1,
            }, a => a.WithStrictOrdering());
    }


    [Test]
    public void GetEnumeratorShouldReturnEnumerator()
    {
        TestNode parent = new();
        TestNode child1 = new();
        parent.AppendChild(child1);
        TestNode child2 = new();
        parent.AppendChild(child2);

        List<Object?> results = new();
        IEnumerator enumerator = ((IEnumerable)parent).GetEnumerator();
        while (enumerator.MoveNext())
        {
            results.Add(enumerator.Current);
        }

        results.Should().BeEquivalentTo(new[] { parent, child1, child2 },
            a => a.WithStrictOrdering());
    }


    // Support
    //----------------------------------------------------------------------------------------------
    
    public class TestNode : TreeNode<TestNode>
    {
        public TestNode()
        {
            
        }
        
        public TestNode(String name)
        {
            Name = name;
        }


        public String Name { get; } = "";

        public override String ToString()
        {
            return Name;
        }
    }


    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class TestNodeWithOverrides : TreeNode<TestNodeWithOverrides>
    {
        protected override void OnChildAdded(TestNodeWithOverrides child)
        {
            HandleOnChildAdded(child);
        }
        
        protected override void OnChildRemoved(TestNodeWithOverrides child)
        {
            HandleOnChildRemoved(child);
        }


        public virtual void HandleOnChildAdded(TestNodeWithOverrides child)
        {
            
        }
        
        public virtual void HandleOnChildRemoved(TestNodeWithOverrides child)
        {
            
        }
    }
}
