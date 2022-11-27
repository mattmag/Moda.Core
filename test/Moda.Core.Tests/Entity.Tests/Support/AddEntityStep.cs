// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using System.Collections.Generic;
using System.Linq;
using Moda.Core.Entity;

namespace Moda.Core.Tests.Entity.Tests.Support;

/// <summary>
///     Add a new entity to the <see cref="EntityManager"/> and set it's ID to the active
///     entity ID in the unit test.
/// </summary>
public class AddEntityStep : EntityManagerStep
{
    /// <summary>
    ///     The components to add.
    /// </summary>
    private readonly IEnumerable<Object> components;

    /// <summary>
    ///     Create a new <see cref="AddEntityStep"/>/
    /// </summary>
    /// <param name="sequence">
    ///     The <see cref="EntityManagerSequence"/> that this step belongs to.
    /// </param>
    /// <param name="components">
    ///     The components to add during creation of the entity.
    /// </param>
    /// <seealso cref="IEntityManagerSequenceBuilder"/>
    public AddEntityStep(EntityManagerSequence sequence, IEnumerable<Object> components)
        : base(sequence)
    {
        this.components = components;
    }

    /// <summary>
    ///     Call <see cref="EntityManager.AddEntity(IEnumerable{Object})"/>
    ///     on the <see cref="EntityManager"/>.
    /// </summary>
    /// <param name="unitUnderTest">
    ///     The <see cref="EntityManager"/> at the focus of the unit test.
    /// </param>
    /// <param name="entityID">
    ///     Will be set to the resulting ID of the new entity.
    /// </param>
    protected override void Action(EntityManager unitUnderTest, ref UInt64 entityID)
    {
        // .ToArray() is to cover chained overloads
        entityID = unitUnderTest.AddEntity(this.components.ToArray());
    }
}