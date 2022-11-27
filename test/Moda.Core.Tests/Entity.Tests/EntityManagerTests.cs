// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/


using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using Moda.Core.Entity;
using Moda.Core.Tests.Entity.Tests.Support;
using Moq;
using NUnit.Framework;
using Optional;
using Optional.Unsafe;

namespace Moda.Core.Tests.Entity.Tests;

[TestFixture]
public class EntityManagerTests
{
    //  RegisterSystem() Tests
    //----------------------------------------------------------------------------------------------
        
    [Test]
    public void RegisterSystemWithEmptyActsOnShouldThrowException()
    {
        EntityManager unitUnderTest = new();
        Mock<IComponentSystem> system = new();
        system.SetupGet(a => a.ActsOn).Returns(ImmutableHashSet.Create<Type>());

        unitUnderTest.Invoking(a => a.RegisterSystem(system.Object))
            .Should().Throw<InvalidSystemException>();
    }
    
    [Test]
    public void RegisterSystemWithNullActsOnShouldThrowException()
    {
        EntityManager unitUnderTest = new();
        Mock<IComponentSystem> system = new();
        system.SetupGet(a => a.ActsOn).Returns((null as ImmutableHashSet<Type>)!);

        unitUnderTest.Invoking(a => a.RegisterSystem(system.Object))
            .Should().Throw<InvalidSystemException>();
    }
    
    [Test]
    public void RegisteringTheSameSystemTwiceShouldThrowAnException()
    {
        EntityManager unitUnderTest = new();
        Mock<IComponentSystem> system = new();
        system.SetupGet(a => a.ActsOn).Returns(ImmutableHashSet.Create(typeof(ComponentA)));
        
        unitUnderTest.RegisterSystem(system.Object);
        
        unitUnderTest.Invoking(a => a.RegisterSystem(system.Object))
            .Should().Throw<SystemAlreadyRegisteredException>();
    }
    
    [Test]
    public void RegisteringTwoDifferentSystemsOfTheSameTypeShouldNotThrowAnException()
    {
        EntityManager unitUnderTest = new();
        unitUnderTest.RegisterSystem(new SystemA());
        unitUnderTest.Invoking(a => a.RegisterSystem(new SystemA()))
            .Should().NotThrow();
    }
    
    [Test]
    public void RegisterSystemShouldStartReceivingEntities()
    {
        EntityManager unitUnderTest = new();
        Mock<IComponentSystem> system = new();
        system.SetupGet(a => a.ActsOn).Returns(ImmutableHashSet.Create(typeof(ComponentA)));

        unitUnderTest.RegisterSystem(system.Object);

        UInt64 id = unitUnderTest.AddEntity(new ComponentA());
        system.Verify(a => a.RegisterEntity(id), Times.Once);
    }

    [Test]
    public void NewlyRegisteredSystemShouldPickUpExistingEntities()
    {
        EntityManager unitUnderTest = new();
        Mock<IComponentSystem> system = new();
        system.SetupGet(a => a.ActsOn).Returns(ImmutableHashSet.Create(typeof(ComponentA)));

        UInt64 entityID1 = unitUnderTest.AddEntity(new ComponentA());
        UInt64 entityID2 = unitUnderTest.AddEntity(new ComponentA());

        unitUnderTest.RegisterSystem(system.Object);

        system.Verify(a => a.RegisterEntity(entityID1), Times.Once);
        system.Verify(a => a.RegisterEntity(entityID2), Times.Once);
    }
    
    [Test]
    public void NewlyRegisteredSystemShouldNotPickUpExistingIrrelevantEntities()
    {
        EntityManager unitUnderTest = new();
        Mock<IComponentSystem> system = new();
        system.SetupGet(a => a.ActsOn).Returns(ImmutableHashSet.Create(typeof(ComponentA)));

        UInt64 entityID1 = unitUnderTest.AddEntity(new ComponentB());
        UInt64 entityID2 = unitUnderTest.AddEntity(new ComponentB(), new ComponentC());

        unitUnderTest.RegisterSystem(system.Object);

        system.Verify(a => a.RegisterEntity(entityID1), Times.Never);
        system.Verify(a => a.RegisterEntity(entityID2), Times.Never);
    }
    
    [Test]
    public void NewlyRegisteredComplexSystemShouldPickUpExistingEntities()
    {
        EntityManager unitUnderTest = new();
        Mock<IComponentSystem> system = new();
        system.SetupGet(a => a.ActsOn).Returns(ImmutableHashSet.Create(
            typeof(ComponentA), typeof(ComponentB)));

        UInt64 entityID1 = unitUnderTest.AddEntity(new ComponentA(), new ComponentB());
        UInt64 entityID2 = unitUnderTest.AddEntity(new ComponentA(), new ComponentB(), new 
            ComponentC());

        unitUnderTest.RegisterSystem(system.Object);

        system.Verify(a => a.RegisterEntity(entityID1), Times.Once);
        system.Verify(a => a.RegisterEntity(entityID2), Times.Once);
    }
    
