// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System.Diagnostics;
using Moda.Core.Utility.Data;

namespace Moda.Core.Utility.Maths;

/// <summary>
///     Defines a range represented by two floating point values, and offers a variety of methods
///     for evaluating or manipulating values with respect to the range.
/// </summary>
[DebuggerDisplay("{Minimum} ↔ {Maximum}")]
public struct RangeF : IEquatable<RangeF>
{
    //##############################################################################################
    //
    //  Fields
    //
    //##############################################################################################
    
    private readonly Int32 hashCode;


    
    //##############################################################################################
    //
    //  Constructors
    //
    //##############################################################################################
    
    /// <summary>
    ///     Initializes a new <see cref="RangeF"/> instance, assigning <see cref="Minimum"/> and
    ///     <see cref="Maximum"/> based on values.
    /// </summary>
    /// <param name="valueA">
    ///     The first value that defines the range.
    /// </param>
    /// <param name="valueB">
    ///     The second value that defines the range.
    /// </param>
    public RangeF(Single valueA, Single valueB)
    {
        this.Minimum = Math.Min(valueA, valueB);
        this.Maximum = Math.Max(valueA, valueB);
        this.Delta = Math.Abs(valueB - valueA);
        this.hashCode = Hash.Standard(this.Minimum, this.Maximum, this.Delta);
    }


    
    //##############################################################################################
    //
    //  Properties
    //
    //##############################################################################################

    /// <summary>
    ///     The lower bound of the range.
    /// </summary>
    public Single Minimum { get; }
    
    /// <summary>
    ///     The upper bound of the range.
    /// </summary>
    public Single Maximum { get; }
    
    /// <summary>
    ///     The difference between the <see cref="Minimum"/> and <see cref="Maximum"/>.
    /// </summary>
    public Single Delta { get; }


    
    //##############################################################################################
    //
    //  Public Methods
    //
    //##############################################################################################
    
    /// <summary>
    ///     Take a value and return one that is guaranteed to be between the range, returning
    ///     the <see cref="Minimum"/> or <see cref="Maximum"/> if the value lies outside of the
    ///     bounds.
    /// </summary>
    /// <param name="value">
    ///     The value to clamp.
    /// </param>
    /// <returns>
    ///     The same value if it falls within the range, otherwise, <see cref="Minimum"/> or
    ///     <see cref="Maximum"/> depending on which side the value extends past.
    /// </returns>
    /// <example>
    ///     Range: 50 ↔ 100
    ///     Input: 120
    ///     Result: 100
    /// </example>
    public Single Clamp(Single value)
    {
        return Math.Min(Math.Max(this.Minimum, value), this.Maximum);
    }
    
    
    /// <summary>
    ///     Ensure that the specified value remains in this range by wrapping it around the bounds
    ///     if it exceeds them.
    /// </summary>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <returns>
    ///     The same value if it falls within the range, otherwise, the value wrapped around the
    ///     bounds in the direction that it exceeds them.
    /// </returns>
    /// <remarks>
    ///     Values larger than the delta of the range will wrap until the result falls within the
    ///     bounds.
    /// </remarks>
    /// <example>
    ///     Range: 50 ↔ 100
    ///     Input: 120
    ///     Result: 70
    /// </example>
    /// <example>
    ///     Range: -1 ↔ 1
    ///     Input: -7.25
    ///     Result: 0.75
    /// </example>
    public Single Wrap(Single value)
    {
        if (this.Delta == 0)
        {
            return this.Minimum;
        }
        
        Single grounded = value - this.Minimum;
        return (grounded - ((Single)Math.Floor(grounded / this.Delta) * this.Delta))
            + this.Minimum;
    }


    /// <summary>
    ///     Scale a value as it exists in one range, to the corresponding value in this range.
    /// </summary>
    /// <param name="value">
    ///     The value to scale.
    /// </param>
    /// <param name="sourceRange">
    ///     The reference range that the value exists in.
    /// </param>
    /// <returns>
    ///     The corresponding value in this range.
    /// </returns>
    /// <example>
    ///     Range: 0 ↔ 1
    ///     Input:
    ///         value: 60,
    ///         sourceRange: 0 ↔ 100
    ///     Result: 0.6
    /// </example>
    /// <example>
    ///     Range: 0 ↔ 1
    ///     Input:
    ///         value: 120,
    ///         sourceRange: 0 ↔ 100
    ///     Result: 1.2
    /// </example>
    /// <remarks>
    ///     Values outside of the source range will not be clamped or wrapped.
    /// </remarks>
    /// <seealso cref="Fit"/>
    /// <seealso cref="Wrap"/>
    /// <seealso cref="Map"/>
    public Single Scale(Single value, RangeF sourceRange)
    {
        return ((value - sourceRange.Minimum) * this.Delta / sourceRange.Delta) + this.Minimum;
    }


