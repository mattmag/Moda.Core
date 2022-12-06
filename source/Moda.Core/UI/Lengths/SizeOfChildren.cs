// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.Utility.Data;
using Optional.Unsafe;
using Optional.Linq;



namespace Moda.Core.UI.Lengths;

public class SizeOfChildren : Length
{
    
    public override Single Calculate()
    {
        return (from owner in this.Owner
                from axis in this.Axis
            select owner.Children
                .Max(a => a.GetBoundary(axis).BetaCoordinate.RelativeValue.ValueOrFailure()))
            .ValueOrFailure();
    }


    protected override void OnInitialize(Cell owner, Axis axis)
    {
        ModifyPrerequisites(GetBetaCoordinates(owner.Children, axis),
            Enumerable.Empty<Coordinate>());
        owner.ChildrenChanged += OwnerOnChildrenChanged;
    }


    private IEnumerable<Coordinate> GetBetaCoordinates(IEnumerable<Cell> cells, Axis axis)
    {
        return cells.Select(a => a.GetBoundary(axis).BetaCoordinate);
    }

    private void OwnerOnChildrenChanged(Cell sender, CollectionChangedArgs<Cell> args)
    {
        this.Axis.MatchSome(axis =>
            ModifyPrerequisites(
                GetBetaCoordinates(args.ItemsAdded, axis),
                GetBetaCoordinates(args.ItemsRemoved, axis))
        );
    }
}

