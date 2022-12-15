// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.UI.Builders;

public class VAnchorBuilder : NAnchorBuilder, IReadyForHeight
{
    public VAnchorBuilder(CellBuilderState runningState, ISetLengthProcessor lengthProcessor,
        VAnchor anchor) : base(runningState, lengthProcessor, anchor.ToNeutral())
    {
    }


    public IReadyForHeight OffsetBy(Length length)
    {
        base.SetOffset(length);
        return this;
    }


    public IComponentAssignerOrReadyToBuild WithHeight(Length width)
    {
        // apply and exit
        SetLength(width);
        return new ComponentAssigner(this.RunningState);
    }
}
