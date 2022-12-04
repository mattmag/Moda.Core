// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using FluentAssertions;
using Moda.Core.UI;
using Moda.Core.UI.Lengths;
using Moq;
using NUnit.Framework;

// ReSharper disable ClassNeverInstantiated.Global

namespace Moda.Core.Tests.UI.Lengths;




public class SumTests
{
    // Operation Combination Tests
    //----------------------------------------------------------------------------------------------

    [Test]
    public void CalculateFromCombinedOperationsShouldMatchCSharpRules()
    {
        Single expected = 4.3f - (40.1f + 2.7f) + 13.7f - 18.0f + (30.2f - 2.1f + 1.0f);
        Single result = (Len(4.3f) - (40.1f + Len(2.7f)) + Len(13.7f) - Len(18.0f) +
            (Len(30.2f) - Len(2.1f) + 1.0f)).Calculate();
        result.Should().Be(expected);
    }

    private Length Len(Single value)
    {
        Mock<Length> length = new();
        length.Setup(a => a.Calculate()).Returns(value);
        return length.Object;
    }
    
}






[TestFixture(typeof(Sum), typeof(Length), typeof(Length), nameof(AddLengthLengthParams))]
[TestFixture(typeof(Sum), typeof(Length), typeof(Single), nameof(AddLengthConstantParams))]
[TestFixture(typeof(Sum), typeof(Length), typeof(Length), nameof(LengthPlusLengthParams))]
[TestFixture(typeof(Sum), typeof(Length), typeof(Single), nameof(LengthPlusConstantParams))]
[TestFixture(typeof(Sum), typeof(Single), typeof(Length), nameof(ConstantPlusLengthParams))]
[TestFixture(typeof(Sum), typeof(Length), typeof(Length), nameof(SubtractLengthLengthParams))]
[TestFixture(typeof(Sum), typeof(Length), typeof(Single), nameof(SubtractLengthConstantParams))]
[TestFixture(typeof(Sum), typeof(Length), typeof(Length), nameof(LengthMinusLengthParams))]
[TestFixture(typeof(Sum), typeof(Length), typeof(Single), nameof(LengthMinusConstantParams))]
[TestFixture(typeof(Sum), typeof(Single), typeof(Length), nameof(ConstantMinusLengthParams))]
public partial class ArithmeticBaseFixture<TArithmetic, TArg1, TArg2>
    where TArithmetic : Arithmetic
    where TArg1 : notnull
    where TArg2 : notnull
{
    
}


[TestFixture(typeof(Sum), typeof(Length), nameof(SecondaryAddLengthParams))]
[TestFixture(typeof(Sum), typeof(Single), nameof(SecondaryAddConstantParams))]
[TestFixture(typeof(Sum), typeof(Length), nameof(SecondarySubtractLengthParams))]
[TestFixture(typeof(Sum), typeof(Single), nameof(SecondarySubtractConstantParams))]
public partial class ArithmeticSecondaryOpFixture<TArithmetic, TArg>
    where TArithmetic : Arithmetic
    where TArg : notnull
{
    
}



// Add
//----------------------------------------------------------------------------------------------

public class AddLengthLengthParams : ArithmeticBaseParams<Sum, Length, Length>
{
    public AddLengthLengthParams() : base(new(1, 2, 3), new (4, 6, 10))
    {
    }
    
    public override Func<Length, Length, Sum> Factory => Sum.Add;
}


public class AddLengthConstantParams : ArithmeticBaseParams<Sum, Length, Single>
{
    public AddLengthConstantParams() : base(new(1, 2, 3), new (4, 2, 6))
    {
    }
    
    public override Func<Length, Single, Sum> Factory => Sum.Add;
}


public class LengthPlusLengthParams : ArithmeticBaseParams<Sum, Length, Length>
{
    public LengthPlusLengthParams() : base(new(1, 2, 3), new (4, 2, 6))
    {
    }
    
    public override Func<Length, Length, Sum> Factory => (a, b) => a + b;
}

