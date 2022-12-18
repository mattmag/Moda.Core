// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.UI.Builders;

public class VAdjacencyBuilder : NAdjacencyBuilder, IReadyForHeightOrFallback
{
    public VAdjacencyBuilder(CellBuilderState runningState, ISetLengthProcessor lengthProcessor,
        VAdjacent adjacent) : base(runningState, lengthProcessor, adjacent.ToNeutral(), Axis.Y)
    {
    }
      
    public IReadyForHeightOrFallback OffsetBy(ILength offset)
    {
        base.OffsetBy(offset);
        return this;
    }
    
    
    public IComponentAssignerOrReadyToBuild WithHeight(ILength height)
    {
        // apply and exit
        base.Setlength(height);
        return new ComponentAssigner(this.RunningState);
    }
    
    
    public VFallbackBuilder Or()
    {
        // hand off
        return new(this.RunningState, GetPlacement());
    }
  
}