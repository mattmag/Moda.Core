// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.UI.Builders;

public class StandardLengthProcessor : ISetLengthProcessor
{
    public void SetLength(AxisRecipe axisRecipe, Placement<ILength> placement, ILength length)
    {
        placement.Calculate(length, out ILength alpha, out ILength beta);
        axisRecipe.Alpha.Set(alpha);
        axisRecipe.Beta.Set(beta);
    }
}