    //  UnregisterSystem() Tests
    //----------------------------------------------------------------------------------------------

    [Test]
    public void UnregisterSystemWithoutRegisteringShouldThrowException()
    {
        EntityManager unitUnderTest = new();
        Assert.Throws<SystemNotFoundException>(
            () => unitUnderTest.UnregisterSystem(Mock.Of<IComponentSystem>()));
    }
    
    [Test]
    public void UnregisterSystemShouldStopReceivingEntities()
    {
        EntityManager unitUnderTest = new();
        Mock<IComponentSystem> system = new();
        system.SetupGet(a => a.ActsOn).Returns(ImmutableHashSet.Create(typeof(ComponentA)));

        unitUnderTest.RegisterSystem(system.Object);
        unitUnderTest.AddEntity(new ComponentA());
        unitUnderTest.UnregisterSystem(system.Object);
        unitUnderTest.AddEntity(new ComponentA());

        system.Verify(a => a.RegisterEntity(It.IsAny<UInt64>()), Times.Once);
    }
    
    //  AddEntity() Tests
    //----------------------------------------------------------------------------------------------
    [Test]
    public void AddEntityWithNoComponentsShouldThrowException()
    {
        EntityManager unitUnderTest = new();

        unitUnderTest.Invoking(a => a.AddEntity(Enumerable.Empty<Object>()))
            .Should().Throw<EmptyEntityException>();
    }
    
    [Test]
    public void AddEntityShouldReturnUniqueID()
    {
        EntityManager unitUnderTest = new();
        HashSet<UInt64> ids = new();
        for (Int32 i = 0; i < 1000; i++)
        {
            UInt64 id = unitUnderTest.AddEntity(new Object());
            ids.Add(id).Should().BeTrue($"ID '{id}' should be unique");
        }
    }
    
    [Test]
    public void AddEntityWithDuplicateComponentTypesShouldThrowException()
    {
        EntityManager unitUnderTest = new();
        unitUnderTest.Invoking(a => a.AddEntity(new ComponentA(), new ComponentA()))
            .Should().Throw<DuplicateComponentException>();
    }
    
    [Test]
    public void AddEntityShouldNotCommitOnFailure()
    {
        EntityManager unitUnderTest = new();
        
        // ensure the exception is no uncaught so that we can check if the entity was added
        unitUnderTest
            .Invoking(a => a.AddEntity(new ComponentA(), new ComponentB(), new ComponentB()))
            .Should().Throw<DuplicateComponentException>();
        
        // assuming sequential ID, starting from 1
        unitUnderTest.Invoking(a => a.GetComposition(1))
            .Should().Throw<EntityNotFoundException>();
    }
    
    [Test]
    public void AddEntityShouldRegisterWithInterestedSystem()
    {
        AddEntity(new ComponentA())
            .ShouldRegisterWithSystemThatActOn(typeof(ComponentA))
            .Test();
    }
    
    [Test]
    public void AddEntityShouldRegisterWithAllInterestedSystem()
    {
        AddEntity(new ComponentA())
            .ShouldRegisterWithSystemThatActOn(typeof(ComponentA))
            .ShouldRegisterWithSystemThatActOn(typeof(ComponentA))
            .Test();
    }

    [Test]
    public void AddEntityShouldNotRegisterWithIrInterestedSystem()
    {
        AddEntity(new ComponentA())
            .ShouldNotAffectSystemsThatActOn(typeof(ComponentB))
            .Test();
    }

    [Test]
    public void AddEntityShouldNotRegisterWithUninterestedComplexSystem()
    {
        AddEntity(new ComponentA())
            .ShouldNotAffectSystemsThatActOn(typeof(ComponentA), typeof(ComponentC))
            .Test();
    }

    [Test]
    public void AddComplexEntityShouldRegisterWithInterestedSystem()
    {
        AddEntity(new ComponentA(), new ComponentB())
            .ShouldRegisterWithSystemThatActOn(typeof(ComponentA))
            .Test();
    }

    [Test]
    public void AddComplexEntityShouldRegisterWithInterestedComplexSystem()
    {
        AddEntity(new ComponentA(), new ComponentB())
            .ShouldRegisterWithSystemThatActOn(typeof(ComponentA), typeof(ComponentB))
            .Test();
    }

