// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System.Runtime.Serialization;

namespace Moda.Core.Entity;

/// <summary>
///     Indicates that an operation was attempted on a component type that does not exist for
///     the specified entity.
/// </summary>
[Serializable]
public class ComponentNotFoundException : KeyNotFoundException
{
    /// <summary>
    ///     Initializes a new instance with a message including details on the entity and
    ///     component type.
    /// </summary>
    /// <param name="componentType">
    ///     The type of component that was not found.
    /// </param>
    public ComponentNotFoundException(Type componentType)
        : base($"No component of type {componentType} found.")
    {
    }

    /// <summary>
    ///     Initializes a new instance with a message including details on the entity and
    ///     component type.
    /// </summary>
    /// <param name="entity">
    ///     The ID of the entity of who's component was not retrieved.
    /// </param>
    /// <param name="componentType">
    ///     The type of component that was not found.
    /// </param>
    public ComponentNotFoundException(UInt64 entity, Type componentType)
        : base($"No component of type {componentType} found for entity {entity}")
    {
    }

    /// <summary>
    ///     Initializes a new instance with a message including details on the entity and
    ///     component type.
    /// </summary>
    /// <param name="entity">
    ///     The ID of the entity of who's component was not retrieved.
    /// </param>
    /// <param name="componentType">
    ///     The type of component that was not found.
    /// </param>
    /// <param name="innerException">
    ///     The exception that lead to this exception.
    /// </param>
    public ComponentNotFoundException(UInt64 entity, Type componentType,
        Exception innerException)
        : base($"No component of type {componentType} found for entity {entity}", innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance with serialized data.
    /// </summary>
    /// <param name="info">
    ///     The SerializationInfo that holds the serialized object data about the exception
    ///     being thrown.
    /// </param>
    /// <param name="context">
    ///     The StreamingContext that contains contextual information about the source or
    ///     destination.
    /// </param>
    protected ComponentNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}