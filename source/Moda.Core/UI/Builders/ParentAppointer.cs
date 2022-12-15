// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.Utility.Data;
using Optional;
using Optional.Unsafe;

namespace Moda.Core.UI.Builders;

public class ParentAppointer : IParentAssigner, IParentAssigned, IParentOrganizer
{
    private readonly CellBuilderState runningState = new();
    
    
    //##############################################################################################
    //
    //  Public Methods
    //
    //##############################################################################################


    public IParentAssigned AssignParent(Cell parent)
    {
        this.runningState.Composition.Parent.Set(parent);
        return this;
    }
    
    
    // IParentAssigner
    //----------------------------------------------------------------------------------------------

    BoundariesInitializer IParentAssigner.AppendTo(Cell parent)
    {
        this.runningState.Composition.Parent.Set(parent);
        return ((IParentOrganizer)this).Append();
    }


    BoundariesInitializer IParentAssigner.InsertAt(Cell parent, Int32 index)
    {
        this.runningState.Composition.Parent.Set(parent);
        return ((IParentOrganizer)this).InsertAt(index);
    }

    BoundariesInitializer IParentAssigner.InsertBefore(Cell peer)
    {
        this.runningState.Composition.Parent.Set(peer.Parent.ValueOrFailure());
        return ((IParentOrganizer)this).InsertBefore(peer);
    }


    BoundariesInitializer IParentAssigner.InsertAfter(Cell peer)
    {
        this.runningState.Composition.Parent.Set(peer.Parent.ValueOrFailure());
        return ((IParentOrganizer)this).InsertAfter(peer);
    }

    // IParentOrganizer
    //----------------------------------------------------------------------------------------------


    BoundariesInitializer IParentOrganizer.Append()
    {
        this.runningState.Composition.InsertionIndex.Set(Option.None<Int32>());
        return ToBoundariesInitializer();
    }

    BoundariesInitializer IParentOrganizer.InsertAt(Int32 index)
    {
        this.runningState.Composition.InsertionIndex.Set(index.Some());
        return ToBoundariesInitializer();
    }


    BoundariesInitializer IParentOrganizer.InsertBefore(Cell peer)
    {
        this.runningState.Composition.InsertionIndex.Set(this.runningState.Composition.Parent.Get()
            .Children.IndexOf(peer).OrFailure());
        return ToBoundariesInitializer();
    }


    BoundariesInitializer IParentOrganizer.InsertAfter(Cell peer)
    {
        this.runningState.Composition.InsertionIndex.Set(this.runningState.Composition.Parent.Get()
            .Children.IndexOf(peer).Map(i => i + 1).OrFailure());
        return ToBoundariesInitializer();
    }


    //##############################################################################################
    //
    //  Private Methods
    //
    //##############################################################################################


    private BoundariesInitializer ToBoundariesInitializer()
    {
        return new(this.runningState);
    }
}
