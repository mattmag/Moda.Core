// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System.Diagnostics;
using Moda.Core.UI.Builders;
using Moda.Core.Utility.Data;
using Optional;

namespace Moda.Core.UI;

/// <summary>
///     The main component of all user interface entities, providing both size and location, and
///     it's relationship to other cells in the tree.
/// </summary>
/// <remarks>
///     A cell declares a rectangular area by it's <see cref="XBoundary"/> and
///     <see cref="YBoundary"/>.
/// </remarks>
[DebuggerDisplay("{DebugName}")]
public class Cell : TreeNode<Cell>
{
    private IHoneyComb hive;
    
    //##############################################################################################
    //
    //  Constructors
    //
    //##############################################################################################
    public Cell(IHoneyComb hive, ICalculation xAlpha, ICalculation xBeta, ICalculation yAlpha,
        ICalculation yBeta)
    {
        this.hive = hive;
        this.XBoundary = new(this, Axis.X, xAlpha, xBeta);
        this.XBoundary.AlphaCoordinate.AbsoluteValueChanged += UpdateChildrenXTare;
        this.YBoundary = new(this, Axis.Y, yAlpha, yBeta);
        this.YBoundary.AlphaCoordinate.AbsoluteValueChanged += UpdateChildrenYTare;
    }


    private void UpdateChildrenXTare(Coordinate sender, ValueChangedArgs<Option<Single>> args)
    {
        foreach (Cell child in this.Children)
        {
            child.XBoundary.AlphaCoordinate.Tare = args.NewValue;
            child.XBoundary.BetaCoordinate.Tare = args.NewValue;
        }
    }
    
     private void UpdateChildrenYTare(Coordinate sender, ValueChangedArgs<Option<Single>> args)
    {
        foreach (Cell child in this.Children)
        {
            child.YBoundary.AlphaCoordinate.Tare = args.NewValue;
            child.YBoundary.BetaCoordinate.Tare = args.NewValue;
        }
    }


    //##############################################################################################
    //
    //  Public Properties
    //
    //##############################################################################################

    /// <summary>
    ///     The <see cref="Coordinate">Coordinates</see> that define the bounds of the cell in the X
    ///     axis.
    /// </summary>
    public Boundary XBoundary { get; }
    
    /// <summary>
    ///     The <see cref="Coordinate">Coordinates</see> that define the bounds of the cell in the Y
    ///     axis.
    /// </summary>
    public Boundary YBoundary { get; }

    /// <summary>
    ///     The ID of the entity that owns this cell as a component.
    /// </summary>
    public UInt64 EntityID { get; internal set; }


    private string _debugName = string.Empty;
    public String DebugName
    {
        get => this._debugName;
        set
        {
            this._debugName = value;
            this.XBoundary.AlphaCoordinate.DebugName = $"{this._debugName}.X.Alpha";
            this.XBoundary.BetaCoordinate.DebugName = $"{this._debugName}.X.Beta";
            this.YBoundary.AlphaCoordinate.DebugName = $"{this._debugName}.Y.Alpha";
            this.YBoundary.BetaCoordinate.DebugName = $"{this._debugName}.Y.Beta";
        }
    }
    

    //##############################################################################################
    //
    //  Public Methods
    //
    //##############################################################################################
    
    
    /// <summary>
    ///     Enumerate through the alpha and beta coordinates of both the X and Y boundary.
    /// </summary>
    /// <returns>
    ///     An <see cref="IEnumerable{T}"/> of each coordinate in the cell.
    /// </returns>
    public IEnumerable<Coordinate> GetCoordinates()
    {
        return this.XBoundary.GetCoordinates().Concat(this.YBoundary.GetCoordinates());
    }


    public Boundary GetBoundary(Axis axis)
    {
        return axis switch
            {
                Axis.X => this.XBoundary,
                Axis.Y => this.YBoundary,
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    // public void Kill()
    // {
    //     this.hive.KillCell(this);
    // }


    public Cell CreateChild(Func<IParentOrganizer, IReadyToBuild> builder)
    {
        return this.hive.NewCell(a => builder(a.AssignParent(this)));
    }

    //##############################################################################################
    //
    //  Private Methods
    //
    //##############################################################################################

    /// <inheritdoc/>
    protected override void OnChildAdded(Cell child)
    {
        child.XBoundary.AlphaCoordinate.Tare = this.XBoundary.AlphaCoordinate.AbsoluteValue;
        child.XBoundary.BetaCoordinate.Tare = this.XBoundary.AlphaCoordinate.AbsoluteValue;
        child.YBoundary.AlphaCoordinate.Tare = this.YBoundary.AlphaCoordinate.AbsoluteValue;
        child.YBoundary.BetaCoordinate.Tare = this.YBoundary.AlphaCoordinate.AbsoluteValue;
    }

    /// <inheritdoc/>
    protected override void OnChildRemoved(Cell child)
    {
        child.XBoundary.AlphaCoordinate.Tare = Option.None<Single>();
        child.XBoundary.BetaCoordinate.Tare = Option.None<Single>();
        child.YBoundary.AlphaCoordinate.Tare = Option.None<Single>();
        child.YBoundary.BetaCoordinate.Tare = Option.None<Single>();
    }


    
}
