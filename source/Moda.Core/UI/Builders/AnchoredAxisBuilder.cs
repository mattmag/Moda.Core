// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.UI.Lengths;
using Optional;
using Optional.Unsafe;

namespace Moda.Core.UI.Builders;

public abstract class AnchoredAxisBuilder
{


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
    
    
    
    
    
    protected void SetLength(Length length)
    {
        switch (this.Anchor)
        {
            case Neutral.Alpha:
                this.AxisRecipe.Alpha.Set(new Pixels(0));
                this.AxisRecipe.Beta.Set(this.AxisRecipe.Alpha.Get() + length);
                break;
            case Neutral.Center:
                
                // this.MyAxisRecipe.Alpha = (new CenterOfParent() - (length / 2)).Some<Length>();
                // this.MyAxisRecipe.Beta = (new CenterOfParent() + (length / 2)).Some<Length>();
                break;
            case Neutral.Beta:
                // this.MyAxisRecipe.Alpha = (this.MyAxisRecipe.Beta.ValueOrFailure() - length).Some<Length>();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    
}