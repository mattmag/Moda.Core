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
///     Add components to the active entity.
/// </summary>
public class AddComponentsStep : EntityManagerStep
{
    /// <summary>
    ///     The components to add.
    /// </summary>
    private readonly IEnumerable<Object> components;

    /// <summary>
    ///     Create a new <see cref="AddComponentsStep"/>.
    /// </summary>
    /// <param name="sequence">
    ///     The <see cref="EntityManagerSequence"/> that this step belongs to.
    /// </param>
    /// <param name="components">
    ///     The components to add to the entity.
    /// </param>
    /// <seealso cref="IEntityManagerSequenceBuilder"/>
    public AddComponentsStep(EntityManagerSequence sequence, IEnumerable<Object> components)
        : base(sequence)
    {
        this.components = components;
    }

    /// <summary>
    ///     Call <see cref="EntityManager.AddComponents(UInt64, global::System.Object[])"/>
    ///     on the <see cref="EntityManager"/>.
    /// </summary>
    /// <param name="unitUnderTest">
    ///     The <see cref="EntityManager"/> at the focus of the unit test.
    /// </param>
    /// <param name="entityID">
    ///     The ID of the active entity.
    /// </param>
    protected override void Action(EntityManager unitUnderTest, ref UInt64 entityID)
    {
        // .ToArray() is to cover chained overloads
        unitUnderTest.AddComponents(entityID, this.components.ToArray());
    }
}
