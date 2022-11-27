// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.Utility.Data;
using Optional;
using Optional.Unsafe;

namespace Moda.Core.UI.Builders;


public class CellBuilder : IParentAssigner, IParentAssigned, IParentOrganizer,
    IInitializeHorizontalAxis, IInitializeVerticalAxis,
    IMinmimumViableCell, IComponentAssigner, IReadyToBuild
    
{
    //##############################################################################################
    //
    //  Fields
    //
    //##############################################################################################
    
    private BoundariesRecipe boundariesRecipe = new();
    private CompositionRecipe compositionRecipe = new();


    //##############################################################################################
    //
    //  Constructors
    //
    //##############################################################################################
    

    
    //##############################################################################################
    //
    //  Public Properties
    //
    //##############################################################################################

    public BoundariesRecipe BoundariesRecipe => this.boundariesRecipe;

    public CompositionRecipe CompositionRecipe => this.compositionRecipe;

    //##############################################################################################
    //
    //  Public Methods
    //
    //##############################################################################################


    public IParentAssigned AssignParent(Cell parent)
    {
        this.CompositionRecipe.Parent.Set(parent);
        return this;
    }
    
    // Axis Initializers
    //----------------------------------------------------------------------------------------------
    
    public AnchoredHorizontalBuilder AnchorAt(Horizontal anchor) => new(this, anchor);
    
    public AnchoredVerticalBuilder AnchorAt(Vertical anchor) => new(this, anchor);
    

    // IParentAssigner
    //----------------------------------------------------------------------------------------------

    IInitializeHorizontalAxis IParentAssigner.AppendTo(Cell parent)
    {
        this.CompositionRecipe.Parent.Set(parent);
        this.CompositionRecipe.InsertionIndex.Set(Option.None<Int32>());
        return this;
    }


    IInitializeHorizontalAxis IParentAssigner.InsertAt(Cell parent, Int32 index)
    {
        this.CompositionRecipe.Parent.Set(parent);
        return ((IParentOrganizer)this).InsertAt(index);
    }

    IInitializeHorizontalAxis IParentAssigner.InsertBefore(Cell peer)
    {
        this.CompositionRecipe.Parent.Set(peer.Parent.ValueOrFailure());
        return ((IParentOrganizer)this).InsertBefore(peer);
    }


    IInitializeHorizontalAxis IParentAssigner.InsertAfter(Cell peer)
    {
        this.CompositionRecipe.Parent.Set(peer.Parent.ValueOrFailure());
        return ((IParentOrganizer)this).InsertAfter(peer);
    }

    // IParentOrganizer
    //----------------------------------------------------------------------------------------------
    

    IInitializeHorizontalAxis IParentOrganizer.InsertAt(Int32 index)
    {
        this.CompositionRecipe.InsertionIndex.Set(index.Some());
        return this;
    }


    IInitializeHorizontalAxis IParentOrganizer.InsertBefore(Cell peer)
    {
        this.CompositionRecipe.InsertionIndex.Set(this.CompositionRecipe.Parent.Get()
            .Children.IndexOf(peer).OrFailure());
        return this;
    }


    IInitializeHorizontalAxis IParentOrganizer.InsertAfter(Cell peer)
    {
        this.CompositionRecipe.InsertionIndex.Set(this.CompositionRecipe.Parent.Get()
            .Children.IndexOf(peer).Map(i => i + 1).OrFailure());
        return this;
    }


    // IComponentComposer
    //----------------------------------------------------------------------------------------------
    
    public IReadyToBuild WithComponents(params Object[] objects)
    {
        return WithComponents((IEnumerable<Object>)objects);
    }


    public IReadyToBuild WithComponents(IEnumerable<Object> objects)
    {
        this.CompositionRecipe.Components.Set(objects);
        return this;
    }


    // IReadyToBuild
    //----------------------------------------------------------------------------------------------
    
    public CellRecipe GetRecipe()
    {
        return new(this.BoundariesRecipe, this.CompositionRecipe);
    }
}
