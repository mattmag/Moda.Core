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
public class CompositeLengthFixture<T> where T : CompositeLength
{
    private Func<ILength, ILength, T> factory;
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
        Mock<ILength> lengthA = new();
        Mock<ILength> lengthB = new();
        ILength uut = this.factory(lengthA.Object, lengthB.Object);
        using IMonitor<ILength> monitor = uut.Monitor();
        
        lengthA.Raise(a => a.ValueInvalidated += null, lengthA.Object);
        
        monitor.Should().Raise(nameof(ILength.ValueInvalidated))
            .WithSender(uut);
    }
    
   

    [Test]
    public void PrerequisitesShouldBeAmalgamationOfComponentsPrerequisites()
    {
        Coordinate prereqA1 = new(Mock.Of<ICalculation>());
        Mock<ILength> lengthA = new();
        lengthA.Setup(a => a.Prerequisites).Returns(new[] { prereqA1 });
            
        Coordinate prereqB1 = new(Mock.Of<ICalculation>());
        Coordinate prereqB2 = new(Mock.Of<ICalculation>());
        Mock<ILength> lengthB = new();
        lengthB.Setup(a => a.Prerequisites).Returns(new[] { prereqB1, prereqB2 });

        ILength uut = this.factory(lengthA.Object, lengthB.Object);
        uut.Prerequisites.Should().BeEquivalentTo(new[] { prereqA1, prereqB1, prereqB2 });
    }
    
    [Test]
    public void PrerequisitesShouldBeDistinct()
    {
        Coordinate prereqCommon = new(Mock.Of<ICalculation>());
        Coordinate prereqA1 = new(Mock.Of<ICalculation>());
        Mock<ILength> lengthA = new();
        lengthA.Setup(a => a.Prerequisites).Returns(new[] { prereqCommon, prereqA1 });
            
        Coordinate prereqB1 = new(Mock.Of<ICalculation>());
        Mock<ILength> lengthB = new();
        lengthB.Setup(a => a.Prerequisites).Returns(new[] { prereqCommon, prereqB1 });

        ILength uut = this.factory(lengthA.Object, lengthB.Object);
        uut.Prerequisites.Should().BeEquivalentTo(new[] { prereqA1, prereqB1, prereqCommon });
    }
    
    [Test]
    public void PrerequisitesChangedShouldBeForwardedWithArgs()
    {
        Coordinate prereqA = new(Mock.Of<ICalculation>());
        Coordinate prereqB = new(Mock.Of<ICalculation>());
        List<Coordinate> prereqsA = new() {  };
        Mock<ILength> lengthA = new();
        lengthA.Setup(a => a.Prerequisites).Returns(() => prereqsA);
        
        Mock<ILength> lengthB = new();
        List<Coordinate> prereqsB = new() { prereqB };
        lengthB.Setup(a => a.Prerequisites).Returns(() => prereqsB);
        
        ILength uut = this.factory(lengthA.Object, lengthB.Object);
        using IMonitor<ILength> monitor = uut.Monitor();
        
        prereqsA.Add(prereqA);
        lengthA.Raise(a => a.PrerequisitesChanged += null, lengthA.Object,
            new CollectionChangedArgs<Coordinate>(
                new[] { prereqA },
                Enumerable.Empty<Coordinate>())
            );
        
        monitor.Should().Raise(nameof(ILength.PrerequisitesChanged))
            .WithSender(uut)
            .WithAssertedArgs<CollectionChangedArgs<Coordinate>>(a =>
                {
                    a.ItemsAdded.Should().BeEquivalentTo(new[] { prereqA });
                    a.ItemsRemoved.Should().BeEmpty();
                });

        prereqsB.Remove(prereqB);
        lengthB.Raise(a => a.PrerequisitesChanged += null, lengthA.Object,
            new CollectionChangedArgs<Coordinate>(
                Enumerable.Empty<Coordinate>(),
                new[] { prereqB })
            );
        
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
        Coordinate prereqA1 = new(Mock.Of<ICalculation>());
        Coordinate prereqA2 = new(Mock.Of<ICalculation>());
        List<Coordinate> prereqsA = new() { prereqA1, prereqA2 };
        Mock<ILength> lengthA = new();
        lengthA.Setup(a => a.Prerequisites).Returns(() => prereqsA);
            
        Coordinate prereqB1 = new(Mock.Of<ICalculation>());
        Coordinate prereqB2 = new(Mock.Of<ICalculation>());
        List<Coordinate> prereqsB = new() { prereqB1 };
        Mock<ILength> lengthB = new();
        lengthB.Setup(a => a.Prerequisites).Returns(() => prereqsB);

        ILength uut = this.factory(lengthA.Object, lengthB.Object);

        prereqsA.Remove(prereqA2);
        lengthA.Raise(a => a.PrerequisitesChanged += null, lengthA.Object,
            new CollectionChangedArgs<Coordinate>(
                Enumerable.Empty<Coordinate>(), new [] { prereqA2 }));
        prereqsB.Add(prereqB2);
        lengthB.Raise(a => a.PrerequisitesChanged += null, lengthB.Object,
            new CollectionChangedArgs<Coordinate>(
                new [] { prereqB2 }, Enumerable.Empty<Coordinate>()));
        
        uut.Prerequisites.Should().BeEquivalentTo(new[] { prereqA1, prereqB1, prereqB2 });
    }


    [Test]
    public void PrerequisitesShouldAccountForChangesToCommonPrerequisites()
    {
        Coordinate commonPrereq = new(Mock.Of<ICalculation>());
        
        Coordinate prereqA1 = new(Mock.Of<ICalculation>());
        List<Coordinate> prereqsA = new() { commonPrereq, prereqA1 };
        Mock<ILength> lengthA = new();
        lengthA.Setup(a => a.Prerequisites).Returns(() => prereqsA);
            
        Coordinate prereqB1 = new(Mock.Of<ICalculation>());
        List<Coordinate> prereqsB = new() { prereqB1 };
        Mock<ILength> lengthB = new();
        lengthB.Setup(a => a.Prerequisites).Returns(() => prereqsB);

        ILength uut = this.factory(lengthA.Object, lengthB.Object);
        using IMonitor<ILength> monitor = uut.Monitor();
        
        prereqsB.Add(commonPrereq);
        lengthB.Raise(a => a.PrerequisitesChanged += null, lengthB.Object,
            new CollectionChangedArgs<Coordinate>(
                new [] { commonPrereq }, Enumerable.Empty<Coordinate>()));
        
        monitor.Should().NotRaise(nameof(ILength.PrerequisitesChanged));
        
        prereqsA.Remove(commonPrereq);
        lengthA.Raise(a => a.PrerequisitesChanged += null, lengthA.Object,
            new CollectionChangedArgs<Coordinate>(
                Enumerable.Empty<Coordinate>(), new [] { commonPrereq }));
        
        monitor.Should().NotRaise(nameof(ILength.PrerequisitesChanged));
    }
    
    
    [Test]
    public void PrerequisitesChangedShouldAccountForChangesToCommonPrerequisites()
    {
        Coordinate commonPrereq = new(Mock.Of<ICalculation>());
        
        Coordinate prereqA1 = new(Mock.Of<ICalculation>());
        List<Coordinate> prereqsA = new() { commonPrereq, prereqA1 };
        Mock<ILength> lengthA = new();
        lengthA.Setup(a => a.Prerequisites).Returns(() => prereqsA);
            
        Coordinate prereqB1 = new(Mock.Of<ICalculation>());
        List<Coordinate> prereqsB = new() { prereqB1 };
        Mock<ILength> lengthB = new();
        lengthB.Setup(a => a.Prerequisites).Returns(() => prereqsB);

        ILength uut = this.factory(lengthA.Object, lengthB.Object);
        
        prereqsB.Add(commonPrereq);
        lengthB.Raise(a => a.PrerequisitesChanged += null, lengthB.Object,
            new CollectionChangedArgs<Coordinate>(
                new [] { commonPrereq }, Enumerable.Empty<Coordinate>()));
        
        uut.Prerequisites.Should().BeEquivalentTo(new[] { commonPrereq, prereqA1, prereqB1 });
        
        prereqsA.Remove(commonPrereq);
        lengthA.Raise(a => a.PrerequisitesChanged += null, lengthA.Object,
            new CollectionChangedArgs<Coordinate>(
                Enumerable.Empty<Coordinate>(), new [] { commonPrereq }));
        
        uut.Prerequisites.Should().BeEquivalentTo(new[] { commonPrereq, prereqA1, prereqB1 });
    }
}
