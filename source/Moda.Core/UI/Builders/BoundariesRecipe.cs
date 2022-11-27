// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.UI.Builders;

public class BoundariesRecipe
{
    public AxisRecipe XBoundary { get; } = new();
    public AxisRecipe YBoundary { get; } = new();
    
    public AxisRecipe GetAxisRecipe(Axis axis)
    {
        return axis switch
            {
                Axis.X => this.XBoundary,
                Axis.Y => this.YBoundary,
                _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
            };
    }
}