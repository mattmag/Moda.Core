// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.UI.Lengths;

public class LengthOrFallback : CompositeLength
{
    private OptionalLength preferred;
    private Length fallback;


    public LengthOrFallback(OptionalLength preferred, Length fallback)
    {
        this.preferred = preferred;
        this.fallback = fallback;
        this.AddLength(preferred);
        this.AddLength(fallback);
    }

    public override Single Calculate()
    {
        return this.preferred.TryCalculate().ValueOr(this.fallback.Calculate());
    }
}
