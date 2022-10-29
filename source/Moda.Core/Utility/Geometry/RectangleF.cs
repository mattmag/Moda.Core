// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System.Diagnostics;
using System.Numerics;
using Moda.Core.Utility.Data;

namespace Moda.Core.Utility.Geometry;

/// <summary>
///     An axis-aligned rectangle defined with floating point values.
/// </summary>
[DebuggerDisplay("(TopLeft: {topLeft}, Size: {Size}")]
public readonly struct RectangleF : IEquatable<RectangleF>
{
    //##############################################################################################
    //
    //  Fields
    //
    //##############################################################################################
    
    private readonly Vector2 topLeft;
    private readonly LineSegment topSide;
    private readonly Vector2 topRight;
    private readonly LineSegment rightSide;
    private readonly Vector2 bottomRight;
    private readonly LineSegment bottomSide;
    private readonly Vector2 bottomLeft;
    private readonly LineSegment leftSide;


    //##############################################################################################
    //
    //   Constructors
    //
    //##############################################################################################

    /// <summary>
    ///     Initialize a new instance.
    /// </summary>
    /// <param name="topLeft">
    ///     The location of the top-left corner.
    /// </param>
    /// <param name="size">
    ///     The width and height of the rectangle.
    /// </param>
    public RectangleF(Vector2 topLeft, Size2 size)
    {
        this.topLeft = topLeft;
        this.topSide = LineSegment.FromPointWithDelta(this.topLeft, new (size.Width, 0));
        this.topRight = this.topSide.End;
        this.rightSide = LineSegment.FromPointWithDelta(this.topRight, new(0, size.Height));
        this.bottomRight = this.rightSide.End;
        this.bottomSide = LineSegment.FromPointWithDelta(this.bottomRight, new(-size.Width, 0));
        this.bottomLeft = this.bottomSide.End;
        this.leftSide = LineSegment.FromPointWithDelta(this.bottomLeft, new(0, -size.Height));

        this.Center = topLeft + new Vector2(size.Width / 2, size.Height / 2);
        this.Size = size;
    }

    /// <summary>
    ///     Initialize a new <see cref="RectangleF"/> instance with the same values as another.
    /// </summary>
    /// <param name="copy">
    ///     The <see cref="RectangleF"/> to copy.
    /// </param>
    public RectangleF(RectangleF copy) : this(copy.topLeft, copy.Size)
    {

    }


    //##############################################################################################
    //
    //  Properties
    //
    //##############################################################################################
    
    /// <summary>
    ///     The location of the center of the rectangle.
    /// </summary>
    public Vector2 Center
    {
        get;
    }


    /// <summary>
    ///     The width of the rectangle.
    /// </summary>
    public Size2 Size
    {
        get;
    }
    
    /// <summary>
    ///    Retrieve a single side of the rectangle.
    /// </summary>
    /// <param name="side">
    ///     The <see cref="Side"/> to retrieve.
    /// </param>
    /// <returns>
    ///     The side, as a <see cref="LineSegment"/>.
    /// </returns>
    /// <remarks>
    ///     Line segments are directed clockwise from the top left.
    /// </remarks>
    public LineSegment this[Side side]
    {
        get
        {
            switch (side)
            {
                case Side.Left:
                {
                    return this.leftSide;
                }
                case Side.Top:
                {
                    return this.topSide;
                }
                case Side.Right:
                {
                    return this.rightSide;
                }
                case Side.Bottom:
                {
                    return this.bottomSide;
                }
                default:
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }
    }
    
    
    /// <summary>
    ///     Retrieve a single corner of the rectangle.
    /// </summary>
    /// <param name="corner">
    ///     The <see cref="Corner"/> to retrieve.
    /// </param>
    /// <returns>
    ///     The location of the corner, as a <see cref="Vector2"/>.
    /// </returns>
    public Vector2 this[Corner corner]
    {
        get
        {
            switch (corner)
            {
                case Corner.TopLeft:
                {
                    return this.topLeft;
                }
                case Corner.TopRight:
                {
                    return this.topRight;
                }
                case Corner.BottomRight:
                {
                    return this.bottomRight;
                }
                case Corner.BottomLeft:
                {
                    return this.bottomLeft;
                }
                default:
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }
    }


    //##############################################################################################
    //
    //  Public Methods
    //
    //##############################################################################################
    

    // Enumerate
    //----------------------------------------------------------------------------------------------
    
    /// <summary>
    ///     Enumerate through each side of the rectangle.
    /// </summary>
    /// <returns>
    ///     A <see cref="KeyValuePair{TKey, TValue}"/> for each side, pairing a
    ///     <see cref="Side"/> with it's <see cref="LineSegment"/>.
    /// </returns>
    public IEnumerable<KeyValuePair<Side, LineSegment>> GetSides() // EnumerateSides
    {
        yield return this.leftSide.KeyedOn(Side.Left);
        yield return this.topSide.KeyedOn(Side.Top);
        yield return this.rightSide.KeyedOn(Side.Right);
        yield return this.bottomSide.KeyedOn(Side.Bottom);
    }


    /// <summary>
    ///     Enumerate through each corner of the rectangle.
    /// </summary>
    /// <returns>
    ///     A <see cref="KeyValuePair{TKey, TValue}"/> for each side, pairing a
    ///     <see cref="Corner"/> with it's location.
    /// </returns>
    public IEnumerable<KeyValuePair<Corner, Vector2>> GetCorners() // EnumerateCorners
    {
        yield return this.topLeft.KeyedOn(Corner.TopLeft);
        yield return this.topRight.KeyedOn(Corner.TopRight);
        yield return this.bottomRight.KeyedOn(Corner.BottomRight);
        yield return this.bottomLeft.KeyedOn(Corner.BottomLeft);
    }

    // Checks
    //----------------------------------------------------------------------------------------------
    
    /// <summary>
    ///     Check if another <see cref="RectangleF"/> overlaps this one.
    /// </summary>
    /// <param name="other">
    ///     The other rectangle.
    /// </param>
    /// <returns>
    ///     True if the two rectangles overlap.
    /// </returns>
    public Boolean IsOverlapping(RectangleF other)
    {
        return this[Side.Left].Point.X <= other[Side.Right].Point.X &&
            this[Side.Right].Point.X >= other[Side.Left].Point.X &&
            this[Side.Top].Point.Y <= other[Side.Bottom].Point.Y &&
            this[Side.Bottom].Point.Y >= other[Side.Top].Point.Y;
    }
    
    
    /// <summary>
    ///     Check if a point falls within the bounds of this rectangle.
    /// </summary>
    /// <param name="location">
    ///     The point to check.
    /// </param>
    /// <returns>
    ///     True if the rectangle inclusively contains the provided point.
    /// </returns>
    public Boolean ContainsPoint(Vector2 location)
    {
        return (location.X >= this.topLeft.X && location.Y >= this.topLeft.Y) &&
            (location.X <= this.bottomRight.X && location.Y <= this.bottomRight.Y);
    }
    
    /// <summary>
    ///     Check if a line segment intersects this rectangle.
    /// </summary>
    /// <param name="segment">
    ///     The line segment to check.
    /// </param>
    /// <returns>
    ///     Every intersection that occurred between the <paramref name="segment"/> and the
    ///     <see cref="Side">Sides</see> of this rectangle.
    /// </returns>
    public IEnumerable<SideIntersection> IntersectionWith(LineSegment segment)
    {
        List<SideIntersection> results = new();
        Comparer<SideIntersection> comparer = Comparer<SideIntersection>.Create(
            (a, b) => a.Intersection.FirstImpact()
                .CompareTo(b.Intersection.FirstImpact()));
        
        foreach ((Side side, LineSegment sideLine) in GetSides())
        {
            segment.IntersectionWith(sideLine)
                .MatchSome(itx => results.AddSorted(new(side, itx), comparer));
        }

        return results;
    }


    // Modifications
    //----------------------------------------------------------------------------------------------
    
    /// <summary>
    ///     Expand this rectangle about it's center by the specified width and height increase.
    /// </summary>
    /// <param name="width">
    ///     The value to expand in the X axis.
    /// </param>
    /// <param name="height">
    ///     The value to expand in the Y axis.
    /// </param>
    /// <returns>
    ///     The resulting <see cref="RectangleF"/> instance.
    /// </returns>
    /// <example>
    ///     RectangleF rectangle = new RectangleF(new Vector2(20, 30), new Size2(40, 60))
    ///     RectangleF inflated = rectangle.Inflate(10, 5)
    /// 
    ///     // prints "w:50 h:65"
    ///     Console.WriteLine($"w:{inflated.Size.Width} h:{inflated.Size.Width}");
    /// 
    ///      // prints "(15, 27.5)"
    ///     Console.WriteLine($"top left: {$inflated[Corner.TopLeft]}");
    /// </example>
    public RectangleF Inflate(Single width, Single height)
    {
        Single newWidth = this.Size.Width + width;
        Single newHeight = this.Size.Height + height;
        Vector2 offset = new(width / 2, height / 2);
        return new(this.topLeft - offset, new(newWidth, newHeight));
    }
    
    
    /// <summary>
    ///     Translate the rectangle by the delta specified as a vector.
    /// </summary>
    /// <param name="by">
    ///     The vector describing the relative translation.
    /// </param>
    /// <returns>
    ///     The resulting <see cref="RectangleF"/> instance with the new location.
    /// </returns>
    public RectangleF Translate(Vector2 by)
    {
        return new(this.topLeft + by, this.Size);
    }
    
    
    /// <summary>
    ///     Translate the rectangle so that it's center lies at the specified location.
    /// </summary>
    /// <param name="location">
    ///     The new location for the rectangles center.
    /// </param>
    /// <returns>
    ///     The resulting <see cref="RectangleF"/> instance with the new location.
    /// </returns>
    public RectangleF CenterAt(Vector2 location)
    {
        Vector2 delta = location - this.Center;
        return this.Translate(delta);
    }


    // Equality
    //----------------------------------------------------------------------------------------------
    
    /// <summary>
    ///     Check if another <see cref="RectangleF"/> has the same size and location as this one.
    /// </summary>
    /// <param name="other">
    ///     The other rectangle.
    /// </param>
    /// <returns>
    ///     True if the rectangles are equal.
    /// </returns>
    public override Boolean Equals(Object? other)
    {
        return Equality.CheckFromOverride(this, other);
    }

    /// <summary>
    ///     Check if another <see cref="RectangleF"/> has the same size and location as this one.
    /// </summary>
    /// <param name="other">
    ///     The other rectangle.
    /// </param>
    /// <returns>
    ///     True if the rectangles are equal.
    /// </returns>
    public Boolean Equals(RectangleF other)
    {
        return Equality.CheckFromIEquatable(this, other,
            a => a.topLeft,
            a => a.Size);
    }

    /// <summary>
    ///     Check if two <see cref="RectangleF"/> instances have the same size and location.
    /// </summary>
    /// <param name="a">
    ///     The first rectangle.
    /// </param>
    /// <param name="b">
    ///     The second rectangle.
    /// </param>
    /// <returns>
    ///     True if the rectangles are equal.
    /// </returns>
    public static Boolean operator ==(RectangleF a, RectangleF b)
    {
        return Equality.CheckFromOperator(a, b);
    }

    /// <summary>
    ///     Check to see if two <see cref="RectangleF"/> instances do not have the same size
    ///     and/or location.
    /// </summary>
    /// <param name="a">
    ///     The first rectangle.
    /// </param>
    /// <param name="b">
    ///     The second rectangle.
    /// </param>
    /// <returns>
    ///     True if the rectangles are not equal.
    /// </returns>
    public static Boolean operator !=(RectangleF a, RectangleF b)
    {
        return !Equality.CheckFromOperator(a, b);
    }

    /// <summary>
    ///     Calculate a hashcode based on this rectangles parameters.
    /// </summary>
    /// <returns>
    ///     A hashcode based on this rectangles parameters.
    /// </returns>
    public override Int32 GetHashCode()
    {
        return Hash.Standard(this.topLeft, this.Size);
    }
    
}