public class LengthPlusConstantParams : ArithmeticBaseParams<Sum, Length, Single>
{
    public LengthPlusConstantParams() : base(new(1, 2, 3), new (4, 2, 6))
    {
    }
    
    public override Func<Length, Single, Sum> Factory => (a, b) => a + b;
}


public class ConstantPlusLengthParams : ArithmeticBaseParams<Sum, Single, Length>
{
    public ConstantPlusLengthParams() : base(new(1, 2, 3), new (1, 4, 5))
    {
    }
    
    public override Func<Single, Length, Sum> Factory => (a, b) => a + b;
}


// Subtract
//----------------------------------------------------------------------------------------------
public class SubtractLengthLengthParams : ArithmeticBaseParams<Sum, Length, Length>
{
    public SubtractLengthLengthParams() : base(new(15, 4, 11), new (40, 9, 31))
    {
    }
    
    public override Func<Length, Length, Sum> Factory => Sum.Subtract;
}


public class SubtractLengthConstantParams : ArithmeticBaseParams<Sum, Length, Single>
{
    public SubtractLengthConstantParams() : base(new(15, 4, 11), new (40, 4, 36))
    {
    }
    
    public override Func<Length, Single, Sum> Factory => Sum.Subtract;
}


public class LengthMinusLengthParams : ArithmeticBaseParams<Sum, Length, Length>
{
    public LengthMinusLengthParams() : base(new(15, 4, 11), new (40, 4, 36))
    {
    }
    
    public override Func<Length, Length, Sum> Factory => (a, b) => a - b;
}

public class LengthMinusConstantParams : ArithmeticBaseParams<Sum, Length, Single>
{
    public LengthMinusConstantParams() : base(new(15, 4, 11), new (40, 4, 36))
    {
    }
    
    public override Func<Length, Single, Sum> Factory => (a, b) => a - b;
}


public class ConstantMinusLengthParams : ArithmeticBaseParams<Sum, Single, Length>
{
    public ConstantMinusLengthParams() : base(new(15, 4, 11), new (15, 9, 6))
    {
    }
    
    public override Func<Single, Length, Sum> Factory => (a, b) => a - b;
}


// Secondary Add
//----------------------------------------------------------------------------------------------

public class SecondaryAddLengthParams : ArithmeticSecondaryOpParams<Sum, Length>
{
    public SecondaryAddLengthParams() : base(new(1, 2, 5, 8), new(4, 6, 2, 12))
    {
    }
    
    public override Func<Length, Length, Sum> Factory => Sum.Add;
    public override Func<Sum, Length, Sum> SecondOperation => (sum, c) => sum + c;
}

public class SecondaryAddConstantParams : ArithmeticSecondaryOpParams<Sum, Single>
{
    public SecondaryAddConstantParams() : base(new(1, 2, 5, 8), new(4, 6, 5, 15))
    {
    }
    
    public override Func<Length, Length, Sum> Factory => Sum.Add;
    public override Func<Sum, Single, Sum> SecondOperation => (sum, c) => sum + c;
}

// Secondary Subtract
//----------------------------------------------------------------------------------------------

public class SecondarySubtractLengthParams : ArithmeticSecondaryOpParams<Sum, Length>
{
    public SecondarySubtractLengthParams() : base(new(20, 3, 2, 15), new(40, 13, 4, 23))
    {
    }
    
    public override Func<Length, Length, Sum> Factory => Sum.Subtract;
    public override Func<Sum, Length, Sum> SecondOperation => (sum, c) => sum - c;
}

public class SecondarySubtractConstantParams : ArithmeticSecondaryOpParams<Sum, Single>
{
    public SecondarySubtractConstantParams() : base(new(20, 3, 2, 15), new(40, 13, 2, 25))
    {
    }
    
    public override Func<Length, Length, Sum> Factory => Sum.Subtract;
    public override Func<Sum, Single, Sum> SecondOperation => (sum, c) => sum - c;
}
