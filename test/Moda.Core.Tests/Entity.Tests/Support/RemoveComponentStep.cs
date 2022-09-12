// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using System.Collections.Generic;
using System.Linq;

namespace Moda.Core.Entity.Tests.Support;

/// <summary>
///     Remove components from the active entity.
/// </summary>
public class RemoveComponentStep : EntityManagerStep
{
    /// <summary>
    ///     The components to add.
    /// </summary>
    private readonly IEnumerable<Type> types;

    /// <summary>
    ///     Create a new <see cref="RemoveComponentStep"/>.
    /// </summary>
    /// <param name="sequence">
    ///     The <see cref="EntityManagerSequence"/> that this step belongs to.
    /// </param>
    /// <param name="types">
    ///     The types of the components to remove from the entity.
    /// </param>
    /// <seealso cref="IEntityManagerSequenceBuilder"/>
    public RemoveComponentStep(EntityManagerSequence sequence, IEnumerable<Type> types)
        : base(sequence)
    {
        this.types = types;
    }

    /// <summary>
    ///     Call <see cref="EntityManager.RemoveComponents(UInt64, Type[])"/>
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
        unitUnderTest.RemoveComponents(entityID, this.types.ToArray());
    }
}