// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System.Collections.Immutable;

namespace Moda.Core.Entity;

/// <summary>
///     A readonly collection of components associated with an entity, often used to pass around a
///     subset of the entity's composition.
/// </summary>
public class ComponentCollection
{
    private readonly ImmutableDictionary<Type, Object> components;

    /// <summary>
    ///     Create a new instance of the <see cref="ComponentCollection"/> class.
    /// </summary>
    /// <param name="components">
    ///     The components to include.
    /// </param>
    public ComponentCollection(IEnumerable<KeyValuePair<Type, Object>> components)
    {
        this.components = components.ToImmutableDictionary();
    }

    /// <summary>
    ///     Retrieve a component of the given type.
    /// </summary>
    /// <typeparam name="T">
    ///     The <see cref="Type"/> of the component to retrieve.
    /// </typeparam>
    /// <returns>
    ///     The component instance of type T.
    /// </returns>
    /// <remarks>
    ///     Caller assumes that the entity contains the component.
    /// </remarks>
    public T GetComponent<T>()
    {
        if (!this.components.ContainsKey(typeof(T)))
        {
            throw new ComponentNotFoundException(typeof(T));
        }
        return (T)this.components[typeof(T)];
    }
}
