// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.Utility.Data;

public delegate void CollectionChangedHandler<T>(object? sender, CollectionChangedArgs<T> args);

public class CollectionChangedArgs<T>
{
    public CollectionChangedArgs(IEnumerable<T> itemsAdded, IEnumerable<T> itemsRemoved)
    {
        ItemsAdded = itemsAdded;
        ItemsRemoved = itemsRemoved;
    }
    
    public IEnumerable<T> ItemsAdded { get; }
    public IEnumerable<T> ItemsRemoved { get; }
}
