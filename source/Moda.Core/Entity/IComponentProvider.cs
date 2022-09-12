// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System.Collections.Immutable;
using Optional;

namespace Moda.Core.Entity;

/// <summary>
///     Provides a way to retrieve components for a given entity.
/// </summary>
/// <remarks>
///     In most cases it is recommended that systems simply store their registered entities as ID's
///     and access components through the methods available in this interface as needed.  This
///     ensures a single source of truth for entity composition.
/// </remarks>
public interface IComponentProvider
{
    /// <summary>
    ///     Retrieve a component of the specified type for the given entity.
    /// </summary>
    /// <param name="entityID">
    ///     The ID of the entity of interest.
    /// </param>
    /// <typeparam name="T">
    ///     The <see cref="Type"/> of the component to retrieve.
    /// </typeparam>
    /// <returns>
    ///     The component instance associated with the entity.
    /// </returns>
    /// <remarks>
    ///     The caller assumes that the entity contains the component.
    ///     This is the recommended way for systems to request components, as a system can always
    ///     assume that a registered entity will have the components specified by it's
    ///     <see cref="IComponentSystem.ActsOn"/> collection.
    /// </remarks>
    /// <exception cref="ComponentNotFoundException">
    ///     Thrown when a component of type T can not be found for the entity.
    /// </exception>
    T GetComponent<T>(UInt64 entityID);


    /// <summary>
    ///     Attempt to retrieve a component of the specified type for the given entity.
    /// </summary>
    /// <param name="entityID">
    ///     The ID of the entity of interest.
    /// </param>
    /// <typeparam name="T">
    ///      The <see cref="Type"/> of the component to retrieve.
    /// </typeparam>
    /// <returns>
    ///     An <see cref="Option"/> instance with a component of the requested type if one
    ///     was found for the given entity.
    /// </returns>
    /// <remarks>
    ///     When following the recommended pattern for component systems this method should not be
    ///     needed, as a system can always assume that a registered entity will have the components
    ///     specified by it's <see cref="IComponentSystem.ActsOn"/> collection.  Also, generally,
    ///     systems should not act on components outside of their scope.  However, it is
    ///     understandable that in special circumstances, or in other contexts, it may be convenient
    ///     to access a component that may or may not exist for a given entity.
    ///
    ///     This method exists as an alternative to calling <see cref="GetComponent{T}"/> and
    ///     potentially throwing a <see cref="ComponentNotFoundException"/>, which may impact
    ///     performance. It also serves as an alternative to iterating through the results of
    ///     <see cref="GetAllComponents(UInt64)"/> or <see cref="GetComposition(UInt64)"/>, which
    ///     may be cumbersome.  
    /// </remarks>
    Option<T> GetComponentOrNone<T>(UInt64 entityID);


    /// <summary>
    ///     Retrieve multiple components for the given entity.
    /// </summary>
    /// <param name="entityID">
    ///     The ID of the entity of interest.
    /// </param>
    /// <param name="types">
    ///     The <see cref="Type">Types</see> of the components to retrieve.
    /// </param>
    /// <returns>
    ///     A <see cref="ComponentCollection"/> containing the component instances of the requested
    ///     types associated with the entity.
    /// </returns>
    /// <remarks>
    ///     Caller assumes that the entity contains the component.
    /// </remarks>
    /// <exception cref="ComponentNotFoundException">
    ///     Thrown when a component of type T can not be found for the entity.
    /// </exception>
    ComponentCollection GetComponents(UInt64 entityID, params Type[] types);


    /// <inheritdoc cref="GetComponents(ulong,System.Type[])"/>
    ComponentCollection GetComponents(UInt64 entityID, IEnumerable<Type> types);


    /// <summary>
    ///     Retrieve all components for the given entity.
    /// </summary>
    /// <param name="entityID">
    ///     The ID of the entity of interest.
    /// </param>
    /// <returns>
    ///     A <see cref="ComponentCollection"/> containing all of the component instances
    ///     associated with the entity.
    /// </returns>
    ComponentCollection GetAllComponents(UInt64 entityID);


    /// <summary>
    ///     Retrieve the full composition of the given entity.
    /// </summary>
    /// <param name="entityID">
    ///     The ID of the entity of interest.
    /// </param>
    /// <returns>
    ///     An <see cref="ImmutableHashSet{T}"/> containing the <see cref="Type">Types</see> of
    ///     all of the entity's components.
    /// </returns>
    ImmutableHashSet<Type> GetComposition(UInt64 entityID);
}
