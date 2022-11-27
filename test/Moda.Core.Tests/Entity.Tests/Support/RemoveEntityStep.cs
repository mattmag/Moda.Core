// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using Moda.Core.Entity;

namespace Moda.Core.Tests.Entity.Tests.Support;

/// <summary>
///     Remove the active entity from the <see cref="EntityManager"/>
/// </summary>
public class RemoveEntityStep : EntityManagerStep
{
    /// <summary>
    ///     Create a new <see cref="RemoveComponentStep"/>.
    /// </summary>
    /// <seealso cref="IEntityManagerSequenceBuilder"/>
    public RemoveEntityStep(EntityManagerSequence sequence)
        : base(sequence)
    {

    }

    /// <summary>
    ///     Call <see cref="EntityManager.RemoveComponents(ulong, Type[])"/>
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
        unitUnderTest.RemoveEntity(entityID);
    }
}
