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




public class ProductTests
{
    // Operation Combination Tests
    //----------------------------------------------------------------------------------------------

    [Test]
    public void CalculateFromCombinedOperationsShouldMatchCSharpRules()
    {
        Single expected = 4.3f / (40.1f * 2.7f) * 13.7f / 18.0f * (30.2f / 2.1f * 1.0f);
        Single result = (Len(4.3f) / (40.1f * Len(2.7f)) * Len(13.7f) / Len(18.0f) *
            (Len(30.2f) / Len(2.1f) * 1.0f)).Calculate();
        result.Should().Be(expected);
    }


    [Test]
    public void CalculateFromCombinedWithSumShouldMatchCSharpRules()
    {
        Single expected = 2.0f * 4.0f + 1.0f / 3.0f - (2 * 5);
        Single result = (Len(2.0f) * 4.0f + Len(1.0f) / Len(3.0f) - (2 * Len(5))).Calculate();
        result.Should().Be(expected);
    }

    private ILength Len(Single value)
    {
        Mock<ILength> length = new();
        length.Setup(a => a.Calculate()).Returns(value);
        return length.Object;
    }
    
}






[TestFixture(typeof(Product), typeof(ILength), typeof(ILength), nameof(MultiplyLengthLengthParams))]
[TestFixture(typeof(Product), typeof(ILength), typeof(Single), nameof(MultiplyLengthConstantParams))]
[TestFixture(typeof(Product), typeof(ILength), typeof(ILength), nameof(LengthTimesLengthParams))]
[TestFixture(typeof(Product), typeof(ILength), typeof(Single), nameof(LengthTimesConstantParams))]
[TestFixture(typeof(Product), typeof(Single), typeof(ILength), nameof(ConstantTimesLengthParams))]
[TestFixture(typeof(Product), typeof(ILength), typeof(ILength), nameof(DivideLengthLengthParams))]
[TestFixture(typeof(Product), typeof(ILength), typeof(Single), nameof(DivideLengthConstantParams))]
[TestFixture(typeof(Product), typeof(ILength), typeof(ILength), nameof(LengthDivideByLengthParams))]
[TestFixture(typeof(Product), typeof(ILength), typeof(Single), nameof(LengthDivideByConstantParams))]
[TestFixture(typeof(Product), typeof(Single), typeof(ILength), nameof(ConstantDivideByLengthParams))]
public partial class ArithmeticBaseFixture<TArithmetic, TArg1, TArg2>
    where TArithmetic : Arithmetic
    where TArg1 : notnull
    where TArg2 : notnull
{
    
}


[TestFixture(typeof(Product), typeof(ILength), nameof(SecondaryMultiplyLengthParams))]
[TestFixture(typeof(Product), typeof(Single), nameof(SecondaryMultiplyConstantParams))]
[TestFixture(typeof(Product), typeof(ILength), nameof(SecondaryDivideLengthParams))]
[TestFixture(typeof(Product), typeof(Single), nameof(SecondaryDivideConstantParams))]
public partial class ArithmeticSecondaryOpFixture<TArithmetic, TArg>
    where TArithmetic : Arithmetic
    where TArg : notnull
{
    
}



// Multiple
//----------------------------------------------------------------------------------------------

public class MultiplyLengthLengthParams : ArithmeticBaseParams<Product, ILength, ILength>
{
    public MultiplyLengthLengthParams() : base(new(2, 4, 8), new(10, 5, 50))
    {
    }
    
    public override Func<ILength, ILength, Product> Factory => Product.Multiply;
}


public class MultiplyLengthConstantParams : ArithmeticBaseParams<Product, ILength, Single>
{
    public MultiplyLengthConstantParams() : base(new(2, 4, 8), new(10, 4, 40))
    {
    }
    
    public override Func<ILength, Single, Product> Factory => Product.Multiply;
}


public class LengthTimesLengthParams : ArithmeticBaseParams<Product, ILength, ILength>
{
    public LengthTimesLengthParams() : base(new(2, 4, 8), new(10, 5, 50))
    {
    }
    