    [Test]
    public void AddComplexEntityShouldNotRegisterWithUninterestedComplexSystems()
    {
        AddEntity(new ComponentA(), new ComponentB())
            .ShouldNotAffectSystemsThatActOn(typeof(ComponentB), typeof(ComponentC))
            .ShouldNotAffectSystemsThatActOn(typeof(ComponentA), typeof(ComponentB),
                typeof(ComponentC))
            .Test();
    }

    [Test]
    public void AddSuperComplexEntityShouldRegisterWithAllInterestedComplexSystems()
    {
        AddEntity(new ComponentA(), new ComponentB(), new ComponentC())
            .ShouldRegisterWithSystemThatActOn(typeof(ComponentA), typeof(ComponentB))
            .ShouldRegisterWithSystemThatActOn(typeof(ComponentA), typeof(ComponentB),
                typeof(ComponentC))
            .Test();
    }

    //  AddComponents() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void AddComponentsWithInvalidEntityIDShouldThrowException()
    {
        EntityManager unitUnderTest = new();
        UInt64 entityID = 18;
        
        unitUnderTest.Invoking(a => a.AddComponents(entityID, new ComponentA()))
            .Should().Throw<EntityNotFoundException>();
    }

    [Test]
    public void AddComponentsWithDuplicatesShouldThrowException()
    {
        EntityManager unitUnderTest = new();
        UInt64 entityID = unitUnderTest.AddEntity(new ComponentA());
        
        unitUnderTest
            .Invoking(a => a.AddComponents(entityID, new ComponentB(), new ComponentB()))
            .Should().Throw<DuplicateComponentException>();
    }

    [Test]
    public void AddComponentsResultingInDuplicatesShouldThrowException()
    {
        EntityManager unitUnderTest = new();
        UInt64 entityID = unitUnderTest.AddEntity(new ComponentA());
        
        unitUnderTest.Invoking(a => a.AddComponents(entityID, new ComponentA()))
            .Should().Throw<DuplicateComponentException>();
    }

    [Test]
    public void AddComponentsShouldNotCommitOnFailure()
    {
        EntityManager unitUnderTest = new();
        UInt64 entityID = unitUnderTest.AddEntity(new ComponentA());

        unitUnderTest.Invoking(a => a.AddComponents(entityID, new ComponentB(), new ComponentC(),
                new ComponentC()))
            .Should().Throw<DuplicateComponentException>();

        ImmutableHashSet<Type> results = unitUnderTest.GetComposition(entityID);
        results.Should().NotContain(typeof(ComponentB));
        results.Should().NotContain(typeof(ComponentC));
    }

    [Test]
    public void AddComponentsShouldRegisterWithNewlyInterestedSystem()
    {
        AddEntity(new ComponentA())
            .Then()
            .AddComponents(new ComponentB())
                .ShouldRegisterWithSystemThatActOn(typeof(ComponentB))
                .Test();
    }


    [Test]
    public void AddComponentsShouldRegisterWithAllNewlyInterestedSystem()
    {
        AddEntity(new ComponentA())
            .Then()
            .AddComponents(new ComponentB())
                .ShouldRegisterWithSystemThatActOn(typeof(ComponentB))
                .ShouldRegisterWithSystemThatActOn(typeof(ComponentB))
                .Test();
    }


    [Test]
    public void AddComponentsShouldRegisterWithNewlyInterestedComplexSystem()
    {
        AddEntity(new ComponentA())
            .Then()
            .AddComponents(new ComponentB())
                .ShouldRegisterWithSystemThatActOn(typeof(ComponentA), typeof(ComponentB))
                .Test();
    }

    [Test]
    public void AddComponentsShouldRegisterWithAllNewlyInterestedComplexSystem()
    {
        AddEntity(new ComponentA(), new ComponentC())
            .Then()
            .AddComponents(new ComponentB())
                .ShouldRegisterWithSystemThatActOn(typeof(ComponentA), typeof(ComponentB))
                .ShouldRegisterWithSystemThatActOn(typeof(ComponentB), typeof(ComponentC))
                .Test();
    }

    [Test]
    public void AddComponentsShouldNotRegisterWithStillUninterestedSystem()
    {
        AddEntity(new ComponentA())
            .Then()
            .AddComponents(new ComponentB())
                .ShouldNotAffectSystemsThatActOn(typeof(ComponentC))
                .Test();
    }

    [Test]
    public void AddComponentsShouldNotRegisterWithStillUninterestedComplexSystem()
    {
        AddEntity(new ComponentA())
            .Then()
            .AddComponents(new ComponentB())
                .ShouldNotAffectSystemsThatActOn(typeof(ComponentB), typeof(ComponentC))
                .Test();
    }

