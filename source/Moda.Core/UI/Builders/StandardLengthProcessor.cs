// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.UI.Builders;

public class StandardLengthProcessor : ISetLengthProcessor
{
    public void SetLength(AxisRecipe axisRecipe, Placement<Length> placement, Length length)
    {
        placement.Calculate(length, out Length alpha, out Length beta);
        axisRecipe.Alpha.Set(alpha);
        axisRecipe.Alpha.Set(beta);
    }
}
