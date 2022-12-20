// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.UI.Lengths;
using Optional;

namespace Moda.Core.UI.Builders;

public abstract class NAdjacencyBuilder
{
    private readonly AxisRecipe axisRecipe;
    private readonly ISetLengthProcessor lengthProcessor;
    private readonly NAdjacent adjacent;
    private Option<ILength> offset;
    
    public NAdjacencyBuilder(CellBuilderState runningState, ISetLengthProcessor lengthProcessor,
        NAdjacent adjacent, Axis axis)
    {
        this.RunningState = runningState;
        this.axisRecipe = runningState.Boundaries.GetAxisRecipe(axis);
        this.lengthProcessor = lengthProcessor;
        this.adjacent = adjacent;
    }
    
    protected CellBuilderState RunningState { get; }

    
    protected void OffsetBy(ILength length)
    {
        this.offset = length.Some();
    }


    protected void Setlength(ILength length)
    {
        this.lengthProcessor.SetLength(this.axisRecipe, GetPlacement().ToBase(), length);
    }


    
    protected Placement<IOptionalLength> GetPlacement()
    {
        return this.adjacent switch
            {
                NAdjacent.AlphaToOtherBeta => new(NCoordinate.Alpha,
                    _ => new SideOfPrevious(NCoordinate.Beta, this.offset)),
                NAdjacent.BetaToOtherAlpha => new(NCoordinate.Beta,
                    _ => new SideOfPrevious(NCoordinate.Alpha, this.offset)),
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
