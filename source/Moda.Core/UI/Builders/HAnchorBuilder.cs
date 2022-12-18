// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.UI.Builders;



public class HAnchorBuilder : NAnchorBuilder, IReadyForWidth
{

    public HAnchorBuilder(CellBuilderState runningState, ISetLengthProcessor lengthProcessor,
        HAnchor anchor) : base(runningState, lengthProcessor, anchor.ToNeutral(), Axis.X)
    {

    }

    public IReadyForWidth OffsetBy(ILength length)
    {
        base.SetOffset(length);
        return this;
    }


    public IVAxisInitializer WithWidth(ILength width)
    {
        // apply and exit
        SetLength(width);
        return new VAxisInitializer(this.RunningState);
    }
    

}
