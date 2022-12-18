// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.UI.Lengths;
using Optional;

namespace Moda.Core.UI.Builders;


public abstract class NAnchorBuilder
{
    private readonly AxisRecipe axisRecipe;
    private readonly ISetLengthProcessor lengthProcessor;
    private readonly NAnchor anchor;
    private Option<ILength> offset;


    protected NAnchorBuilder(CellBuilderState runningState, ISetLengthProcessor lengthProcessor,
        NAnchor anchor, Axis axis)
    {
        this.RunningState = runningState;
        this.axisRecipe = this.RunningState.Boundaries.GetAxisRecipe(axis);
        this.lengthProcessor = lengthProcessor;
        this.anchor = anchor;
    }
    
    protected CellBuilderState RunningState { get; }
    
    protected void SetOffset(ILength length)
    {
        this.offset = length.Some();
    }


    protected void SetLength(ILength width)
    {
        this.lengthProcessor.SetLength(this.axisRecipe, GetPlacement().ToBase(), width);
    }
    
    
    private Placement<ILength> GetPlacement()
    {
        Placement<ILength> getBetaPlacement()
        {
            ILength beta = new PercentOfParent(100);
            return new(NCoordinate.Beta, _ => this.offset.Match(off => beta + off, () => beta));
        }
        
        Placement<ILength> getCenterPlacement()
        {
            ILength center = new PercentOfParent(50);
            ILength target = this.offset.Match(off => center + off, () => center);
            return new(NCoordinate.Alpha, len => target - (len / 2));
        }
        
        return this.anchor switch
            {
                NAnchor.Alpha => new(NCoordinate.Alpha, _ => this.offset.ValueOr(new Pixels(0))),
                NAnchor.Center => getCenterPlacement(),
                NAnchor.Beta => getBetaPlacement(),
                _ => throw new ArgumentOutOfRangeException()
            };
    }
    
}
