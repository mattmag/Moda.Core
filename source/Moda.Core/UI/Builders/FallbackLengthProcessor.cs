// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/


using Moda.Core.UI.Lengths;

namespace Moda.Core.UI.Builders;

public class FallbackLengthProcessor : ISetLengthProcessor
{
    private readonly Placement<IOptionalLength> placement;
    private Placement<IOptionalLength> preferred;


    public FallbackLengthProcessor(Placement<IOptionalLength> placement)
    {
        this.placement = placement;
    }


    public void SetLength(AxisRecipe axisRecipe, Placement<ILength> fallback, ILength length)
    {
        this.preferred.Calculate(length, out ILength preferredAlpha, out ILength preferredBeta);
        
        fallback.Calculate(length, out ILength fallbackAlpha, out ILength fallbackBeta);
        
        axisRecipe.Alpha.Set(
            new LengthOrFallback((IOptionalLength)preferredAlpha, fallbackAlpha));
        axisRecipe.Beta.Set(
            new LengthOrFallback((IOptionalLength)preferredBeta, fallbackBeta));
        
    }
}


