// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.Utility.Data;
using Optional;
using Optional.Linq;
using Optional.Unsafe;

namespace Moda.Core.UI.Lengths;

public class PercentOfParent : Length
{

    public PercentOfParent(Single percent)
    {
        this.Percent = percent;
    }
    
    
    private Single _percent;
    public Single Percent
    {
        get => this._percent;
        set
        {
            if (this._percent != value)
            {
                this._percent = value;
                RaiseValueInvalidated();
            }
        }
    }
    
    
    public override Single Calculate()
    {
        return (from owner in this.Owner
            from parent in owner.Parent
            from axis in this.Axis
            from range in  parent.GetBoundary(axis).RelativeRange
            select range.Delta * (this.Percent * 0.01f))
            .ValueOrFailure();
    }
    
    
    protected override void OnInitialize(Cell owner, Axis axis)
    {
        ModifyPrerequisites(GetAxisCoordinates(owner.Parent), Enumerable.Empty<Coordinate>());
        
        owner.ParentChanged += OwnerOnParentChanged;
    }

    
    private void OwnerOnParentChanged(Cell sender, ValueChangedArgs<Option<Cell>> args)
    {
        ModifyPrerequisites(GetAxisCoordinates(args.NewValue), GetAxisCoordinates(args.OldValue));
    }



    private IEnumerable<Coordinate> GetAxisCoordinates(Option<Cell> cell) => (from c in cell
            from axis in this.Axis
            select c.GetBoundary(axis).GetCoordinates())
        .ValueOr(Enumerable.Empty<Coordinate>());

}
