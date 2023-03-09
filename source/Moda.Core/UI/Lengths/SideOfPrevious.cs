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

public class SideOfPrevious : Length, IOptionalLength
{
    private NCoordinate targetSide;
    private Option<ILength> offset;
    private Option<Coordinate> targetCoordinate;



    //##############################################################################################
    //
    //  Constructors
    //
    //##############################################################################################
    
    public SideOfPrevious(NCoordinate targetSide, Option<ILength> offset)
    {
        this.targetSide = targetSide;
        this.offset = offset;
    }


    //##############################################################################################
    //
    //  Fields
    //
    //##############################################################################################
    

    public IEnumerable<Coordinate> Prerequisites { get; private set; } = 
        Enumerable.Empty<Coordinate>();


    //##############################################################################################
    //
    //  Public Methods
    //
    //##############################################################################################
    

    public override Single Calculate()
    {
        return this.TryCalculate().ValueOrFailure();
    }


    public Option<Single> TryCalculate()
    {
        return this.targetCoordinate.Map(coordinate => this.offset.Match(
                off => coordinate.RelativeValue.ValueOrFailure() + off.Calculate(),
                () => coordinate.RelativeValue.ValueOrFailure()));
    }


    //##############################################################################################
    //
    //  Private methods
    //
    //##############################################################################################
    

    protected override void OnInitialize(Cell owner, Axis axis)
    {
        UpdateTargetCoordinate();
        SetupParent(Option.None<Cell>(), owner.Parent);
        owner.ParentChanged += (_, args) => SetupParent(args.OldValue, args.NewValue);
    }


    private void SetupParent(Option<Cell> oldParent, Option<Cell> newParent)
    {
        oldParent.MatchSome(p => p.ChildrenChanged -= ParentOnChildrenChanged);
        newParent.MatchSome(p => p.ChildrenChanged += ParentOnChildrenChanged);
    }


    private void ParentOnChildrenChanged(Cell sender, CollectionChangedArgs<Cell> args)
    {
        UpdateTargetCoordinate();
    }

    
    
    private void UpdateTargetCoordinate()
    {
        Option<Coordinate> newTarget = this.Owner.Parent.FlatMap(parent =>
            parent.Children.IndexOf(this.Owner)
                .Filter(a => a > 0)
                .Map(a => parent.Children[a - 1])
                .Map(peer => peer.GetBoundary(this.Axis).GetCoordinate(this.targetSide))
        );

        if (this.targetCoordinate != newTarget)
        {
            Option<Coordinate> oldTarget = this.targetCoordinate;
            this.targetCoordinate = newTarget;
            this.Prerequisites = this.targetCoordinate.YieldOrEmpty();
            RaisePrerequistesChanged( newTarget.YieldOrEmpty(), oldTarget.YieldOrEmpty());
        }
    }
    
}