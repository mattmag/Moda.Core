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

public class AdjacentToPrevious : Length, IOptionalLength
{
    private NAdjacent side;
    private readonly Option<ILength> offset;


    public AdjacentToPrevious(NAdjacent side)
    {
        this.side = side;
    }
    
    public AdjacentToPrevious(NAdjacent side, Option<ILength> offset)
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
        return (from peer in this.GetPeer()
            select GetRelevantCoordinate(peer));
    }


    private Single GetRelevantCoordinate(Cell peer)
    {
        Boundary boundary = peer.GetBoundary(this.Axis);
        return this.side switch
            {
                NAdjacent.AlphaToOtherBeta => boundary.AlphaCoordinate.RelativeValue.ValueOrFailure(),
                NAdjacent.BetaToOtherAlpha => boundary.BetaCoordinate.RelativeValue.ValueOrFailure(),
                _ => throw new ArgumentOutOfRangeException()
            };
    }
    
    private Option<Cell> GetPeer()
    {
        // TODO: find and store peer during init and children chages for performance
        IReadOnlyList<Cell> peers = this.Owner.Parent.ValueOrFailure().Children;
        return peers
            .IndexOf(this.Owner)
            .Filter(a => a > 0)
            .Map(a => peers[a - 1]);
    }


    
}