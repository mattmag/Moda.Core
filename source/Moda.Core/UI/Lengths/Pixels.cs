// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.UI;

namespace Moda.Core.Lengths;

public class Pixels : Length
{
    public Pixels(Int32 value)
    {
        Value = value;
    }
    
    public Int32 Value { get; set; }
    
    public override Single Calculate()
    {
        throw new NotImplementedException();
    }
}
