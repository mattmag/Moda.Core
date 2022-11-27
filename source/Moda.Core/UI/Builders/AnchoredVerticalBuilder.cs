// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.Lengths;
using Optional;

namespace Moda.Core.UI.Builders;

public class AnchoredVerticalBuilder : AnchoredAxisBuilder
{
    public AnchoredVerticalBuilder(CellBuilder sourceBuilder, Vertical anchor)
        : base(sourceBuilder, Axis.Y, ConvertAnchor(anchor))
    {
        //  TODO: placeholder, could probably do in base class
        this.AxisRecipe.Alpha = new Pixels(0).Some<ILength>();
        this.AxisRecipe.Beta = new Pixels(0).Some<ILength>();
    }
    
    public IMinmimumViableCell WithHeight(ILength width)
    {
        this.SetLength(width);
        return this.CellBuilder;
    }
    
    private static Neutral ConvertAnchor(Vertical anchor)
    {
        return anchor switch
            {
                Vertical.Top => Neutral.Alpha,
                Vertical.Middle => Neutral.Center,
                Vertical.Bottom => Neutral.Beta,
                _ => throw new ArgumentOutOfRangeException(nameof(anchor), anchor, null)
            };
    }
}