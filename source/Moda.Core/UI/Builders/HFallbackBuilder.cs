// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.UI.Lengths;

namespace Moda.Core.UI.Builders;

public class HFallbackBuilder : IHAxisInitializer
{
    private readonly Placement<IOptionalLength> preferred;
    private readonly CellBuilderState runningState;


    public HFallbackBuilder(CellBuilderState runningState, Placement<IOptionalLength> preferred)
    {
        this.preferred = preferred;
        this.runningState = runningState;
    }


    public HAdjacencyBuilder LeftOfPrevious() => ToAdjacency(HAdjacent.LeftOf);

    public HAdjacencyBuilder RightOfPrevious() => ToAdjacency(HAdjacent.RightOf);


    public HAnchorBuilder AnchorLeft() => ToAnchorBuilder(HAnchor.Left);

    public HAnchorBuilder AnchorCenter() => ToAnchorBuilder(HAnchor.Center);

    public HAnchorBuilder AnchorRight() => ToAnchorBuilder(HAnchor.Right);


    private HAdjacencyBuilder ToAdjacency(HAdjacent adjacency) =>
        new(this.runningState, new FallbackLengthProcessor(this.preferred), adjacency);


    private HAnchorBuilder ToAnchorBuilder(HAnchor anchor) =>
        new(this.runningState, new FallbackLengthProcessor(this.preferred), anchor);

}
