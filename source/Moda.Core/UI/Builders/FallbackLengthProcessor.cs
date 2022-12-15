// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/


using Moda.Core.UI.Lengths;

namespace Moda.Core.UI.Builders;

public class FallbackLengthProcessor : ISetLengthProcessor
{
    private readonly Placement<OptionalLength> placement;
    private Placement<OptionalLength> preferred;


    public FallbackLengthProcessor(Placement<OptionalLength> placement)
    {
        this.placement = placement;
    }


    public void SetLength(AxisRecipe axisRecipe, Placement<Length> fallback, Length length)
    {
        this.preferred.Calculate(length, out Length preferredAlpha, out Length preferredBeta);
        
        fallback.Calculate(length, out Length fallbackAlpha, out Length fallbackBeta);
        
        axisRecipe.Alpha.Set(
            new LengthOrFallback((OptionalLength)preferredAlpha, fallbackAlpha));
        axisRecipe.Beta.Set(
            new LengthOrFallback((OptionalLength)preferredBeta, fallbackBeta));
        
    }
}


