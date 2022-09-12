// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System.Runtime.Serialization;

namespace Moda.Core.Entity;

/// <summary>
///     Indicates that an attempt was made to register the same system twice.
/// </summary>
[Serializable]
public class SystemAlreadyRegisteredException : ArgumentException
{
    /// <summary>
    ///     Initialize a new instance of the DuplicateSystemException.
    /// </summary>
    /// <param name="systemType">
    ///     The type of the system that was already registered, for messaging purposes.
    /// </param>
    public SystemAlreadyRegisteredException(Type systemType)
        : base($"The same instance of system {systemType} is already registered")
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
    protected SystemAlreadyRegisteredException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
