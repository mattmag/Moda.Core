// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.Utility.Data;

namespace Moda.Core.UI.Lengths;

public class Add : CompositeLength
{
    
    
    public Add(ILength lengthA, ILength lengthB) : base(new []{ lengthA, lengthB })
    {
        
    }

    public override Single Calculate()
    {   
        return this.Lengths.Sum(a => a.Calculate());
    }
}