    [Test]
    public void AddComponentsAgainShouldRegisterWithNewlyInterestedSystem()
    {
        AddEntity(new ComponentA())
            .Then()
            .AddComponents(new ComponentB())
            .Then()
            .AddComponents(new ComponentC())
                .ShouldRegisterWithSystemThatActOn(typeof(ComponentC))
                .Test();
    }

    [Test]
    public void AddComponentsAgainShouldRegisterWithNewlyInterestedComplexSystem()
    {
        AddEntity(new ComponentA())
            .Then()
            .AddComponents(new ComponentB())
            .Then()
            .AddComponents(new ComponentC())
                .ShouldRegisterWithSystemThatActOn(typeof(ComponentB), typeof(ComponentC))
                .Test();
    }


    //  GetComponent() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void GetComponentShouldReturnComponentFromAddEntity()
    {
        EntityManager unitUnderTest = new();
        ComponentA component = new();
        
        UInt64 id = unitUnderTest.AddEntity(component);
        
        unitUnderTest.GetComponent<ComponentA>(id).Should().BeSameAs(component);
    }

    [Test]
    public void GetComponentShouldReturnComponentFromAddComponent()
    {
        EntityManager unitUnderTest = new();
        ComponentB component = new();
        UInt64 id = unitUnderTest.AddEntity(new ComponentA());
        
        unitUnderTest.AddComponents(id, component);
        
        unitUnderTest.GetComponent<ComponentB>(id).Should().BeSameAs(component);
    }

    [Test]
    public void GetComponentShouldThrowExceptionIfComponentDoesNotExist()
    {
        EntityManager unitUnderTest = new();
        ComponentA component = new();
        
        UInt64 id = unitUnderTest.AddEntity(component);

        unitUnderTest.Invoking(a => a.GetComponent<ComponentB>(id))
            .Should().Throw<ComponentNotFoundException>();
    }

    [Test]
    public void GetComponentShouldThrowExceptionIfEntityDoesNotExist()
    {
        EntityManager unitUnderTest = new();
        Assert.Throws<EntityNotFoundException>(() => unitUnderTest.GetComponent<ComponentA>(18));
    }

    [Test]
    public void GetComponentShouldReturnComponentFromRequestedEntity()
    {
        EntityManager unitUnderTest = new();
        ComponentA componentAX = new();
        ComponentA componentAY = new();

        UInt64 idX = unitUnderTest.AddEntity(componentAX);
        UInt64 idY = unitUnderTest.AddEntity(componentAY);
        
        ComponentB componentBX = new();
        ComponentB componentBY = new();
        unitUnderTest.AddComponents(idX, componentBX);
        unitUnderTest.AddComponents(idY, componentBY);

        unitUnderTest.GetComponent<ComponentA>(idX).Should().BeSameAs(componentAX);
        unitUnderTest.GetComponent<ComponentB>(idX).Should().BeSameAs(componentBX);
        unitUnderTest.GetComponent<ComponentA>(idY).Should().BeSameAs(componentAY);
        unitUnderTest.GetComponent<ComponentB>(idY).Should().BeSameAs(componentBY);
    }

    [Test]
    public void GetComponentShouldThrowExceptionWhenComponentHasBeenRemoved()
    {
        EntityManager unitUnderTest = new();
        ComponentA componentA = new();
        ComponentB componentB = new();

        UInt64 entityID = unitUnderTest.AddEntity(componentA, componentB);
        unitUnderTest.RemoveComponents(entityID, typeof(ComponentB));

        unitUnderTest.Invoking(a => a.GetComponent<ComponentB>(entityID))
            .Should().Throw<ComponentNotFoundException>();
    }

    //  GetComponentOrNone() Tests
    //----------------------------------------------------------------------------------------------

    [Test]
    public void GetComponentOrNoneShouldReturnComponentFromAddedEntity()
    {
        EntityManager unitUnderTest = new();
        ComponentA component = new();
        
        UInt64 id = unitUnderTest.AddEntity(component);

        unitUnderTest.GetComponentOrNone<ComponentA>(id).ValueOrFailure()
            .Should().BeSameAs(component);
    }

    [Test]
    public void GetComponentOrNoneShouldReturnComponentFromAddedComponent()
    {
        EntityManager unitUnderTest = new();
        ComponentB component = new();
        
        UInt64 id = unitUnderTest.AddEntity(new ComponentA());
        unitUnderTest.AddComponents(id, component);
        
        unitUnderTest.GetComponentOrNone<ComponentB>(id).ValueOrFailure()
            .Should().BeSameAs(component);
    }

    [Test]
    public void GetComponentOrNoneShouldReturnNoneIfComponentDoesNotExist()
    {
        EntityManager unitUnderTest = new();
        ComponentA component = new();
        
        UInt64 id = unitUnderTest.AddEntity(component);

        unitUnderTest.GetComponentOrNone<ComponentB>(id)
            .Should().Be(Option.None<ComponentB>());
    }