    public override Func<ILength, ILength, Product> Factory => (a, b) => a * b;
}

public class LengthTimesConstantParams : ArithmeticBaseParams<Product, ILength, Single>
{
    public LengthTimesConstantParams() : base(new(2, 4, 8), new(10, 4, 40))
    {
    }
    
    public override Func<ILength, Single, Product> Factory => (a, b) => a * b;
}


public class ConstantTimesLengthParams : ArithmeticBaseParams<Product, Single, ILength>
{
    public ConstantTimesLengthParams() : base(new(2, 4, 8), new(2, 5, 10))
    {
    }
    
    public override Func<Single, ILength, Product> Factory => (a, b) => a * b;
}


// Divide
//----------------------------------------------------------------------------------------------
public class DivideLengthLengthParams : ArithmeticBaseParams<Product, ILength, ILength>
{
    public DivideLengthLengthParams() : base(new(24, 6, 4), new(30, 8, 3.75f))
    {
    }
    
    public override Func<ILength, ILength, Product> Factory => Product.Divide;
}


public class DivideLengthConstantParams : ArithmeticBaseParams<Product, ILength, Single>
{
    public DivideLengthConstantParams() : base(new(24, 6, 4), new(30, 6, 5))
    {
    }
    
    public override Func<ILength, Single, Product> Factory => Product.Divide;
}


public class LengthDivideByLengthParams : ArithmeticBaseParams<Product, ILength, ILength>
{
    public LengthDivideByLengthParams() : base(new(24, 6, 4), new(30, 8, 3.75f))
    {
    }
    
    public override Func<ILength, ILength, Product> Factory => (a, b) => a / b;
}

public class LengthDivideByConstantParams : ArithmeticBaseParams<Product, ILength, Single>
{
    public LengthDivideByConstantParams() : base(new(24, 6, 4), new(30, 6, 5))
    {
    }
    
    public override Func<ILength, Single, Product> Factory => (a, b) => a / b;
}


public class ConstantDivideByLengthParams : ArithmeticBaseParams<Product, Single, ILength>
{
    public ConstantDivideByLengthParams() : base(new(24, 6, 4), new(24, 12, 2))
    {
    }
    
    public override Func<Single, ILength, Product> Factory => (a, b) => a / b;
}


// Secondary Multiply
//----------------------------------------------------------------------------------------------

public class SecondaryMultiplyLengthParams : ArithmeticSecondaryOpParams<Product, ILength>
{
    public SecondaryMultiplyLengthParams() : base(new(2, 3, 8, 48), new(4, 7, 3, 84))
    {
    }
    
    public override Func<ILength, ILength, Product> Factory => Product.Multiply;
    public override Func<Product, ILength, Product> SecondOperation => (sum, c) => sum * c;
}

public class SecondaryMultiplyConstantParams : ArithmeticSecondaryOpParams<Product, Single>
{
    public SecondaryMultiplyConstantParams() : base(new(2, 3, 8, 48), new(4, 7, 8, 224))
    {
    }
    
    public override Func<ILength, ILength, Product> Factory => Product.Multiply;
    public override Func<Product, Single, Product> SecondOperation => (sum, c) => sum * c;
}

// Secondary Divide
//----------------------------------------------------------------------------------------------

public class SecondaryDivideLengthParams : ArithmeticSecondaryOpParams<Product, ILength>
{
    public SecondaryDivideLengthParams() : base(new(144, 2, 6, 12), new(180, 10, 2, 9))
    {
    }
    
    public override Func<ILength, ILength, Product> Factory => Product.Divide;
    public override Func<Product, ILength, Product> SecondOperation => (sum, c) => sum / c;
}

public class SecondaryDivideConstantParams : ArithmeticSecondaryOpParams<Product, Single>
{
    public SecondaryDivideConstantParams() : base(new(144, 2, 6, 12), new(180, 10, 6, 3))
    {
    }
    
    public override Func<ILength, ILength, Product> Factory => Product.Divide;
    public override Func<Product, Single, Product> SecondOperation => (sum, c) => sum / c;
}
