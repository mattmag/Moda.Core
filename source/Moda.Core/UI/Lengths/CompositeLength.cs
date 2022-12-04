// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.Utility.Data;

namespace Moda.Core.UI.Lengths;

public abstract class CompositeLength  : ILength
{
    //##############################################################################################
    //
    //  Events
    //
    //##############################################################################################
    
    public event NotificationHandler<ICalculation>? ValueInvalidated;
    public event CollectionChangedHandler<ICalculation, Coordinate>? PrerequisitesChanged;

    //##############################################################################################
    //
    //  Public Properties
    //
    //##############################################################################################
    
    private HashSet<ILength> _lengths = new();
    public IEnumerable<ILength> Lengths => this._lengths;
    
    
    private HashSet<Coordinate> _prerequisites = new();
    public IEnumerable<Coordinate> Prerequisites => this._prerequisites;


    //##############################################################################################
    //
    //  Public Methods
    //
    //##############################################################################################
    
    
    
    public abstract Single Calculate();


    protected void AddLength(ILength length)
    {
        if (this._lengths.Add(length))
        {
            SyncPrerequisites();
            length.ValueInvalidated += _ => this.ValueInvalidated?.Invoke(this);
            length.PrerequisitesChanged += LengthOnPrerequisitesChanged;
        }
    }


    //##############################################################################################
    //
    //  Private Methods
    //
    //##############################################################################################
    

    private void LengthOnPrerequisitesChanged(ICalculation sender,
        CollectionChangedArgs<Coordinate> args)
    {
        SyncPrerequisites();
    }


    private void SyncPrerequisites()
    {
        Coordinate[] newPrereqs = this._lengths.SelectMany(a => a.Prerequisites).Distinct()
            .ToArray();
        Coordinate[] actuallyRemoved = this._prerequisites.Except(newPrereqs).ToArray();
        Coordinate[] actuallyAdded = newPrereqs.Except(this._prerequisites).ToArray();

        if (actuallyRemoved.Any() || actuallyAdded.Any())
        {
            this._prerequisites = newPrereqs.ToHashSet();
            this.PrerequisitesChanged?.Invoke(this, new(actuallyAdded, actuallyRemoved));
        }
    }
}
