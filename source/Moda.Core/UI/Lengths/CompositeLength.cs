// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.Utility.Data;

namespace Moda.Core.UI.Lengths;

public abstract class CompositeLength  : Length
{

    

    //##############################################################################################
    //
    //  Public Properties
    //
    //##############################################################################################
    
    private HashSet<Length> _lengths = new();
    public IEnumerable<Length> Lengths => this._lengths;
 
    

    //##############################################################################################
    //
    //  Public Methods
    //
    //##############################################################################################
    
    

    //##############################################################################################
    //
    //  Private Methods
    //
    //##############################################################################################
    
    protected override void OnInitialize(Cell owner, Axis axis)
    {
        foreach (Length length in this.Lengths)
        {
            length.Initialize(owner, axis);
        }
    }

    protected void AddLength(Length length)
    {
        if (this._lengths.Add(length))
        {
            SyncPrerequisites();
            this.Owner.MatchSome(owner => this.Axis.MatchSome(axis =>
                length.Initialize(owner, axis)));
            length.ValueInvalidated += _ => RaiseValueInvalidated();
            length.PrerequisitesChanged += LengthOnPrerequisitesChanged;
        }
    }


    private void LengthOnPrerequisitesChanged(ICalculation sender,
        CollectionChangedArgs<Coordinate> args)
    {
        SyncPrerequisites();
    }


    private void SyncPrerequisites()
    {
        Coordinate[] newPrereqs = this._lengths.SelectMany(a => a.Prerequisites)
            .Distinct().ToArray();
        Coordinate[] actuallyRemoved = this.Prerequisites.Except(newPrereqs).ToArray();
        Coordinate[] actuallyAdded = newPrereqs.Except(this.Prerequisites).ToArray();

        ModifyPrerequisites(actuallyAdded, actuallyRemoved);
    }
}
