// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.UI.Lengths;

public class Product : Arithmetic
{
    private static readonly Operation MULTIPLY = (a, b) => a * b;
    private static readonly Operation DIVIDE = (a, b) => a * (1/b);
    
    
    private Product(ILength lengthA, ILength lengthB, Operation operation)
        : base(lengthA, lengthB, operation)
    {
        
    }
    
    private Product(ILength length, Single constant, Operation operation)
        : base(length, constant, operation)
    {
        
    }
    
    private Product(Single constant, ILength length, Operation operation)
        : base(constant, length, operation)
    {
        
    }
   

    public static Product Multiply(ILength lengthA, ILength lengthB) => 
        new(lengthA, lengthB, MULTIPLY);
    public static Product Multiply(ILength lengthA, Single constant) => 
        new(lengthA, constant, MULTIPLY);
    public static Product Divide(ILength lengthA, ILength lengthB) => 
        new(lengthA, lengthB, DIVIDE);
    public static Product Divide(ILength length, Single constant) => 
        new(length, constant, DIVIDE);
    public static Product Divide(Single constant, ILength length) => 
        new(constant, length, DIVIDE);
    
    
    public static Product operator *(Product sum, ILength length)
    {
        sum.AppendLengthComponent(length);
        sum.AppendOperation(MULTIPLY);
        return sum;
    }
    
    public static Product operator *(Product sum, Single constant)
    {
        sum.AppendConstantComponent(constant);
        sum.AppendOperation(MULTIPLY);
        return sum;
    }
    
    public static Product operator /(Product sum, ILength length)
    {
        sum.AppendLengthComponent(length);
        sum.AppendOperation(DIVIDE);
        return sum;
    }
    
    public static Product operator /(Product sum, Single constant)
    {
        sum.AppendConstantComponent(constant);
        sum.AppendOperation(DIVIDE);
        return sum;
    }

}
