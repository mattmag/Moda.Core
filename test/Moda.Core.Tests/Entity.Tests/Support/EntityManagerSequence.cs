// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Moq;

namespace Moda.Core.Entity.Tests.Support;

/// <summary>
///     Used to execute a sequence of entity modification on an <see cref="EntityManager"/> 
///     while testing the sideeffects imparted on registered systems.
/// </summary>
/// <remarks>
///     This class was created in an effort to make certain <see cref="EntityManagerTests"/>
///     more readable.
/// </remarks>
public class EntityManagerSequence : IEntityManagerSequenceBuilder
{
    /// <summary>
    ///     The steps to execute.
    /// </summary>
    private readonly List<EntityManagerStep> steps = new ();

    /// <summary>
    ///     Create a new <see cref="EntityManagerSequence"/>.
    /// </summary>
    public EntityManagerSequence()
    {

    }

    /// <summary>
    ///     Add an entity to the <see cref="EntityManager"/> with the provided components.
    /// </summary>
    /// <param name="components">
    ///     The components to include during creation of the entity.
    /// </param>
    /// <returns>
    ///     A <see cref="IEntityManagerStepBuilder"/> instance, for creation of an
    ///     <see cref="AddEntityStep"/>.
    /// </returns>
    public IEntityManagerStepBuilder AddEntity(params Object[] components)
    {
        AddEntityStep rval = new(this, components);
        this.steps.Add(rval);
        return rval;
    }

    /// <summary>
    ///     Add components to the active entity (as determined by the previous
    ///     <see cref="AddEntity"/> call).
    /// </summary>
    /// <param name="components">
    ///     The components to add.
    /// </param>
    /// <returns>
    ///     A <see cref="IEntityManagerStepBuilder"/> instance, for creation of an
    ///     <see cref="AddComponentsStep"/>.
    /// </returns>
    public IEntityManagerStepBuilder AddComponents(params Object[] components)
    {
        AddComponentsStep rval = new(this, components);
        this.steps.Add(rval);
        return rval;
    }

    /// <summary>
    ///     Remove components from the active entity (as determined by the previous
    ///     <see cref="AddEntity"/> call).
    /// </summary>
    /// <param name="types">
    ///     The types of the components to remove.
    /// </param>
    /// <returns>
    ///     A <see cref="IEntityManagerStepBuilder"/> instance, for creation of an
    ///     <see cref="AddComponentsStep"/>.
    /// </returns>
    public IEntityManagerStepBuilder RemoveComponents(params Type[] types)
    {
        RemoveComponentStep rval = new(this, types);
        this.steps.Add(rval);
        return rval;
    }


    /// <summary>
    ///     Remove the active entity from the <see cref="EntityManager"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="IEntityManagerStepBuilder"/> instance, for creation of an
    ///     <see cref="AddComponentsStep"/>.
    /// </returns>
    public IEntityManagerStepBuilder RemoveEntity()
    {
        RemoveEntityStep rval = new(this);
        this.steps.Add(rval);
        return rval;
    }

    /// <summary>
    ///     Execute the test steps and verify that the expected side effects took place.
    /// </summary>
    public void Test()
    {
        EntityManager unitUnderTest = new();

        List<KeyValuePair<ImmutableHashSet<Type>, Mock<IComponentSystem>>> temp = new();

        foreach (SystemSpecification systemSpec in this.steps.SelectMany(a => a.SystemsSpecifications))
        {
            Mock<IComponentSystem> system = new();
            system.SetupGet(a => a.ActsOn).Returns(systemSpec.ActsOn);
            unitUnderTest.RegisterSystem(system.Object);
            temp.Add(new KeyValuePair<ImmutableHashSet<Type>, Mock<IComponentSystem>>(systemSpec.ActsOn, system));
        }

        ILookup<ImmutableHashSet<Type>, Mock<IComponentSystem>> systems = temp.ToLookup(a => a.Key, a => a.Value);

        UInt64 entityID = 0;
        Int32 stepNumber = 1;
        foreach (EntityManagerStep step in this.steps)
        {
            step.Test(unitUnderTest, stepNumber, ref entityID, systems);
            foreach (Mock<IComponentSystem> system in systems.SelectMany(a => a))
            {
                system.Invocations.Clear();
            }
            stepNumber++;
        }
    }

}