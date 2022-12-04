// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.Utility.Data;
using Optional;
using Optional.Unsafe;

namespace Moda.Core.UI.Lengths;

public class SizeOfParent : Length
{
    private Option<Boundary> parentBoundary;
    
    public override Single Calculate()
    {
        return this.parentBoundary
            .FlatMap(a => a.RelativeRange.Map(b => b.Delta))
            .ValueOrFailure();
    }
    
    
    protected override void OnInitialize(Cell owner, Axis axis)
    {
        UpdateParentBoundary();
        ModifyPrerequisites(GetCoordinates(owner.Parent), Enumerable.Empty<Coordinate>());
        
        owner.ParentChanged += OwnerOnParentChanged;
    }

    
    private void OwnerOnParentChanged(Cell sender, ValueChangedArgs<Option<Cell>> args)
    {
        UpdateParentBoundary();
        ModifyPrerequisites(GetCoordinates(args.NewValue), GetCoordinates(args.OldValue));
    }


    private void UpdateParentBoundary()
    {
        this.parentBoundary = this.Axis.FlatMap(axis =>
            this.Owner.FlatMap(owner =>
                owner.Parent.Map(parent =>
                    parent.GetBoundary(axis))));
    }

    
    private IEnumerable<Coordinate> GetCoordinates(Option<Cell> cell) => cell.Match(
        a => a.GetBoundary(this.Axis.ValueOrFailure()).GetCoordinates(),
        Enumerable.Empty<Coordinate>);
    
}