    /// <summary>
    ///     Scale a value as it exists in one range, to the corresponding value in this range,
    ///     and clamp the results if needed to ensure the value remains within the target range.
    /// </summary>
    /// <param name="value">
    ///     The value to scale.
    /// </param>
    /// <param name="sourceRange">
    ///     The reference range that the value exists in.
    /// </param>
    /// <returns>
    ///     The corresponding value in this range.
    /// </returns>
    /// <example>
    ///     Range: 0 ↔ 1
    ///     Input:
    ///         value: 60,
    ///         sourceRange: 0 ↔ 100
    ///     Result: 0.6
    /// </example>
    /// <example>
    ///     Range: 0 ↔ 1
    ///     Input:
    ///         value: 120,
    ///         sourceRange: 0 ↔ 100
    ///     Result: 1
    /// </example>
    /// <seealso cref="Scale"/>
    /// <seealso cref="Wrap"/>
    /// <seealso cref="Map"/>
    public Single Fit(Single value, RangeF sourceRange)
    {
        return Clamp(Scale(value, sourceRange));
    }


    /// <summary>
    ///     Scale a value as it exists in one range, to the corresponding value in this range,
    ///     and wrap the results if needed to ensure the value remains within the target range.
    /// </summary>
    /// <param name="value">
    ///     The value to scale.
    /// </param>
    /// <param name="sourceRange">
    ///     The reference range that the value exists in.
    /// </param>
    /// <returns>
    ///     The corresponding value in this range.
    /// </returns>
    /// <example>
    ///     Range: 0-1
    ///     Input:
    ///         value: 120,
    ///         fromRange: 0-100
    ///     Result: 0.2
    /// </example>
    /// <seealso cref="Scale"/>
    /// <seealso cref="Wrap"/>
    /// <seealso cref="Fit"/>
    public Single Map(Single value, RangeF sourceRange)
    {
        return Wrap(Scale(value, sourceRange));
    }


    /// <summary>
    ///     Get a human-friendly string representing the values of this range.
    /// </summary>
    /// <returns>
    ///     A formatted string with the <see cref="Minimum"/> and <see cref="Maximum"/>
    ///     values.
    /// </returns>
    public override String ToString() => $"[{Minimum} ↔ {Maximum}]";
    
    
    /// <summary>
    ///     Check if this <see cref="RangeF"/> instance is equal to another.
    /// </summary>
    /// <param name="other">
    ///     The other <see cref="RangeF"/> object to compare this one to.
    /// </param>
    /// <returns>
    ///     True if the two instances are equivalent.
    /// </returns>
    public Boolean Equals(RangeF other)
    {
        return Equality.CheckFromIEquatable(this, other,
            a => a.Minimum,
            a => a.Maximum);
    }

    /// <summary>
    ///     Check if this <see cref="RangeF"/> instance is equal to another.
    /// </summary>
    /// <param name="other">
    ///     The other <see cref="RangeF"/> object to compare this one to.
    /// </param>
    /// <returns>
    ///     True if the two instances are equivalent.
    /// </returns>
    public override Boolean Equals(Object? other)
    {
        return Equality.CheckFromOverride(this, other);
    }


    /// <summary>
    ///     Check if one <see cref="RangeF"/> instance is equal to another.
    /// </summary>
    /// <param name="objectA">
    ///     The first <see cref="RangeF"/> instance.
    /// </param>
    /// /// <param name="objectB">
    ///     The second <see cref="RangeF"/> instance.
    /// </param>
    /// <returns>
    ///     True if the two instances are equivalent.
    /// </returns>
    public static Boolean operator ==(RangeF objectA, RangeF objectB)
    {
        return Equality.CheckFromOperator(objectA, objectB);
    }

    /// <summary>
    ///     Check if one <see cref="RangeF"/> instance are not equal to another.
    /// </summary>
    /// <param name="objectA">
    ///     The first <see cref="RangeF"/> instance.
    /// </param>
    /// <param name="objectB">
    ///     The second <see cref="RangeF"/> instance.
    /// </param>
    /// <returns>
    ///     True if the two instances are not equivalent.
    /// </returns>
    public static Boolean operator !=(RangeF objectA, RangeF objectB)
    {
        return !(objectA == objectB);
    }

    /// <summary>
    ///     Get a hash code for this instance.
    /// </summary>
    /// <returns>
    ///     The hash code calculated for this instance.
    /// </returns>
    public override Int32 GetHashCode()
    {
        return this.hashCode;
    }
}
