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
    private Func<Length, Length, T> factory;
    public CompositeLengthFixture(String nameOfFactoryMethod)
    {
        this.factory = (typeof(T).GetMethod(nameOfFactoryMethod,
                    new[] { typeof(Length), typeof(Length) })
                ?? throw new MissingMethodException())
            .CreateDelegate<Func<Length, Length, T>>();
    }
        
    [Test]
    public void ValueInvalidatedShouldForwarded()
    {
        MockLength lengthA = new();
        Mock<Length> lengthB = new();
        Length uut = this.factory(lengthA, lengthB.Object);
        using IMonitor<Length> monitor = uut.Monitor();
        
        lengthA.RaiseInvalidate();
        
        monitor.Should().Raise(nameof(Length.ValueInvalidated))
            .WithSender(uut);
    }
    

    [Test]
    public void PrerequisitesShouldBeAmalgamationOfComponentsPrerequisites()
    {
        Coordinate prereqA1 = new(Mock.Of<ICalculation>());
        Mock<Length> lengthA = new();
        lengthA.Setup(a => a.Prerequisites).Returns(new[] { prereqA1 });
            
        Coordinate prereqB1 = new(Mock.Of<ICalculation>());
        Coordinate prereqB2 = new(Mock.Of<ICalculation>());
        Mock<Length> lengthB = new();
        lengthB.Setup(a => a.Prerequisites).Returns(new[] { prereqB1, prereqB2 });

        Length uut = this.factory(lengthA.Object, lengthB.Object);
        uut.Prerequisites.Should().BeEquivalentTo(new[] { prereqA1, prereqB1, prereqB2 });
    }
    
    
    [Test]
    public void PrerequisitesShouldBeDistinct()
    {
        Coordinate prereqCommon = new(Mock.Of<ICalculation>());
        Coordinate prereqA1 = new(Mock.Of<ICalculation>());
        Mock<Length> lengthA = new();
        lengthA.Setup(a => a.Prerequisites).Returns(new[] { prereqCommon, prereqA1 });
            
        Coordinate prereqB1 = new(Mock.Of<ICalculation>());
        Mock<Length> lengthB = new();
        lengthB.Setup(a => a.Prerequisites).Returns(new[] { prereqCommon, prereqB1 });

        Length uut = this.factory(lengthA.Object, lengthB.Object);
        uut.Prerequisites.Should().BeEquivalentTo(new[] { prereqA1, prereqB1, prereqCommon });
    }
    
    
    [Test]
    public void PrerequisitesChangedShouldBeForwardedWithArgs()
    {
        Coordinate prereqA = new(Mock.Of<ICalculation>());
        Coordinate prereqB = new(Mock.Of<ICalculation>());

        MockLength lengthA = new();
        MockLength lengthB = new(new [] { prereqB });
        
        Length uut = this.factory(lengthA, lengthB);
        using IMonitor<Length> monitor = uut.Monitor();
        
        lengthA.PrerequisitesList.Add(prereqA);
        lengthA.RaisePrerequisitesChanged(new[] { prereqA }, Enumerable.Empty<Coordinate>());
        
        monitor.Should().Raise(nameof(Length.PrerequisitesChanged))
            .WithSender(uut)
            .WithAssertedArgs<CollectionChangedArgs<Coordinate>>(a =>
                {
                    a.ItemsAdded.Should().BeEquivalentTo(new[] { prereqA });
                    a.ItemsRemoved.Should().BeEmpty();
                });

        lengthB.PrerequisitesList.Remove(prereqB);
        lengthB.RaisePrerequisitesChanged(Enumerable.Empty<Coordinate>(), new[] { prereqB });
        
        monitor.Should().Raise(nameof(Length.PrerequisitesChanged))
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
        Coordinate prereqA1 = new(Mock.Of<ICalculation>());
        Coordinate prereqA2 = new(Mock.Of<ICalculation>());
        MockLength lengthA = new(new[] { prereqA1, prereqA2 });
            
        Coordinate prereqB1 = new(Mock.Of<ICalculation>());
        Coordinate prereqB2 = new(Mock.Of<ICalculation>());
        MockLength lengthB = new(new [] { prereqB1 });

        Length uut = this.factory(lengthA, lengthB);

        lengthA.PrerequisitesList.Remove(prereqA2);
        lengthA.RaisePrerequisitesChanged(Enumerable.Empty<Coordinate>(), new [] { prereqA2 });
        lengthB.PrerequisitesList.Add(prereqB2);
        lengthB.RaisePrerequisitesChanged(new [] { prereqB2 }, Enumerable.Empty<Coordinate>());
        
        uut.Prerequisites.Should().BeEquivalentTo(new[] { prereqA1, prereqB1, prereqB2 });
    }


    [Test]
    public void PrerequisitesShouldAccountForChangesToCommonPrerequisites()
    {
        Coordinate commonPrereq = new(Mock.Of<ICalculation>());
        
        Coordinate prereqA1 = new(Mock.Of<ICalculation>());
        MockLength lengthA = new(new[] { commonPrereq, prereqA1 });
            
        Coordinate prereqB1 = new(Mock.Of<ICalculation>());
        MockLength lengthB = new(new[] { prereqB1 });

        Length uut = this.factory(lengthA, lengthB);
        using IMonitor<Length> monitor = uut.Monitor();
        
        lengthB.PrerequisitesList.Add(commonPrereq);
        lengthB.RaisePrerequisitesChanged(new[] { commonPrereq }, Enumerable.Empty<Coordinate>());
        
        monitor.Should().NotRaise(nameof(Length.PrerequisitesChanged));
        
        lengthA.PrerequisitesList.Remove(commonPrereq);
        lengthA.RaisePrerequisitesChanged(Enumerable.Empty<Coordinate>(), new [] { commonPrereq });
        
        monitor.Should().NotRaise(nameof(Length.PrerequisitesChanged));
    }
    
    
    [Test]
    public void PrerequisitesChangedShouldAccountForChangesToCommonPrerequisites()
    {
        Coordinate commonPrereq = new(Mock.Of<ICalculation>());
        
        Coordinate prereqA1 = new(Mock.Of<ICalculation>());
        MockLength lengthA = new(new[] { prereqA1 });
            
        Coordinate prereqB1 = new(Mock.Of<ICalculation>());
        MockLength lengthB = new(new[] { prereqB1 });

        Length uut = this.factory(lengthA, lengthB);
        
        lengthB.PrerequisitesList.Add(commonPrereq);
        lengthB.RaisePrerequisitesChanged(new [] { commonPrereq }, Enumerable.Empty<Coordinate>());
        
        uut.Prerequisites.Should().BeEquivalentTo(new[] { commonPrereq, prereqA1, prereqB1 });
        
        lengthA.PrerequisitesList.Remove(commonPrereq);
        lengthA.RaisePrerequisitesChanged(Enumerable.Empty<Coordinate>(), new [] { commonPrereq });
        
        uut.Prerequisites.Should().BeEquivalentTo(new[] { commonPrereq, prereqA1, prereqB1 });
    }


    public class MockLength : Length
    {
        public MockLength()
        {
            
        }
        
        public MockLength(IEnumerable<Coordinate> prereqs)
        {
            this.PrerequisitesList.AddRange(prereqs);
        }


        public override Single Calculate()
        {
            return 0;
        }


        public void RaiseInvalidate()
        {
            RaiseValueInvalidated();
        }


        public readonly List<Coordinate> PrerequisitesList = new();
        public override IEnumerable<Coordinate> Prerequisites => this.PrerequisitesList;


        public void RaisePrerequisitesChanged(IEnumerable<Coordinate> added,
            IEnumerable<Coordinate> removed)
        {
            base.RaisePrerequistesChanged(added, removed);
        }
    }
}
