// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.Entity;

/// <summary>
///     Provides storage for entities through methods that add and remove associated components.
/// </summary>
public interface IComponentDepository
{
    /// <summary>
    ///     Add a new entity with the specified components and register it with relevant systems.
    /// </summary>
    /// <param name="components">
    ///     The components to associate with the new entity.
    /// </param>
    /// <returns>
    ///     The ID assigned to the entity.
    /// </returns>
    /// <exception cref="EmptyEntityException">
    ///     Thrown when <paramref name="components"/> has no items.
    /// </exception>
    /// <exception cref="DuplicateComponentException">
    ///     Thrown when <paramref name="components"/> has two components of the same type.
    /// </exception>
    UInt64 AddEntity(params Object[] components);


    /// <inheritdoc cref="AddEntity(object[])"/>
    UInt64 AddEntity(IEnumerable<Object> components);


    /// <summary>
    ///     Remove all components from an entity and unregister it from all relevant systems.
    /// </summary>
    /// <param name="entityID">
    ///     The ID of the entity to remove.
    /// </param>
    /// <exception cref="EntityNotFoundException">
    ///     Thrown when the entity does not exist
    /// </exception>
    void RemoveEntity(UInt64 entityID);


    /// <summary>
    ///     Add new components to an existing entity, registering it with any newly relevant
    ///     systems.
    /// </summary>
    /// <param name="entityID">
    ///     The ID of the entity to add components to.
    /// </param>
    /// <param name="components">
    ///     The components to add to the entity.
    /// </param>
    /// <exception cref="EntityNotFoundException">
    ///     Thrown when the entity does not exist
    /// </exception>
    /// <exception cref="DuplicateComponentException">
    ///     Thrown when a component of type T already exists for the entity.
    /// </exception>
    void AddComponents(UInt64 entityID, params Object[] components);


    /// <inheritdoc cref="AddComponents(ulong,object[])"/>
    void AddComponents(UInt64 entityID, IEnumerable<Object> components);


    /// <summary>
    ///     Remove the components of the specified types from the entity, unregistering it with any
    ///     newly irrelevant systems.
    /// </summary>
    /// <param name="entityID">
    ///     The ID of the entity to remove components from.
    /// </param>
    /// <param name="types">
    ///     The types of the components to remove.
    /// </param>
    /// <exception cref="ComponentNotFoundException">
    ///     Thrown when the entity does not contain a component of the specified type.
    /// </exception>
    void RemoveComponents(UInt64 entityID, params Type[] types);


    /// <inheritdoc cref="RemoveComponents(ulong,System.Type[])"/>
    void RemoveComponents(UInt64 entityID, IEnumerable<Type> types);
}
