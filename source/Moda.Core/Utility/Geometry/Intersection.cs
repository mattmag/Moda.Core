// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.Utility.Maths;

namespace Moda.Core.Utility.Geometry;

/// <summary>
///     Describes an intersection between two line segments.
/// </summary>
public abstract record Intersection
{
    /// <summary>
    ///     Get the first value of the intersection.  For a simple <see cref="Intersects"/>, this
    ///     is the <see cref="Intersects.At"/> value.  For a collinear <see cref="Overlaps"/>, it is
    ///     the lesser value of the <see cref="Overlaps.Range"/>.
    /// </summary>
    /// <returns></returns>
    public abstract Single FirstImpact();
        
    /// <summary>
    ///     Indicates that an intersection occurred at distance (as a scalar, 0-1) along the line
    ///     segment.
    /// </summary>
    /// <param name="At">
    ///     The distance, as a scalar (0-1) along the line segment that the intersection occurred.
    /// </param>
    public record Intersects(Single At) : Intersection
    {
        /// <inheritdoc/>
        public override Single FirstImpact() => this.At;
    }


    /// <summary>
    ///     Indicates that the line segments were collinear and overlapped, instead of a simple 
    ///     <see cref="Intersection.Intersects"/>.
    /// </summary>
    /// <param name="Range">
    ///     The positions along the line segments as scalars (0-1) where the other line segment is
    ///     overlapping the first.
    /// </param>
    public record Overlaps(RangeF Range) : Intersection
    {
        /// <inheritdoc/>
        public override Single FirstImpact() => this.Range.Minimum;
    }
}
