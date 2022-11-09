// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Events;
using FluentAssertions.Execution;

namespace Moda.Core.Support;

public static class AssertionHelper
{
    public static bool ToPredicate(Action act)
    {
        using (var scope = new AssertionScope())
        {
            act();
            return !scope.Discard().Any();
        }
    }
    
    public static IEventRecording WithAssertedArgs<T>(this IEventRecording eventRecording,
        Action<T> assert) => eventRecording.WithArgs<T>(a => ToPredicate(() => assert(a)));
    
}