    [Test]
    public void GetComponentOrNoneShouldThrowExceptionIfEntityDoesNotExist()
    {
        EntityManager unitUnderTest = new();
        unitUnderTest.Invoking(a => a.GetComponentOrNone<ComponentA>(18))
            .Should().Throw<EntityNotFoundException>();
    }

    [Test]
    public void GetComponentOrNoneShouldReturnComponentFromRequestedEntity()
    {
        EntityManager unitUnderTest = new();
        ComponentA componentAX = new();
        ComponentA componentAY = new();
        
        UInt64 idX = unitUnderTest.AddEntity(componentAX);
        UInt64 idY = unitUnderTest.AddEntity(componentAY);
        
        ComponentB componentBX = new();
        ComponentB componentBY = new();
        unitUnderTest.AddComponents(idX, componentBX);
        unitUnderTest.AddComponents(idY, componentBY);

        unitUnderTest.GetComponentOrNone<ComponentA>(idX).ValueOrFailure()
            .Should().BeSameAs(componentAX);
        unitUnderTest.GetComponentOrNone<ComponentB>(idX).ValueOrFailure()
            .Should().BeSameAs(componentBX);
        unitUnderTest.GetComponentOrNone<ComponentA>(idY).ValueOrFailure()
            .Should().BeSameAs(componentAY);
        unitUnderTest.GetComponentOrNone<ComponentB>(idY).ValueOrFailure()
            .Should().BeSameAs(componentBY);
    }

    [Test]
    public void GetComponentOrNoneShouldReturnNoneWhenComponentHasBeenRemoved()
    {
        EntityManager unitUnderTest = new();
        ComponentA componentA = new();
        ComponentB componentB = new();

        UInt64 entityID = unitUnderTest.AddEntity(componentA, componentB);
        unitUnderTest.RemoveComponents(entityID, typeof(ComponentB));

        unitUnderTest.GetComponentOrNone<ComponentB>(entityID)
            .Should().Be(Option.None<ComponentB>());
    }

    //  GetComponents() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void GetComponentsShouldReturnAllComponentsOfSpecifiedTypes()
    {
        EntityManager unitUnderTest = new();
        ComponentA componentA = new();
        ComponentB componentB = new();

        UInt64 entityID = unitUnderTest.AddEntity(componentA, componentB, new ComponentC());
        ComponentCollection results = unitUnderTest.GetComponents(entityID,
            typeof(ComponentA), typeof(ComponentB));

        results.GetComponent<ComponentA>().Should().BeSameAs(componentA);
        results.GetComponent<ComponentB>().Should().BeSameAs(componentB);
    }


    [Test]
    public void GetComponentsShouldReturnAddedComponents()
    {
        EntityManager unitUnderTest = new();
        ComponentA componentA = new();
        ComponentB componentB = new();

        UInt64 entityID = unitUnderTest.AddEntity(componentA);
        unitUnderTest.AddComponents(entityID, componentB);
        ComponentCollection results = unitUnderTest.GetComponents(entityID,
            typeof(ComponentA), typeof(ComponentB));

        results.GetComponent<ComponentA>().Should().BeSameAs(componentA);
        results.GetComponent<ComponentB>().Should().BeSameAs(componentB);
    }

    [Test]
    public void GetComponentsShouldReturnComponentsFromRequestedEntity()
    {
        EntityManager unitUnderTest = new();
        ComponentA componentAX = new();
        ComponentA componentAY = new();

        UInt64 idX = unitUnderTest.AddEntity(componentAX);
        UInt64 idY = unitUnderTest.AddEntity(componentAY);
        
        ComponentB componentBX = new();
        ComponentB componentBY = new();
        unitUnderTest.AddComponents(idX, componentBX);
        unitUnderTest.AddComponents(idY, componentBY);
        
        ComponentCollection resultsX = unitUnderTest.GetComponents(idX,
            typeof(ComponentA), typeof(ComponentB));
        ComponentCollection resultsY = unitUnderTest.GetComponents(idY,
           typeof(ComponentA), typeof(ComponentB));

        resultsX.GetComponent<ComponentA>().Should().BeSameAs(componentAX);
        resultsX.GetComponent<ComponentB>().Should().BeSameAs(componentBX);
        resultsY.GetComponent<ComponentA>().Should().BeSameAs(componentAY);
        resultsY.GetComponent<ComponentB>().Should().BeSameAs(componentBY);
    }


