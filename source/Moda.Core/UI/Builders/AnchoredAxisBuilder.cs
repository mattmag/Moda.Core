// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.UI.Builders;

public abstract class AnchoredAxisBuilder
{


    public AnchoredAxisBuilder(CellBuilder cellBuilder, Axis axis, Neutral anchor)
    {
        this.CellBuilder = cellBuilder;
        this.Axis = axis;
        this.AxisRecipe = cellBuilder.BoundariesRecipe.GetAxisRecipe(axis);
        this.Anchor = anchor;
        
        // TODO: set initial coordinates based on anchor
    }
    
    
    public CellBuilder CellBuilder { get; }
    public Neutral Anchor { get; }
    public Axis Axis { get; }
    public AxisRecipe AxisRecipe { get; }
    
    
    
    protected void SetLength(ILength length)
    {
        switch (this.Anchor)
        {
            // TODO:
            case Neutral.Alpha: 
                // this.MyAxisRecipe.Beta = (this.MyAxisRecipe.Alpha.ValueOrFailure() + length).Some<Length>();
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


    // public CellBuilder FillRemaining()
    // {
    //     // TODO ...
    // }
}