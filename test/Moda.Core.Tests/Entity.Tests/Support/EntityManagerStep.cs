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
///     Represents a step in an <see cref="EntityManagerSequence"/>.
/// </summary>
public abstract class EntityManagerStep : IEntityManagerStepBuilder
{
    //##########################################################################################
    //
    //   Fields
    //
    //##########################################################################################

    /// <summary>
    ///     The sequence that this step belongs to.
    /// </summary>
    private readonly EntityManagerSequence sequence;


    //##########################################################################################
    //
    //   Constructors
    //
    //##########################################################################################
    
    /// <summary>
    ///     Create a new <see cref="EntityManagerStep"/>
    /// </summary>
    /// <param name="sequence"></param>
    public EntityManagerStep(EntityManagerSequence sequence)
    {
        this.sequence = sequence;
    }


    //##########################################################################################
    //
    //   Properties
    //
    //##########################################################################################
   

    private readonly List<SystemSpecification> _systemSpecifications = new();
    /// <summary>
    ///     The criteria to check for passing the unit test.
    /// </summary>
    public IEnumerable<SystemSpecification> SystemsSpecifications
    {
        get
        {
            return this._systemSpecifications;
        }
    }

    //##########################################################################################
    //
    //   Public Methods
    //
    //##########################################################################################

    /// <summary>
    ///     Perform the step's action on the <see cref="EntityManager"/> then check the
    ///     registered systems for the expected side effects.
    /// </summary>
    /// <param name="unitUnderTest">
    ///     The <see cref="EntityManager"/> at the focus of the test.
    /// </param>
    /// <param name="stepNumber">
    ///     Indicates which step in the <see cref="EntityManagerSequence"/> this step is, used
    ///     to help debug failed tests.
    /// </param>
    /// <param name="entityID">
    ///     The ID of the active entity.  Note the <see langword="ref"/> keyword, used by steps
    ///     such as <see cref="AddEntityStep"/> to store the ID of the newly created entity.
    /// </param>
    /// <param name="systems">
    ///     A <see cref="ILookup{TKey,TElement}"/> of <see cref="Mock"/> systems, keyed by
    ///     their <see cref="IComponentSystem.ActsOn"/> values.
    /// </param>
    public void Test(EntityManager unitUnderTest, Int32 stepNumber, ref UInt64 entityID,
        ILookup<ImmutableHashSet<Type>, Mock<IComponentSystem>> systems)
    {
        // perform the steps action
        Action(unitUnderTest, ref entityID);

        // local copy to appease our lambda functions below
        UInt64 localEntityID = entityID;

        // incremented with each check, used to help debug failed tests.
        Int32 systemNumber = 1;
        foreach (SystemSpecification systemSpec in this.SystemsSpecifications)
        {
            IEnumerable<Mock<IComponentSystem>> mockSystems = systems[systemSpec.ActsOn];
            switch (systemSpec.ExpectedRegistration)
            {
                case RegistrationBehavior.NoChange:
                {
                    foreach (var mock in mockSystems)
                    {
                        mock.Verify(a => a.RegisterEntity(It.IsAny<UInt64>()), Times.Never,
                            GetLocation(stepNumber, systemNumber));
                        mock.Verify(a => a.UnregisterEntity(It.IsAny<UInt64>()), Times.Never,
                            GetLocation(stepNumber, systemNumber));
                    }
                    break;
                }
                case RegistrationBehavior.RegisterEntity:
                {
                    foreach (var mock in mockSystems)
                    {
                        mock.Verify(a => a.RegisterEntity(localEntityID), Times.Once,
                            GetLocation(stepNumber, systemNumber));
                    }
                    break;
                }
                case RegistrationBehavior.UnregisterEntity:
                {
                    foreach (var mock in mockSystems)
                    {
                        mock.Verify(a => a.UnregisterEntity(localEntityID), Times.Once,
                            GetLocation(stepNumber, systemNumber));
                    }
                    break;
                }
            }

            systemNumber++;
        }
    }

    /// <summary>
    ///     Mark that the step should have the affect of registering the active entity with
    ///     systems that act on the specified types.
    /// </summary>
    /// <param name="actsOn">
    ///     The types that these imaginary systems act on.
    /// </param>
    /// <returns>
    ///   This instance, to continue the builder pattern.
    /// </returns>
    public IEntityManagerStepBuilder ShouldRegisterWithSystemThatActOn(params Type[] actsOn)
    {
        this._systemSpecifications.Add(
            new SystemSpecification(RegistrationBehavior.RegisterEntity, actsOn));
        return this;
    }

    /// <summary>
    ///     Mark that the step should have no affect on systems that act on the specified types.
    /// </summary>
    /// <param name="actsOn">
    ///     The types that these imaginary systems act on.
    /// </param>
    /// <returns>
    ///   This instance, to continue the builder pattern.
    /// </returns>
    public IEntityManagerStepBuilder ShouldNotAffectSystemsThatActOn(params Type[] actsOn)
    {
        this._systemSpecifications.Add(
            new SystemSpecification(RegistrationBehavior.NoChange, actsOn));
        return this;
    }

    /// <summary>
    ///     Mark that the step should have the affect of unregistering the active entity with
    ///     systems that act on the specified types.
    /// </summary>
    /// <param name="actsOn">
    ///     The types that these imaginary systems act on.
    /// </param>
    /// <returns>
    ///   This instance, to continue the builder pattern.
    /// </returns>
    public IEntityManagerStepBuilder ShouldUnregisterWithSystemsThatActOn(params Type[] actsOn)
    {
        this._systemSpecifications.Add(
            new SystemSpecification(RegistrationBehavior.UnregisterEntity, actsOn));
        return this;
    }

    /// <summary>
    ///     End the builder pattern by testing the entire <see cref="EntityManagerSequence"/>.
    /// </summary>
    public void Test()
    {
        this.sequence.Test();
    }

    /// <summary>
    ///     Finish defining this step and being defining a new one.
    /// </summary>
    /// <returns>
    ///     The <see cref="IEntityManagerSequenceBuilder"/>, in order to continue building
    ///     the sequence.
    /// </returns>
    public IEntityManagerSequenceBuilder Then()
    {
        return this.sequence;
    }


    //##########################################################################################
    //
    //   Private Methods
    //
    //##########################################################################################

    /// <summary>
    ///     An action to perform on the <see cref="EntityManager"/> when
    ///     <see cref="Test(Moda.Core.Entity.EntityManager,int,ref ulong,System.Linq.ILookup{System.Collections.Immutable.ImmutableHashSet{System.Type},Moq.Mock{Moda.Core.Entity.IComponentSystem}})"/>
    ///     is called.
    /// </summary>
    /// <param name="unitUnderTest">
    ///     The <see cref="EntityManager"/> at the focus of the test.
    /// </param>
    /// <param name="entityID">
    ///     The ID of the active entity.  Note the <see langword="ref"/> keyword, used by steps
    ///     such as <see cref="AddEntityStep"/> to store the ID of the newly created entity.
    /// </param>
    protected abstract void Action(EntityManager unitUnderTest, ref UInt64 entityID);

    /// <summary>
    ///     Format a user-friendly string to help identify where in the sequence the unit test
    ///     failed.
    /// </summary>
    /// <param name="stepNumber">
    ///     Identifies which step in the sequence.
    /// </param>
    /// <param name="systemNumber">
    ///     Identifies which <see cref="SystemSpecification"/> in the step failed.
    /// </param>
    /// <returns></returns>
    private String GetLocation(Int32 stepNumber, Int32 systemNumber)
    {
        return $"Step: {stepNumber} | System Index: {systemNumber}";
    }

}