// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System.Collections.Immutable;

namespace Moda.Core.Entity;

/// <summary>
///     A system adds logic and functionality to a component or group of components.
///
///     Systems registered with an entity manager will receive notifications via
///     <see cref="RegisterEntity(UInt64)"/> when an entity's composition is a superset of the
///     <see cref="ActsOn"/> set.  Systems can then choose to do what they like with the relevant
///     components for each registered entity, with a common pattern being to loop through each
///     registered entity during an update loop, retrieving the components from the entity manager,
///     and applying some logic to the component's data.
/// </summary>
public interface IComponentSystem
{
    /// <summary>
    ///     Indicates to an <see cref="IEntityManager"/> what component types an entity is required
    ///     to have in order to be processed by this system.
    /// </summary>
    ImmutableHashSet<Type> ActsOn { get; }


    /// <summary>
    ///     Add a new entity to be processed by this system.
    /// </summary>
    /// <param name="entityID">
    ///     The ID of the entity whose composition is now a superset of this system;s
    ///     <see cref="ActsOn"/> collection.
    /// </param>
    void RegisterEntity(UInt64 entityID);


    /// <summary>
    ///     Remove the specified entity from this system.
    /// </summary>
    /// <param name="entityID">
    ///     The ID of the entity whose composition is no longer a superset of this system's
    ///     <see cref="ActsOn"/> collection.
    /// </param>
    void UnregisterEntity(UInt64 entityID);
}
