// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

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
public class Cell : TreeNode<Cell>
{
    //##############################################################################################
    //
    //  Fields
    //
    //##############################################################################################
    
    
    
    //##############################################################################################
    //
    //  Public Properties
    //
    //##############################################################################################

    /// <summary>
    ///     The <see cref="Coordinate">Coordinates</see> that define the bounds of the cell in the X
    ///     axis.
    /// </summary>
    public Boundary XBoundary { get; } = new();
    
    /// <summary>
    ///     The <see cref="Coordinate">Coordinates</see> that define the bounds of the cell in the Y
    ///     axis.
    /// </summary>
    public Boundary YBoundary { get; } = new();

    /// <summary>
    ///     The ID of the entity that owns this cell as a component.
    /// </summary>
    public Option<UInt64> Entity { get; set; }

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
