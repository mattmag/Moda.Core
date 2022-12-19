// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Events;
using Moda.Core.Tests.Support;
using Moda.Core.UI;
using Moda.Core.UI.Lengths;
using Moda.Core.Utility.Data;
using Moq;
using NUnit.Framework;

namespace Moda.Core.Tests.UI.Lengths;


[TestFixture(typeof(Sum), nameof(Sum.Add))]
[TestFixture(typeof(Sum), nameof(Sum.Subtract))]
[TestFixture(typeof(Product), nameof(Product.Multiply))]
[TestFixture(typeof(Product), nameof(Product.Divide))]
public class CompositeLengthFixture<T> where T : CompositeLength
{
    private readonly Func<ILength, ILength, T> factory;
    
    
    public CompositeLengthFixture(String nameOfFactoryMethod)
    {
        this.factory = (typeof(T).GetMethod(nameOfFactoryMethod,
                    new[] { typeof(ILength), typeof(ILength) })
                ?? throw new MissingMethodException())
            .CreateDelegate<Func<ILength, ILength, T>>();
    }
    
        
    [Test]
    public void ValueInvalidatedShouldForwarded()
    {
        MockLength lengthA = new();
        Mock<ILength> lengthB = new();
        ILength uut = this.factory(lengthA, lengthB.Object);
        using IMonitor<ILength> monitor = uut.Monitor();
        
        lengthA.RaiseInvalidate();
        
        monitor.Should().Raise(nameof(ILength.ValueInvalidated))
            .WithSender(uut);
    }
    

    [Test]
    public void PrerequisitesShouldBeAmalgamationOfComponentsPrerequisites()
    {
        Coordinate prereqA1 = new(GetMockedCell(), Axis.X, Mock.Of<ICalculation>());
        MockLength lengthA = new(new[] { prereqA1 });
            
        Coordinate prereqB1 = new(GetMockedCell(), Axis.X, Mock.Of<ICalculation>());
        Coordinate prereqB2 = new(GetMockedCell(), Axis.X, Mock.Of<ICalculation>());
        MockLength lengthB = new(new[] { prereqB1, prereqB2 });

        ILength uut = this.factory(lengthA, lengthB);
        uut.Prerequisites.Should().BeEquivalentTo(new[] { prereqA1, prereqB1, prereqB2 });
    }
    
    
    [Test]
    public void PrerequisitesShouldBeDistinct()
    {
        Coordinate prereqCommon = new(GetMockedCell(), Axis.X, Mock.Of<ICalculation>());
        Coordinate prereqA1 = new(GetMockedCell(), Axis.X, Mock.Of<ICalculation>());
        MockLength lengthA = new(new[] { prereqCommon, prereqA1 });
            
        Coordinate prereqB1 = new(GetMockedCell(), Axis.X, Mock.Of<ICalculation>());
        MockLength lengthB = new(new[] { prereqCommon, prereqB1 });

        ILength uut = this.factory(lengthA, lengthB);
        uut.Prerequisites.Should().BeEquivalentTo(new[] { prereqA1, prereqB1, prereqCommon });
    }
    
    
    [Test]
    public void PrerequisitesChangedShouldBeForwardedWithArgs()
    {
        Coordinate prereqA = new(GetMockedCell(), Axis.X, Mock.Of<ICalculation>());
        Coordinate prereqB = new(GetMockedCell(), Axis.X, Mock.Of<ICalculation>());

        MockLength lengthA = new();
        MockLength lengthB = new(new [] { prereqB });
        
        ILength uut = this.factory(lengthA, lengthB);
        using IMonitor<ILength> monitor = uut.Monitor();
        
        lengthA.AddPrerequisite(prereqA);
        lengthA.RaisePrerequisitesChanged(new[] { prereqA }, Enumerable.Empty<Coordinate>());
        
        monitor.Should().Raise(nameof(ILength.PrerequisitesChanged))
            .WithSender(uut)
            .WithAssertedArgs<CollectionChangedArgs<Coordinate>>(a =>
                {
                    a.ItemsAdded.Should().BeEquivalentTo(new[] { prereqA });
                    a.ItemsRemoved.Should().BeEmpty();
                });

        lengthB.RemovePrerequisite(prereqB);
        lengthB.RaisePrerequisitesChanged(Enumerable.Empty<Coordinate>(), new[] { prereqB });
        
        monitor.Should().Raise(nameof(ILength.PrerequisitesChanged))
            .WithSender(uut)
            .WithAssertedArgs<CollectionChangedArgs<Coordinate>>(a =>
                {
                    a.ItemsAdded.Should().BeEmpty();
                    a.ItemsRemoved.Should().BeEquivalentTo(new[] { prereqB });
                });
    }
    
    
    [Test]
    public void PrerequisitesShouldReflectComponentPrerequisiteChanged()
    {
        Coordinate prereqA1 = new(GetMockedCell(), Axis.X, Mock.Of<ICalculation>());
        Coordinate prereqA2 = new(GetMockedCell(), Axis.X, Mock.Of<ICalculation>());
        MockLength lengthA = new(new[] { prereqA1, prereqA2 });
            
        Coordinate prereqB1 = new(GetMockedCell(), Axis.X, Mock.Of<ICalculation>());
        Coordinate prereqB2 = new(GetMockedCell(), Axis.X, Mock.Of<ICalculation>());
        MockLength lengthB = new(new [] { prereqB1 });

        ILength uut = this.factory(lengthA, lengthB);

        lengthA.RemovePrerequisite(prereqA2);
        lengthA.RaisePrerequisitesChanged(Enumerable.Empty<Coordinate>(), new [] { prereqA2 });
        lengthB.AddPrerequisite(prereqB2);
        lengthB.RaisePrerequisitesChanged(new [] { prereqB2 }, Enumerable.Empty<Coordinate>());
        
        uut.Prerequisites.Should().BeEquivalentTo(new[] { prereqA1, prereqB1, prereqB2 });
    }


