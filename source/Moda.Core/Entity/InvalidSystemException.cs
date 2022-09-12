// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System.Runtime.Serialization;

namespace Moda.Core.Entity;

/// <summary>
///     Indicates that an attempt was made to register a system that does not meet the
///     registration criteria.
/// </summary>
[Serializable]
public class InvalidSystemException : KeyNotFoundException
{
    /// <summary>
    ///     Initialize a new instance with the default message.
    /// </summary>
    /// <param name="message">
    ///     The message to include with the exception
    /// </param>
    public InvalidSystemException(String message) : base(message)
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
    protected InvalidSystemException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}