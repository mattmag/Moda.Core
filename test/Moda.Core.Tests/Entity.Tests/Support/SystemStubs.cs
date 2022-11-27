// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using System.Collections.Immutable;
using Moda.Core.Entity;

namespace Moda.Core.Tests.Entity.Tests.Support;


public class SystemA : IComponentSystem
{
    public ImmutableHashSet<Type> ActsOn => ImmutableHashSet.Create(typeof(ComponentA));
    
    public void RegisterEntity(UInt64 entityID)
    {
        
    }


    public void UnregisterEntity(UInt64 entityID)
    {
        
    }
}
