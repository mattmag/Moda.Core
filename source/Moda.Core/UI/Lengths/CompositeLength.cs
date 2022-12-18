// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.Utility.Data;
using Optional;

namespace Moda.Core.UI.Lengths;


//
// public class LengthCollection
// {
//     private HashSet<Length> _lengths = new();
//     public IEnumerable<Length> Lengths => this._lengths;
//
//
//     public void Initialize(Cell owner, Axis axis)
//     {
//         foreach (Length length in this.Lengths)
//         {
//             length.Initialize(owner, axis);
//         }
//     }
//     
//     protected void AddLength(Length length)
//     {
//         if (this._lengths.Add(length))
//         {
//             SyncPrerequisites();
//             if (this.IsInitialized)
//             {
//                 length.Initialize(this.Owner, this.Axis);
//             }
//             length.ValueInvalidated += _ => RaiseValueInvalidated();
//             length.PrerequisitesChanged += LengthOnPrerequisitesChanged;
//         }
//     }
//
//
//     public Option<Axis> Axis { get; set; }
//     public Option<Cell> Owner { get; set; }
//     
//     
//     private void SyncPrerequisites()
//     {
//         Coordinate[] newPrereqs = this._lengths.SelectMany(a => a.Prerequisites)
//             .Distinct().ToArray();
//         Coordinate[] actuallyRemoved = this.Prerequisites.Except(newPrereqs).ToArray();
//         Coordinate[] actuallyAdded = newPrereqs.Except(this.Prerequisites).ToArray();
//
//         ModifyPrerequisites(actuallyAdded, actuallyRemoved);
//     }
// }


// TODO: composition over inheritance? CompositeLengthManager or something
// TODO: really worked well as a composite length...kind of annoying
// TODO: doing this to avoid multiple inheritance with OptionalLength
// TODO: can we bail on all of it? can length go back to being an interface? can optionallength be one too?
// TODO: can we do composition for everything else like composite length, prereqs, etc?
// TODO: can we have a single PrerequisiteManager that combines the purpose of CompositeLength and the ModifyPrerequisites base method? 
// TODO: or: can all lengths effectively be a composite length? this class has almost no bearing on the public API
public abstract class CompositeLength : Length
{

    

    //##############################################################################################
    //
    //  Public Properties
    //
    //##############################################################################################
    
    private HashSet<ILength> _lengths = new();
    public IEnumerable<ILength> Lengths => this._lengths;
 
    

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
        foreach (ILength length in this.Lengths)
        {
            length.Initialize(owner, axis);
        }
    }

    protected void AddLength(ILength length)
    {
        if (this._lengths.Add(length))
        {
            SyncPrerequisites();
            if (this.IsInitialized)
            {
                length.Initialize(this.Owner, this.Axis);
            }
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
