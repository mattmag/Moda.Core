// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System.Runtime.Serialization;

namespace Moda.Core.Utility.Data;


/// <summary>
///     Indicates that an operation was performed on the graph that would result in a cycle.
/// </summary>
[Serializable]
public class CycleDetectedException : InvalidOperationException
{
    /// <summary>
    ///     Initialize a new instance with the default message.
    /// </summary>
    public CycleDetectedException() 
        : base("A cycle was detected in the graph, rendering it invalid")
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
    protected CycleDetectedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}