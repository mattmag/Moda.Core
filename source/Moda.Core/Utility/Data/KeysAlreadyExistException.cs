// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System.Runtime.Serialization;

namespace Moda.Core.Utility.Data;

/// <summary>
///     Indicates that an attempt was made to add values to a dictionary for which the keys already
///     existed. 
/// </summary>
[Serializable]
public class KeysAlreadyExistException : ArgumentException
{
    /// <summary>
    ///     Initialize a new instance with the default message.
    /// </summary>
    public KeysAlreadyExistException(IEnumerable<String> keysAsStrings)
        : base("Keys already exist. See the keys collection for details")
    {
        this.KeysAsStrings = keysAsStrings;
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
    protected KeysAlreadyExistException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }


    /// <summary>
    ///     The list of keys that already exited in the dictionary.
    /// </summary>
    public IEnumerable<String> KeysAsStrings { get; } = new List<String>();
}
