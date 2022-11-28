// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.UI.Lengths;

public class Subtract : CompositeLength
{
    public Subtract(ILength lengthA, ILength lengthB) : base(new []{ lengthA, lengthB })
    {
        
        
    }

    public override Single Calculate()
    {
        return this.Lengths.Select(a => a.Calculate()).Aggregate((a, b) => a - b);
    }
}
