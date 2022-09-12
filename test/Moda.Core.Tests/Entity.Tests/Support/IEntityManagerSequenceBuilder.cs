// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;

namespace Moda.Core.Entity.Tests.Support;

/// <summary>
///     Provides methods to construct an <see cref="EntityManagerSequence"/>.
/// </summary>
public interface IEntityManagerSequenceBuilder
{
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
    IEntityManagerStepBuilder AddEntity(params Object[] components);

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
    IEntityManagerStepBuilder AddComponents(params Object[] components);

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
    IEntityManagerStepBuilder RemoveComponents(params Type[] types);

    /// <summary>
    ///     Remove the active entity from the <see cref="EntityManager"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="IEntityManagerStepBuilder"/> instance, for creation of an
    ///     <see cref="AddComponentsStep"/>.
    /// </returns>
    IEntityManagerStepBuilder RemoveEntity();
}