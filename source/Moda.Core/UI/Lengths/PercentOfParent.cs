// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.Utility.Data;
using Optional;
using Optional.Linq;

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
            from range in parent.GetBoundary(axis).RelativeRange
            select range.Delta * (this.Percent * 0.01f))
            .ValueOrFailure();
    }


    public override Option<Single> TryCalculate()
    {
        // TODO: better pattern for only calling after initialization? initialize vars being optional is annoying
        // TODO: wrap into intialize class and return as an optional whole?
        // TODO: failure to like...get the RelativeRange should not be considered optional...that's a bug in the prereqs
        // TODO: whats a better way to enforce that besides unit testing?
        // TODO: I don't like that TryCalculate has to spread virally to everything because of arithmetic
        return
            from parent in this.Owner.Parent
            from range in parent.GetBoundary(this.Axis).RelativeRange
            select range.Delta * (this.Percent * 0.01f);
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
