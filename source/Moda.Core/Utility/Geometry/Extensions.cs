// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System.Numerics;

namespace Moda.Core.Utility.Geometry;

/// <summary>
///     Adds useful methods to various geometry-related types
/// </summary>
public static class Extensions
{
    // Vectors
    //----------------------------------------------------------------------------------------------
    
    /// <summary>
    ///     Calculate the cross product (a x b) of the two vectors.
    /// </summary>
    /// <param name="a">
    ///     The first vector.
    /// </param>
    /// <param name="b">
    ///     The second vector.
    /// </param>
    /// <returns>
    ///     The cross product of the two vectors, as a scalar.
    /// </returns>
    /// <remarks>
    ///     Traditionally, cross product is an operation done on a 3D vector.  Assuming that the
    ///     cross product of two 2D vectors is the same as two 3D vectors with 0 as their Z
    ///     component, the resulting 3D vector would only have a Z component. Here we return that Z
    ///     component as a scalar. 
    /// </remarks>
    public static Single CrossProduct(this Vector2 a, Vector2 b)
    {
        return (a.X * b.Y) - (a.Y * b.X);
    }


    // Shapes
    //----------------------------------------------------------------------------------------------
    
    /// <summary>
    ///    Returns the opposite of this side of the rectangle.
    /// </summary>
    /// <param name="side">
    ///     This side of the rectangle.
    /// </param>
    /// <returns>
    ///     The opposite of this side of the rectangle
    /// </returns>
    public static Side Opposite(this Side side)
    {
        return side switch
        {
            Side.Left => Side.Right,
            Side.Right => Side.Left,
            Side.Top => Side.Bottom,
            Side.Bottom => Side.Top,
            _ => throw new ArgumentException(),
        };
    }
}
