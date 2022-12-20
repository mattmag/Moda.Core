// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.UI.Builders;
using Moda.Core.Utility.Data;
using Optional;
using Optional.Linq;
using Optional.Unsafe;

namespace Moda.Core.UI.Lengths;

public class SideOfPrevious : Length, IOptionalLength
{
    private NCoordinate side;
    private readonly Option<ILength> offset;


 
    public SideOfPrevious(NCoordinate side, Option<ILength> offset)
    {
        this.side = side;
        this.offset = offset;
    }
    
    
    
    public override Single Calculate()
    {
        return this.TryCalculate().ValueOrFailure();
    }


    public Option<Single> TryCalculate()
    {
        return from peer in this.GetPeer()
            select this.offset.Match(
                off => GetRelevantCoordinate(peer) + off.Calculate(),
                () => GetRelevantCoordinate(peer));
    }


    private Single GetRelevantCoordinate(Cell peer)
    {
        Boundary boundary = peer.GetBoundary(this.Axis);
        return this.side switch
            {
                NCoordinate.Alpha => boundary.AlphaCoordinate.RelativeValue.ValueOrFailure(),
                NCoordinate.Beta => boundary.BetaCoordinate.RelativeValue.ValueOrFailure(),
                _ => throw new ArgumentOutOfRangeException()
            };
    }
    
    private Option<Cell> GetPeer()
    {
        // TODO: find and store peer during init and children chages for performance
        // TODO: better exceptions than OptionValueMissingException ? can't inherit though...
        IReadOnlyList<Cell> peers = this.Owner.Parent.ValueOrFailure().Children;
        return peers
            .IndexOf(this.Owner)
            .Filter(a => a > 0)
            .Map(a => peers[a - 1]);
    }


    
}