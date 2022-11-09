// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.Utility.Maths;
using Optional;

namespace Moda.Core.UI;

/// <summary>
///     Contains the <see cref="Coordinate">Coordinates</see> that mark the boundary along an axis
///     that a define a <see cref="Cell"/>.
/// </summary>
public class Boundary
{
    //##############################################################################################
    //
    //  Constructors
    //
    //##############################################################################################
    
    /// <summary>
    ///     Initializes a new <see cref="Boundary"/> instance.
    /// </summary>
    public Boundary()
    {
        this.AlphaCoordinate.RelativeValueChanged += (_, _) => UpdateSecondaryRelativeValues();
        this.BetaCoordinate.RelativeValueChanged += (_, _) => UpdateSecondaryRelativeValues();
        this.AlphaCoordinate.AbsoluteValueChanged += (_, _) => UpdateSecondaryAbsoluteValues();
        this.BetaCoordinate.AbsoluteValueChanged += (_, _) => UpdateSecondaryAbsoluteValues();
    }


    //##############################################################################################
    //
    //  Public Properties
    //
    //##############################################################################################


    /// <summary>
    ///     The minimum value of the boundary along it's axis (Left if X, Top if Y).
    /// </summary>
    public Coordinate AlphaCoordinate { get; } = new();
    
    /// <summary>
    ///     The maximum value of the boundary along it's axis  (Right if X, Bottom if Y).
    /// </summary>
    public Coordinate BetaCoordinate { get; } = new();
    
    /// <summary>
    ///     The <see cref="Coordinate.AbsoluteValue">Absolute Values</see>
    ///     <see cref="AlphaCoordinate"/> and <see cref="BetaCoordinate"/> as a 
    ///     <see cref="RangeF"/>.
    /// </summary>
    public Option<RangeF> AbsoluteRange { get; private set; }
    
    /// <summary>
    ///     The <see cref="Coordinate.RelativeValue">Absolute Values</see>
    ///     <see cref="AlphaCoordinate"/> and <see cref="BetaCoordinate"/> as a 
    ///     <see cref="RangeF"/>.
    /// </summary>
    public Option<RangeF> RelativeRange { get; private set; }

    //##############################################################################################
    //
    //  Private Methods
    //
    //##############################################################################################


    private void UpdateSecondaryRelativeValues()
    {
        this.RelativeRange = this.AlphaCoordinate.RelativeValue.FlatMap(
            alpha => this.BetaCoordinate.RelativeValue.Map(beta => new RangeF(alpha, beta)));
    }
    
    private void UpdateSecondaryAbsoluteValues()
    {
        this.AbsoluteRange = this.AlphaCoordinate.AbsoluteValue.FlatMap(
            alpha => this.BetaCoordinate.AbsoluteValue.Map(beta => new RangeF(alpha, beta)));
    }
}