    [Test]
    public void GetComponentsShouldThrowAnExceptionWhenComponentIsMissing()
    {
        EntityManager unitUnderTest = new();
        ComponentA componentA = new();
        ComponentB componentB = new();

        UInt64 entityID = unitUnderTest.AddEntity(componentA, componentB);

        unitUnderTest.Invoking(a => a.GetComponents(entityID, typeof(ComponentA),
            typeof(ComponentB), typeof(ComponentC)))
            .Should().Throw<ComponentNotFoundException>();
    }

    [Test]
    public void GetComponentsShouldThrowExceptionIfEntityDoesNotExist()
    {
        EntityManager unitUnderTest = new();
        
        unitUnderTest.Invoking(a => a.GetComponents(18, typeof(ComponentA)))
            .Should().Throw<EntityNotFoundException>();
    }

    [Test]
    public void GetComponentsShouldNotIncludeRemovedComponents()
    {
        EntityManager unitUnderTest = new();
        ComponentA componentA = new();
        ComponentB componentB = new();

        UInt64 entityID = unitUnderTest.AddEntity(componentA, componentB);
        unitUnderTest.RemoveComponents(entityID, typeof(ComponentB));

        unitUnderTest.Invoking(a => a.GetComponents(entityID, typeof(ComponentB)))
            .Should().Throw<ComponentNotFoundException>();
    }


    //  GetAllComponents() Tests
    //----------------------------------------------------------------------------------------------

    [Test]
    public void GetAllComponentsShouldReturnAllComponents()
    {
        EntityManager unitUnderTest = new();
        ComponentA componentA = new();
        ComponentB componentB = new();
        ComponentC componentC = new();

        UInt64 entityID = unitUnderTest.AddEntity(componentA, componentB, componentC);

        ComponentCollection results = unitUnderTest.GetAllComponents(entityID);
        results.GetComponent<ComponentA>().Should().BeSameAs(componentA);
        results.GetComponent<ComponentB>().Should().BeSameAs(componentB);
        results.GetComponent<ComponentC>().Should().BeSameAs(componentC);
    }

    [Test]
    public void GetAllComponentsShouldIncludeAddedComponents()
    {
        EntityManager unitUnderTest = new();
        ComponentA componentA = new();

        UInt64 entityID = unitUnderTest.AddEntity(componentA);

        ComponentB componentB = new();
        ComponentC componentC = new();

        unitUnderTest.AddComponents(entityID, componentB);
        unitUnderTest.AddComponents(entityID, componentC);

        ComponentCollection results = unitUnderTest.GetAllComponents(entityID);
        results.GetComponent<ComponentA>().Should().BeSameAs(componentA);
        results.GetComponent<ComponentB>().Should().BeSameAs(componentB);
        results.GetComponent<ComponentC>().Should().BeSameAs(componentC);
    }
    
    [Test]
    public void GetAllComponentsShouldReturnComponentsFromRequestedEntity()
    {
        EntityManager unitUnderTest = new();
        ComponentA componentAX = new();
        ComponentB componentBX = new();
        ComponentA componentAY = new();
        ComponentB componentBY = new();

        UInt64 idX = unitUnderTest.AddEntity(componentAX, componentBX);
        UInt64 idY = unitUnderTest.AddEntity(componentAY, componentBY);

        ComponentC componentCX = new();
        ComponentC componentCY = new();
        unitUnderTest.AddComponents(idX, componentCX);
        unitUnderTest.AddComponents(idY, componentCY);

        ComponentCollection resultsX = unitUnderTest.GetAllComponents(idX);
        ComponentCollection resultsY = unitUnderTest.GetAllComponents(idY);

        resultsX.GetComponent<ComponentA>().Should().BeSameAs(componentAX);
        resultsX.GetComponent<ComponentB>().Should().BeSameAs(componentBX);
        resultsX.GetComponent<ComponentC>().Should().BeSameAs(componentCX);
        
        resultsY.GetComponent<ComponentA>().Should().BeSameAs(componentAY);
        resultsY.GetComponent<ComponentB>().Should().BeSameAs(componentBY);
        resultsY.GetComponent<ComponentC>().Should().BeSameAs(componentCY);
    }

    [Test]
    public void GetAllComponentsShouldThrowExceptionIfEntityDoesNotExist()
    {
        EntityManager unitUnderTest = new();
        unitUnderTest.Invoking(a => a.GetAllComponents(18))
            .Should().Throw<EntityNotFoundException>();
    }

    [Test]
    public void GetAllComponentsShouldNotIncludeRemovedComponents()
    {
        EntityManager unitUnderTest = new();
        ComponentA componentA = new();
        ComponentB componentB = new();

        UInt64 entityID = unitUnderTest.AddEntity(componentA, componentB);
        unitUnderTest.RemoveComponents(entityID, typeof(ComponentB));

        ComponentCollection results = unitUnderTest.GetAllComponents(entityID);

        results.Invoking(a => a.GetComponent<ComponentB>())
            .Should().Throw<ComponentNotFoundException>();
    }


