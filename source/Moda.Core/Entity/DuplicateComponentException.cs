// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System.Runtime.Serialization;

namespace Moda.Core.Entity;

/// <summary>
///     Indicates that an attempt was made to add a component to an entity that already has
///     a component of that type.
/// </summary>
public class DuplicateComponentException : ArgumentException
{
    /// <summary>
    ///     Create a new DuplicateComponentException.
    /// </summary>
    /// <param name="componentType">
    ///     The type of the component.
    /// </param>
    public DuplicateComponentException(Type componentType)
            : base($"Duplicate component of type {componentType}.")
    {
    }


    /// <summary>
    ///     Create a new DuplicateComponentException.
    /// </summary>
    /// <param name="componentType">
    ///     The type of the component.
    /// </param>
    /// <param name="innerException">
    ///     The exception that lead to this exception.
    /// </param>
    public DuplicateComponentException(Type componentType, Exception innerException)
        : base($"Duplicate component of type {componentType}.", innerException)
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
    protected DuplicateComponentException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}