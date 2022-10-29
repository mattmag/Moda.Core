// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System.Numerics;
using Moda.Core.Utility.Data;

namespace Moda.Core.Utility.Geometry;

/// <summary>
///     A simple vector representing an objects size in 2 dimensions.
/// </summary>
public readonly struct Size2 : IEquatable<Size2>
{
    //##########################################################################################
    //
    //   Constructors
    //
    //##########################################################################################
    
    /// <summary>
    ///     Creates a new <see cref="Size2"/> instance.
    /// </summary>
    /// <param name="width">
    ///     The width value.
    /// </param>
    /// <param name="height">
    ///     The height value.
    /// </param>
    public Size2(Single width, Single height)
    {
        this.Width = width;
        this.Height = height;
    }



    //##########################################################################################
    //
    //   Properties
    //
    //##########################################################################################
    
    /// <summary>
    ///     The width value.
    /// </summary>
    public Single Width { get; }

    /// <summary>
    ///     The height value.
    /// </summary>
    public Single Height { get; }



    //##########################################################################################
    //
    //   Public Methods
    //
    //##########################################################################################
    
    /// <summary>
    ///     Create a new <see cref="Vector2"/> instance with the X and Y values set to the
    ///     width and height values of this <see cref="Size2"/> instance.
    /// </summary>
    /// <returns>
    ///     A new <see cref="Vector2"/> instance.
    /// </returns>
    public Vector2 ToVector2()
    {
        return new(this.Width, this.Height);
    }

    //  Equality
    //------------------------------------------------------------------------------------------
    
    /// <summary>
    ///     Check if this <see cref="Size2"/> instance has the same width and height as
    ///     another.
    /// </summary>
    /// <param name="other">
    ///     The other <see cref="Size2"/> instance to check against.
    /// </param>
    /// <returns>
    ///     True if the instances are equal.
    /// </returns>
    public Boolean Equals(Size2 other)
    {
        return Equality.CheckFromIEquatable(this, other,
            a => a.Width,
            a => a.Height);
    }

    /// <summary>
    ///     Check if this <see cref="Size2"/> instance has the same width and height as
    ///     another.
    /// </summary>
    /// <param name="other">
    ///     The other <see cref="Size2"/> instance to check against.
    /// </param>
    /// <returns>
    ///     True if the instances are equal.
    /// </returns>
    public override Boolean Equals(Object? other)
    {
        return Equality.CheckFromOverride(this, other);
    }

    /// <summary>
    ///     Check if one <see cref="Size2"/> instance has the same width and height as
    ///     another.
    /// </summary>
    /// <param name="a">
    ///     The first <see cref="Size2"/> instance.
    /// </param>
    /// <param name="b">
    ///     The second <see cref="Size2"/> instance.
    /// </param>
    /// <returns>
    ///     True if the instances are equal.
    /// </returns>
    public static Boolean operator ==(Size2 a, Size2 b)
    {
        return Equality.CheckFromOperator(a, b);
    }


    /// <summary>
    ///     Check if one <see cref="Size2"/> instance does not have the same width and/or
    ///     height as another.
    /// </summary>
    /// <param name="a">
    ///     The first <see cref="Size2"/> instance.
    /// </param>
    /// <param name="b">
    ///     The second <see cref="Size2"/> instance.
    /// </param>
    /// <returns>
    ///     True if the instances are not equal.
    /// </returns>
    public static Boolean operator !=(Size2 a, Size2 b)
    {
        return !Equality.CheckFromOperator(a, b);
    }


    /// <inheritdoc/>
    public override Int32 GetHashCode()
    {
        return Hash.Standard(this.Width, this.Height);
    }



    //  Other Operators
    //------------------------------------------------------------------------------------------
    
    /// <summary>
    ///     Add the width and height of one <see cref="Size2"/> instance to another.
    /// </summary>
    /// <param name="a">
    ///     The first <see cref="Size2"/> instance.
    /// </param>
    /// <param name="b">
    ///     The second <see cref="Size2"/> instance.
    /// </param>
    /// <returns>
    ///     A new <see cref="Size2"/> instance with the width and height values set to the sum
    ///     of the respective values of each input size.
    /// </returns>
    public static Size2 operator +(Size2 a, Size2 b)
    {
        return new(a.Width + b.Width, a.Height + b.Height);
    }

    /// <summary>
    ///     Subtract the row and column values of one <see cref="Size2"/> from another.
    /// </summary>
    /// <param name="a">
    ///     The <see cref="Size2"/> instance to subtract from.
    /// </param>
    /// <param name="b">
    ///     The <see cref="Size2"/> instance of whose values to subtract.
    /// </param>
    /// <returns>
    ///     A new <see cref="Size2"/> instance with the width and height values set to the
    ///     difference of the respective values of each input size.
    /// </returns>
    public static Size2 operator -(Size2 a, Size2 b)
    {
        return new(a.Width - b.Width, a.Height - b.Height);
    }


    /// <inheritdoc/>
    public override String ToString()
    {
        return $"(w:{this.Width} h:{this.Height})";
    }

}
