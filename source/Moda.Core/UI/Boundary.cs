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
    public Boundary(Cell owningCell, Axis axis, ICalculation alphaCalculation,
        ICalculation betaCalculation)
    {
        this.AlphaCoordinate = new(owningCell, axis, alphaCalculation);
        this.AlphaCoordinate.RelativeValueChanged += (_, _) => UpdateSecondaryRelativeValues();
        this.AlphaCoordinate.AbsoluteValueChanged += (_, _) => UpdateSecondaryAbsoluteValues();

        this.BetaCoordinate = new(owningCell, axis, betaCalculation);
        this.BetaCoordinate.RelativeValueChanged += (_, _) => UpdateSecondaryRelativeValues();
        this.BetaCoordinate.AbsoluteValueChanged += (_, _) => UpdateSecondaryAbsoluteValues();
    }


    //##############################################################################################
    //
    //  Public Properties
    //
    //##############################################################################################
    
    /// <summary>
    ///     The minimum value of the boundary along it's axis (Left if X, Up if Y).
    /// </summary>
    public Coordinate AlphaCoordinate { get; }
    
    /// <summary>
    ///     The maximum value of the boundary along it's axis  (Right if X, Down if Y).
    /// </summary>
    public Coordinate BetaCoordinate { get; }
    
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
    //  Public Methods
    //
    //##############################################################################################
    
    /// <summary>
    ///     Provides an easy way to iterate both <see cref="AlphaCoordinate"/> and
    ///     <see cref="BetaCoordinate"/>.
    /// </summary>
    /// <returns>
    ///     The <see cref="AlphaCoordinate"/> and <see cref="BetaCoordinate"/> as an
    ///     <see cref="IEnumerable{T}"/>
    /// </returns>
    public IEnumerable<Coordinate> GetCoordinates()
    {
        yield return this.AlphaCoordinate;
        yield return this.BetaCoordinate;
    }

    /// <summary>
    ///     Get the specified coordinate.
    /// </summary>
    /// <param name="coordinate">
    ///     The coordinate of interest.
    /// </param>
    /// <returns>
    ///     <see cref="AlphaCoordinate"/> when <paramref name="coordinate"/> is
    ///     <see cref="NCoordinate.Alpha"/>, <see cref="BetaCoordinate"/> when
    ///     <paramref name="coordinate"/> is <see cref="NCoordinate.Beta"/>
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     When an unknown value is passed to the <paramref name="coordinate"/> parameter. 
    /// </exception>
    public Coordinate GetCoordinate(NCoordinate coordinate) => coordinate switch
        {
            NCoordinate.Alpha => this.AlphaCoordinate,
            NCoordinate.Beta => this.BetaCoordinate,
            _ => throw new ArgumentOutOfRangeException(nameof(coordinate), coordinate, null)
        };


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
