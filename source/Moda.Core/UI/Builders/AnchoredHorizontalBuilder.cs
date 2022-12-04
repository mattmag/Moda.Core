using Moda.Core.UI.Lengths;
using Optional;

namespace Moda.Core.UI.Builders;
// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/


public class AnchoredHorizontalBuilder : AnchoredAxisBuilder
{
    public AnchoredHorizontalBuilder(CellBuilder cellBuilder, Horizontal anchor)
        : base(cellBuilder, Axis.X, ConvertAnchor(anchor))
    {
        //  TODO: placeholder
        this.AxisRecipe.Alpha = new Pixels(0).Some<Length>();
        this.AxisRecipe.Beta = new Pixels(0).Some<Length>();
    }


    public IInitializeVerticalAxis WithWidth(Length width)
    {
        this.SetLength(width);
        return this.CellBuilder;
    }
    
    private static Neutral ConvertAnchor(Horizontal anchor)
    {
        return anchor switch
            {
                Horizontal.Left => Neutral.Alpha,
                Horizontal.Center => Neutral.Center,
                Horizontal.Right => Neutral.Beta,
                _ => throw new ArgumentOutOfRangeException(nameof(anchor), anchor, null)
            };
    }
}