    [Test]
    public void PrerequisitesShouldAccountForChangesToCommonPrerequisites()
    {
        Coordinate commonPrereq = new(GetMockedCell(), Axis.X, Mock.Of<ICalculation>());
        
        Coordinate prereqA1 = new(GetMockedCell(), Axis.X, Mock.Of<ICalculation>());
        MockLength lengthA = new(new[] { commonPrereq, prereqA1 });
            
        Coordinate prereqB1 = new(GetMockedCell(), Axis.X, Mock.Of<ICalculation>());
        MockLength lengthB = new(new[] { prereqB1 });

        ILength uut = this.factory(lengthA, lengthB);
        using IMonitor<ILength> monitor = uut.Monitor();
        
        lengthB.AddPrerequisite(commonPrereq);
        lengthB.RaisePrerequisitesChanged(new[] { commonPrereq }, Enumerable.Empty<Coordinate>());
        
        monitor.Should().NotRaise(nameof(ILength.PrerequisitesChanged));
        
        lengthA.RemovePrerequisite(commonPrereq);
        lengthA.RaisePrerequisitesChanged(Enumerable.Empty<Coordinate>(), new [] { commonPrereq });
        
        monitor.Should().NotRaise(nameof(ILength.PrerequisitesChanged));
    }
    
    
    [Test]
    public void PrerequisitesChangedShouldAccountForChangesToCommonPrerequisites()
    {
        Coordinate commonPrereq = new(GetMockedCell(), Axis.X, Mock.Of<ICalculation>());
        
        Coordinate prereqA1 = new(GetMockedCell(), Axis.X, Mock.Of<ICalculation>());
        MockLength lengthA = new(new[] { prereqA1 });
            
        Coordinate prereqB1 = new(GetMockedCell(), Axis.X, Mock.Of<ICalculation>());
        MockLength lengthB = new(new[] { prereqB1 });

        ILength uut = this.factory(lengthA, lengthB);
        
        lengthB.AddPrerequisite(commonPrereq);
        lengthB.RaisePrerequisitesChanged(new [] { commonPrereq }, Enumerable.Empty<Coordinate>());
        
        uut.Prerequisites.Should().BeEquivalentTo(new[] { commonPrereq, prereqA1, prereqB1 });
        
        lengthA.RemovePrerequisite(commonPrereq);
        lengthA.RaisePrerequisitesChanged(Enumerable.Empty<Coordinate>(), new [] { commonPrereq });
        
        uut.Prerequisites.Should().BeEquivalentTo(new[] { commonPrereq, prereqA1, prereqB1 });
    }
    

    
    // Support
    //----------------------------------------------------------------------------------------------
    private static Cell GetMockedCell() => new(Mock.Of<IHoneyComb>(), Mock.Of<ICalculation>(),
        Mock.Of<ICalculation>(), Mock.Of<ICalculation>(), Mock.Of<ICalculation>());

    public class MockLength : Length
    {
        public MockLength()
        {
            
        }
        
        public MockLength(IEnumerable<Coordinate> prereqs)
        {
            ModifyPrerequisites(prereqs, Enumerable.Empty<Coordinate>());
        }


        public override Single Calculate()
        {
            return 0;
        }


        public void RaiseInvalidate()
        {
            RaiseValueInvalidated();
        }


        public void AddPrerequisite(Coordinate coordinate)
        {
            ModifyPrerequisites(new[] { coordinate }, Enumerable.Empty<Coordinate>());
        }
        
        public void RemovePrerequisite(Coordinate coordinate)
        {
            ModifyPrerequisites(Enumerable.Empty<Coordinate>(), new[] { coordinate });
        }

        public void RaisePrerequisitesChanged(IEnumerable<Coordinate> added,
            IEnumerable<Coordinate> removed)
        {
            RaisePrerequistesChanged(added, removed);
        }
    }
}
