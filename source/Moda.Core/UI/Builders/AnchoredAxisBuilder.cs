// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.UI.Lengths;
using Optional;

namespace Moda.Core.UI.Builders;



public abstract class AnchoredAxisBuilder
{
    private Option<Length> offset;

    public AnchoredAxisBuilder(CellBuilder cellBuilder, Axis axis, Neutral anchor)
    {
        this.CellBuilder = cellBuilder;
        this.Axis = axis;
        this.AxisRecipe = cellBuilder.BoundariesRecipe.GetAxisRecipe(axis);
        this.Anchor = anchor;
    }
    
    
    public CellBuilder CellBuilder { get; }
    public Neutral Anchor { get; }
    public Axis Axis { get; }
    public AxisRecipe AxisRecipe { get; }



    protected void SetOffset(Length length)
    {
        this.offset = length.Some();
    }
    
    
    protected void SetLength(Length length)
    {
        switch (this.Anchor)
        {
            case Neutral.Alpha:
                this.AxisRecipe.Alpha.Set(this.offset.ValueOr(new Pixels(0)));
                this.AxisRecipe.Beta.Set(this.AxisRecipe.Alpha.Get() + length);
                break;
            case Neutral.Center:
                Length center = new SizeOfParent() / 2;
                Length target = this.offset.Match(off => center + off, () => center);
                this.AxisRecipe.Alpha.Set(target - (length / 2));
                this.AxisRecipe.Beta.Set(target + (length / 2));
                break;
            case Neutral.Beta:
                Length beta = new SizeOfParent();
                this.AxisRecipe.Beta.Set(this.offset.Match(off => beta + off, () => beta));
                this.AxisRecipe.Alpha.Set(this.AxisRecipe.Beta.Get() - length);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    
}