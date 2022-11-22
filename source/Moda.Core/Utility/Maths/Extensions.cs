// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.Utility.Maths;

public static class Extensions
{
    public static Int32 Decrement(this Int32 val)
    {
        return Math.Max(val - 1, 0);
    }
}
