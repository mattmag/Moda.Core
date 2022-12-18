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

    private ILength Len(Single value)
    {
        Mock<ILength> length = new();
        length.Setup(a => a.Calculate()).Returns(value);
        return length.Object;
    }
    
}






[TestFixture(typeof(Sum), typeof(ILength), typeof(ILength), nameof(AddLengthLengthParams))]
[TestFixture(typeof(Sum), typeof(ILength), typeof(Single), nameof(AddLengthConstantParams))]
[TestFixture(typeof(Sum), typeof(ILength), typeof(ILength), nameof(LengthPlusLengthParams))]
[TestFixture(typeof(Sum), typeof(ILength), typeof(Single), nameof(LengthPlusConstantParams))]
[TestFixture(typeof(Sum), typeof(Single), typeof(ILength), nameof(ConstantPlusLengthParams))]
[TestFixture(typeof(Sum), typeof(ILength), typeof(ILength), nameof(SubtractLengthLengthParams))]
[TestFixture(typeof(Sum), typeof(ILength), typeof(Single), nameof(SubtractLengthConstantParams))]
[TestFixture(typeof(Sum), typeof(ILength), typeof(ILength), nameof(LengthMinusLengthParams))]
[TestFixture(typeof(Sum), typeof(ILength), typeof(Single), nameof(LengthMinusConstantParams))]
[TestFixture(typeof(Sum), typeof(Single), typeof(ILength), nameof(ConstantMinusLengthParams))]
public partial class ArithmeticBaseFixture<TArithmetic, TArg1, TArg2>
    where TArithmetic : Arithmetic
    where TArg1 : notnull
    where TArg2 : notnull
{
    
}


[TestFixture(typeof(Sum), typeof(ILength), nameof(SecondaryAddLengthParams))]
[TestFixture(typeof(Sum), typeof(Single), nameof(SecondaryAddConstantParams))]
[TestFixture(typeof(Sum), typeof(ILength), nameof(SecondarySubtractLengthParams))]
[TestFixture(typeof(Sum), typeof(Single), nameof(SecondarySubtractConstantParams))]
public partial class ArithmeticSecondaryOpFixture<TArithmetic, TArg>
    where TArithmetic : Arithmetic
    where TArg : notnull
{
    
}



// Add
//----------------------------------------------------------------------------------------------

public class AddLengthLengthParams : ArithmeticBaseParams<Sum, ILength, ILength>
{
    public AddLengthLengthParams() : base(new(1, 2, 3), new (4, 6, 10))
    {
    }
    
    public override Func<ILength, ILength, Sum> Factory => Sum.Add;
}


public class AddLengthConstantParams : ArithmeticBaseParams<Sum, ILength, Single>
{
    public AddLengthConstantParams() : base(new(1, 2, 3), new (4, 2, 6))
    {
    }
    
    public override Func<ILength, Single, Sum> Factory => Sum.Add;
}


public class LengthPlusLengthParams : ArithmeticBaseParams<Sum, ILength, ILength>
{
    public LengthPlusLengthParams() : base(new(1, 2, 3), new (4, 2, 6))
    {
    }
    
    public override Func<ILength, ILength, Sum> Factory => (a, b) => a + b;
}

public class LengthPlusConstantParams : ArithmeticBaseParams<Sum, ILength, Single>
{
    public LengthPlusConstantParams() : base(new(1, 2, 3), new (4, 2, 6))
    {
    }
    
    public override Func<ILength, Single, Sum> Factory => (a, b) => a + b;
}


public class ConstantPlusLengthParams : ArithmeticBaseParams<Sum, Single, ILength>
{
    public ConstantPlusLengthParams() : base(new(1, 2, 3), new (1, 4, 5))
    {
    }
    
    public override Func<Single, ILength, Sum> Factory => (a, b) => a + b;
}


// Subtract
//----------------------------------------------------------------------------------------------
public class SubtractLengthLengthParams : ArithmeticBaseParams<Sum, ILength, ILength>
{
    public SubtractLengthLengthParams() : base(new(15, 4, 11), new (40, 9, 31))
    {
    }
    
    public override Func<ILength, ILength, Sum> Factory => Sum.Subtract;
}


public class SubtractLengthConstantParams : ArithmeticBaseParams<Sum, ILength, Single>
{
    public SubtractLengthConstantParams() : base(new(15, 4, 11), new (40, 4, 36))
    {
    }
    
    public override Func<ILength, Single, Sum> Factory => Sum.Subtract;
}


public class LengthMinusLengthParams : ArithmeticBaseParams<Sum, ILength, ILength>
{
    public LengthMinusLengthParams() : base(new(15, 4, 11), new (40, 4, 36))
    {
    }
    
    public override Func<ILength, ILength, Sum> Factory => (a, b) => a - b;
}

public class LengthMinusConstantParams : ArithmeticBaseParams<Sum, ILength, Single>
{
    public LengthMinusConstantParams() : base(new(15, 4, 11), new (40, 4, 36))
    {
    }
    
    public override Func<ILength, Single, Sum> Factory => (a, b) => a - b;
}


public class ConstantMinusLengthParams : ArithmeticBaseParams<Sum, Single, ILength>
{
    public ConstantMinusLengthParams() : base(new(15, 4, 11), new (15, 9, 6))
    {
    }
    
    public override Func<Single, ILength, Sum> Factory => (a, b) => a - b;
}


// Secondary Add
//----------------------------------------------------------------------------------------------

public class SecondaryAddLengthParams : ArithmeticSecondaryOpParams<Sum, ILength>
{
    public SecondaryAddLengthParams() : base(new(1, 2, 5, 8), new(4, 6, 2, 12))
    {
    }
    
    public override Func<ILength, ILength, Sum> Factory => Sum.Add;
    public override Func<Sum, ILength, Sum> SecondOperation => (sum, c) => sum + c;
}

public class SecondaryAddConstantParams : ArithmeticSecondaryOpParams<Sum, Single>
{
    public SecondaryAddConstantParams() : base(new(1, 2, 5, 8), new(4, 6, 5, 15))
    {
    }
    
    public override Func<ILength, ILength, Sum> Factory => Sum.Add;
    public override Func<Sum, Single, Sum> SecondOperation => (sum, c) => sum + c;
}

// Secondary Subtract
//----------------------------------------------------------------------------------------------

public class SecondarySubtractLengthParams : ArithmeticSecondaryOpParams<Sum, ILength>
{
    public SecondarySubtractLengthParams() : base(new(20, 3, 2, 15), new(40, 13, 4, 23))
    {
    }
    
    public override Func<ILength, ILength, Sum> Factory => Sum.Subtract;
    public override Func<Sum, ILength, Sum> SecondOperation => (sum, c) => sum - c;
}

public class SecondarySubtractConstantParams : ArithmeticSecondaryOpParams<Sum, Single>
{
    public SecondarySubtractConstantParams() : base(new(20, 3, 2, 15), new(40, 13, 2, 25))
    {
    }
    
    public override Func<ILength, ILength, Sum> Factory => Sum.Subtract;
    public override Func<Sum, Single, Sum> SecondOperation => (sum, c) => sum - c;
}
