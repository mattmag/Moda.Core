// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System.Numerics;
using Moda.Core.Utility.Data;
using Moda.Core.Utility.Maths;
using Optional;

namespace Moda.Core.Utility.Geometry;

/// <summary>
///     A line segment defined by two endpoints. Current implementation is directed from
///     <see cref="Point"/> to <see cref="End"/>.
/// </summary>
public readonly struct LineSegment : IEquatable<LineSegment>
{

    //##############################################################################################
    //
    //  Constructors, Factory Methods
    //
    //##############################################################################################
    
    /// <summary>
    ///     Creates a new <see cref="LineSegment"/> instance from the provided starting point and
    ///     the delta from that point until the end.
    /// </summary>
    /// <param name="point">
    ///     The origin point of the line segment.
    /// </param>
    /// <param name="delta">
    ///     The difference vector between the end and start points.
    /// </param>
    /// <returns>
    ///     An initialized <see cref="LineSegment"/> instance.
    /// </returns>
    public static LineSegment FromPointWithDelta(Vector2 point, Vector2 delta)
    {
        return new(point, delta, point + delta);
    }


    /// <summary>
    ///     Creates a new <see cref="LineSegment"/> instance from the provided starting point and
    ///     end point.
    /// </summary>
    /// <param name="point">
    ///     The origin point of the line segment.
    /// </param>
    /// <param name="end">
    ///     The end point of the line segment.
    /// </param>
    /// <returns>
    ///     An initialized <see cref="LineSegment"/> instance.
    /// </returns>
    /// <remarks>
    ///     Same functionality as the <see cref="LineSegment(Vector2, Vector2)"/>, providing an
    ///     optional syntax to match <see cref="FromPointWithDelta"/>.
    /// </remarks>
    public static LineSegment FromPointToEnd(Vector2 point, Vector2 end)
    {
        return new(point, end - point, end);
    }
    
    /// <summary>
    ///     Initialize a new <see cref="LineSegment"/> instance.
    /// </summary>
    /// <param name="point">
    ///     The origin point of the line segment.
    /// </param>
    /// <param name="end">
    ///     The end point of the line segment.
    /// </param>
    public LineSegment(Vector2 point, Vector2 end) : this(point, end - point, end)
    {
        
    }
    
    /// <summary>
    ///     Initialize a new <see cref="LineSegment"/> instance.
    /// </summary>
    /// <param name="point">
    ///     The origin point of the line segment.
    /// </param>
    /// <param name="delta">
    ///     The difference vector between the end and start points.
    /// </param>
    /// <param name="end">
    ///     The end point of the line segment.
    /// </param>
    private LineSegment(Vector2 point, Vector2 delta, Vector2 end)
    {
        this.Point = point;
        this.Delta = delta;
        this.End = end;
    }
    

    //##############################################################################################
    //
    //  Public Properties
    //
    //##############################################################################################
    

    /// <summary>
    ///     The origin of the line segment.
    /// </summary>
    public Vector2 Point
    {
        get;
    }

    /// <summary>
    ///     The difference vector between the end and start points.
    /// </summary>
    public Vector2 Delta
    {
        get;
    }

    /// <summary>
    ///     The end of the line segment.
    /// </summary>
    /// <remarks>
    ///     Defined as <see cref="Point"/> + <see cref="Delta"/>
    /// </remarks>
    public Vector2 End
    {
        get;
    }


    //##############################################################################################
    //
    //  Public Methods
    //
    //##############################################################################################
    
    /// <summary>
    ///     Check if this <see cref="LineSegment"/> intersects another.
    /// </summary>
    /// <param name="other">
    ///     The other <see cref="LineSegment"/>
    /// </param>
    /// <returns>
    ///     An <see cref="Intersection"/> instance with this line segment instance as the frame of
    ///     reference, or <see cref="Option.None{T}"/> if no collision occured.
    /// </returns>
    /// <remarks> 
    ///     This method was originally designed to check movement vectors for collision against
    ///     bounding box sides of a Minkowski sum, though it can serve as a general purpose
    ///     intersection test.
    /// 
    ///     Shout out to
    ///     <a href="https://stackoverflow.com/users/68063/gareth-rees">Gareth Rees</a>
    ///     for their clear explanation and math describing line segment
    ///     intersections in this stack overflow answer: https://stackoverflow.com/a/565282/1724034
    /// </remarks>
    public Option<Intersection> IntersectionWith(LineSegment other)
    {
        Vector2 p = this.Point;
        Vector2 r = this.Delta;
        Vector2 q = other.Point;
        Vector2 s = other.Delta;

        Single rCrossS = r.CrossProduct(s);
        if (rCrossS == 0)
        {
            if ((q - p).CrossProduct(r) == 0)
            {
                // lines are collinear
                Single rDotR = Vector2.Dot(r, r);
                Single t0 = Vector2.Dot(q - p, r / rDotR);
                Single t1 = t0 + Vector2.Dot(s, r / rDotR);
                RangeF tRange = new(t0, t1);
                RangeF unitRange = new(0, 1);
                if (tRange.Overlaps(unitRange))
                {
                    // lines overlap
                    RangeF overlapped = new(unitRange.Clamp(tRange.Minimum),
                        unitRange.Clamp(tRange.Maximum));

                    return new Intersection.Overlaps(overlapped).Some<Intersection>();
                }
            }
            else
            {
                // lines are parallel
                return Option.None<Intersection>();
            }
        }
        else
        {
            Vector2 qMinusP = q - p;
            Single t = qMinusP.CrossProduct(s) / rCrossS;
            Single u = qMinusP.CrossProduct(r) / rCrossS;

            if (t is >= 0 and <= 1 && u is >= 0 and <= 1)
            {
                // lines intersect
                return new Intersection.Intersects(t).Some<Intersection>();
            }
        }
        
        return Option.None<Intersection>();
    }


    /// <inheritdoc/>
    public override String ToString()
    {
        return $"{this.Point} → {this.End}";
    }


    /// <inheritdoc/>
    public Boolean Equals(LineSegment other)
    {
        return Equality.CheckFromIEquatable(this, other,
            a => a.Point, 
            a => a.Delta,
            a => a.End);
    }


    /// <inheritdoc/>
    public override Boolean Equals(Object? other)
    {
        return Equality.CheckFromOverride(this, other);
    }


    /// <inheritdoc/>
    public override Int32 GetHashCode()
    {
        return Hash.Standard(
            this.Point,
            this.Delta,
            this.End);
    }


    /// <summary>
    ///     Check if one <see cref="LineSegment"/> instance has the same values as another.
    /// </summary>
    /// <param name="a">
    ///     The first <see cref="LineSegment"/> instance.
    /// </param>
    /// <param name="b">
    ///     The second <see cref="LineSegment"/> instance.
    /// </param>
    /// <returns>
    ///     True if the instances are equal.
    /// </returns>
    public static Boolean operator ==(LineSegment a, LineSegment b)
    {
        return Equality.CheckFromOperator(a, b);
    }


    /// <summary>
    ///     Check if one <see cref="LineSegment"/> instance has any difference in values from
    ///     another.
    /// </summary>
    /// <param name="a">
    ///     The first <see cref="LineSegment"/> instance.
    /// </param>
    /// <param name="b">
    ///     The second <see cref="LineSegment"/> instance.
    /// </param>
    /// <returns>
    ///     True if the instances are equal.
    /// </returns>
    public static Boolean operator !=(LineSegment a, LineSegment b)
    {
        return !(a == b);
    }
}