    //  GetCompositionTests()
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void GetCompositionShouldReturnTypesForAllComponents()
    {
        EntityManager unitUnderTest = new();

        UInt64 entityID = unitUnderTest.AddEntity(new ComponentA(), new ComponentB(),
            new ComponentC());

        ImmutableHashSet<Type> results = unitUnderTest.GetComposition(entityID);

        results.Should().Contain(typeof(ComponentA));
        results.Should().Contain(typeof(ComponentB));
        results.Should().Contain(typeof(ComponentC));
    }

    [Test]
    public void GetCompositionShouldIncludeAddedComponents()
    {
        EntityManager unitUnderTest = new();

        UInt64 entityID = unitUnderTest.AddEntity(new ComponentA());

        unitUnderTest.AddComponents(entityID, new ComponentB());
        unitUnderTest.AddComponents(entityID, new ComponentC());

        ImmutableHashSet<Type> results = unitUnderTest.GetComposition(entityID);

        results.Should().Contain(typeof(ComponentA));
        results.Should().Contain(typeof(ComponentB));
        results.Should().Contain(typeof(ComponentC));
    }
    
    [Test]
    public void GetCompositionShouldReturnTypesFromRequestedEntity()
    {
        EntityManager unitUnderTest = new();

        UInt64 idX = unitUnderTest.AddEntity(new ComponentA());
        UInt64 idY = unitUnderTest.AddEntity(new ComponentB());

        unitUnderTest.AddComponents(idX, new ComponentB());
        unitUnderTest.AddComponents(idY, new ComponentC());

        ImmutableHashSet<Type> resultsX = unitUnderTest.GetComposition(idX);
        ImmutableHashSet<Type> resultsY = unitUnderTest.GetComposition(idY);

        resultsX.Should().Contain(typeof(ComponentA));
        resultsX.Should().Contain(typeof(ComponentB));
        resultsX.Should().NotContain(typeof(ComponentC));

        resultsY.Should().Contain(typeof(ComponentB));
        resultsY.Should().Contain(typeof(ComponentC));
        resultsY.Should().NotContain(typeof(ComponentA));
    }

    [Test]
    public void GetCompositionShouldThrowExceptionIfEntityDoesNotExist()
    {
        EntityManager unitUnderTest = new();
        Assert.Throws<EntityNotFoundException>(() => unitUnderTest.GetComposition(18));
    }

    [Test]
    public void GetCompositionShouldNotIncludeRemovedComponents()
    {
        EntityManager unitUnderTest = new();
        UInt64 entityID = unitUnderTest.AddEntity(new ComponentA(), new ComponentB());
        
        unitUnderTest.RemoveComponents(entityID, typeof(ComponentB));
        
        ImmutableHashSet<Type> composition = unitUnderTest.GetComposition(entityID);

        composition.Should().NotContain(typeof(ComponentB));
    }


    //  RemoveComponents() Tests
    //----------------------------------------------------------------------------------------------

    [Test]
    public void RemoveComponentsWithInvalidEntityIDShouldThrowException()
    {
        EntityManager unitUnderTest = new();

        unitUnderTest.Invoking(a => a.RemoveComponents(18, typeof(ComponentA)))
            .Should().Throw<EntityNotFoundException>();
    }

    [Test]
    public void RemoveComponentsShouldThrowExceptionWhenComponentDoesNotExist()
    {
        EntityManager unitUnderTest = new();
        UInt64 entityID = unitUnderTest.AddEntity(new ComponentA(), new ComponentB());

        unitUnderTest.Invoking(a => a.RemoveComponents(entityID, typeof(ComponentC)))
            .Should().Throw<ComponentNotFoundException>();
    }

    [Test]
    public void RemoveComponentsShouldNotCommitOnFailure()
    {
        EntityManager unitUnderTest = new();
        UInt64 entityID = unitUnderTest.AddEntity(new ComponentA(), new ComponentB());

        unitUnderTest
            .Invoking(a => a.RemoveComponents(entityID, typeof(ComponentB), typeof(ComponentC)))
            .Should().Throw<ComponentNotFoundException>();

        ImmutableHashSet<Type> results = unitUnderTest.GetComposition(entityID);
        results.Should().Contain(typeof(ComponentB));
    }

    [Test]
    public void RemoveLastComponentShouldCompletelyRemoveEtntity()
    {
        EntityManager unitUnderTest = new();
        UInt64 entityID = unitUnderTest.AddEntity(new ComponentA(), new ComponentB());

        unitUnderTest.RemoveComponents(entityID, typeof(ComponentA), typeof(ComponentB));

        Assert.Throws<EntityNotFoundException>(() => unitUnderTest.GetComposition(entityID));
    }

