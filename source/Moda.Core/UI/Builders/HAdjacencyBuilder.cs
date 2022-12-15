// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.UI.Builders;

public class HAdjacencyBuilder : NAdjacencyBuilder, IReadyForWidthOrFallback
{
    public HAdjacencyBuilder(CellBuilderState runningState, ISetLengthProcessor lengthProcessor,
        HAdjacent adjacent) : base(runningState, lengthProcessor, adjacent.ToNeutral(), Axis.X)
    {
    }
      
    public IReadyForWidthOrFallback OffsetBy(Length offset)
    {
        base.OffsetBy(offset);
        return this;
    }
    
    
    public IVAxisInitializer WithWidth(Length width)
    {
        // apply and exit
        base.Setlength(width);
        return new VAxisInitializer(this.RunningState);
    }
    
    
    public HFallbackBuilder Or()
    {
        // hand off
        return new(this.RunningState, GetPlacement());
    }
  
}