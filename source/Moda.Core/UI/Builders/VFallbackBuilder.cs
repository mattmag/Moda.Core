// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.UI.Lengths;

namespace Moda.Core.UI.Builders;

public class VFallbackBuilder : IVAxisInitializer
{
    private readonly Placement<IOptionalLength> preferred;
    private readonly CellBuilderState runningState;
    
    public VFallbackBuilder(CellBuilderState runningState, Placement<IOptionalLength> preferred)
    {
        this.preferred = preferred;
        this.runningState = runningState;
    }
    
    public VAdjacencyBuilder AbovePrevious() => ToAdjacency(VAdjacent.Above);


    public VAdjacencyBuilder BelowPrevious() => ToAdjacency(VAdjacent.Below);


    public VAnchorBuilder AnchorUp() => ToAnchorBuilder(VAnchor.Up);


    public VAnchorBuilder AnchorMiddle() => ToAnchorBuilder(VAnchor.Middle);


    public VAnchorBuilder AnchorDown() => ToAnchorBuilder(VAnchor.Down);
    
    
    private VAdjacencyBuilder ToAdjacency(VAdjacent adjacency) =>
        new(this.runningState, new FallbackLengthProcessor(this.preferred), adjacency);


    private VAnchorBuilder ToAnchorBuilder(VAnchor anchor) =>
        new(this.runningState, new FallbackLengthProcessor(this.preferred), anchor);
}