    [Test]
    public void RemoveComponentsShouldUnregisterWithNewlyUninterestedSystem()
    {
        AddEntity(new ComponentA(), new ComponentB())
            .Then()
            .RemoveComponents(typeof(ComponentB))
                .ShouldUnregisterWithSystemsThatActOn(typeof(ComponentB))
                .Test();
    }

    [Test]
    public void RemoveComponentsShouldUnregisterWithAllNewlyUninterestedSystem()
    {
        AddEntity(new ComponentA(), new ComponentB())
            .Then()
            .RemoveComponents(typeof(ComponentB))
                .ShouldUnregisterWithSystemsThatActOn(typeof(ComponentB))
                .ShouldUnregisterWithSystemsThatActOn(typeof(ComponentB))
                .Test();
    }

    [Test]
    public void RemoveComponentsShouldUnregisterWithNewlyUninterestedComplexSystem()
    {
        AddEntity(new ComponentA(), new ComponentB())
            .Then()
            .RemoveComponents(typeof(ComponentB))
                .ShouldUnregisterWithSystemsThatActOn(typeof(ComponentA), typeof(ComponentB))
                .Test();
    }

    [Test]
    public void RemoveComponentsShouldUnregisterWithAllNewlyUninterestedComplexSystem()
    {
        AddEntity(new ComponentA(), new ComponentB(), new ComponentC())
            .Then()
            .RemoveComponents(typeof(ComponentB))
                .ShouldUnregisterWithSystemsThatActOn(typeof(ComponentA), typeof(ComponentB))
                .ShouldUnregisterWithSystemsThatActOn(typeof(ComponentB), typeof(ComponentC))
                .Test();
    }

    [Test]
    public void RemoveComponentsShouldNotUregisterWithStillInterestedSystem()
    {
        AddEntity(new ComponentA(), new ComponentB())
            .Then()
            .RemoveComponents(typeof(ComponentB))
                .ShouldNotAffectSystemsThatActOn(typeof(ComponentA))
                .Test();
    }

    [Test]
    public void RemoveComponentsShouldNotUregisterWithStillInterestedComplexSystem()
    {
        AddEntity(new ComponentA(), new ComponentB(), new ComponentC())
            .Then()
            .RemoveComponents(typeof(ComponentC))
                .ShouldNotAffectSystemsThatActOn(typeof(ComponentA), typeof(ComponentB))
                .Test();
    }

    [Test]
    public void RemoveComponentsAgainShouldUnregisterWithNewlyUninterestedSystem()
    {
        AddEntity(new ComponentA(), new ComponentB(), new ComponentC())
            .Then()
            .RemoveComponents(typeof(ComponentB))
                .Then()
                .RemoveComponents(typeof(ComponentC))
                    .ShouldUnregisterWithSystemsThatActOn(typeof(ComponentC))
                    .Test();
    }

    [Test]
    public void RemoveComponentsAgainShouldUnregisterWithNewlyUninterestedComplexSystem()
    {
        AddEntity(new ComponentA(), new ComponentB(), new ComponentC())
            .Then()
            .RemoveComponents(typeof(ComponentB))
                .Then()
                .RemoveComponents(typeof(ComponentC))
                    .ShouldUnregisterWithSystemsThatActOn(typeof(ComponentA), typeof(ComponentC))
                    .Test();
    }


    //  RemoveEntity() Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void RemoveEntityWithInvalidEntityIDShouldThrowException()
    {
        EntityManager unitUnderTest = new();

        unitUnderTest.Invoking(a => a.RemoveEntity(18))
            .Should().Throw<EntityNotFoundException>();
    }

    [Test]
    public void RemovedEntityShouldNotExist()
    {
        EntityManager unitUnderTest = new();
        UInt64 entityID = unitUnderTest.AddEntity(new ComponentA(), new ComponentB());

        unitUnderTest.RemoveEntity(entityID);
        
        unitUnderTest.Invoking(a => a.GetComposition(entityID));
    }

    [Test]
    public void RemoveEntityShouldUnregisterWithPreviouslyInterestedSystems()
    {
        AddEntity(new ComponentA(), new ComponentB())
            .Then()
            .RemoveEntity()
                .ShouldUnregisterWithSystemsThatActOn(typeof(ComponentA))
                .ShouldUnregisterWithSystemsThatActOn(typeof(ComponentB))
                .ShouldUnregisterWithSystemsThatActOn(typeof(ComponentA), typeof(ComponentB))
                .Test();
    }


    
    //  Support
    //----------------------------------------------------------------------------------------------

    private static IEntityManagerStepBuilder AddEntity(params Object[] components)
    {
        return new EntityManagerSequence().AddEntity(components);
    }
}
