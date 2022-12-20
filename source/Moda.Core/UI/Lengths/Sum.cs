// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.UI.Lengths;



public class Sum : Arithmetic
{
    private static readonly Operation ADD = (a, b) => a + b;
    private static readonly Operation SUBTRACT = (a, b) => a - b;


    private Sum(ILength lengthA, ILength lengthB, Operation operation)
        : base(lengthA, lengthB, operation)
    {
        
    }
    
    private Sum(ILength length, Single constant, Operation operation)
        : base(length, constant, operation)
    {
        
    }
    
    private Sum(Single constant, ILength length, Operation operation)
        : base(constant, length, operation)
    {
        
    }

    public static Sum Add(ILength lengthA, ILength lengthB) => new(lengthA, lengthB, ADD);
    public static Sum Add(ILength lengthA, Single constant) => new(lengthA, constant, ADD);
    public static Sum Subtract(ILength lengthA, ILength lengthB) => new(lengthA, lengthB, SUBTRACT);
    public static Sum Subtract(ILength length, Single constant) => new(length, constant, SUBTRACT);
    public static Sum Subtract(Single constant, ILength length) => new(constant, length, SUBTRACT);



    // Addition Operators (for absorption)
    //----------------------------------------------------------------------------------------------
    
    public static Sum operator +(Sum sum, ILength length)
    {
        sum.AppendLengthComponent(length);
        sum.AppendOperation(ADD);
        return sum;
    }

    public static Sum operator +(Sum sum, Single constant)
    {
        sum.AppendConstantComponent(constant);
        sum.AppendOperation(ADD);
        return sum;
    }
    
    
    
    // Subtraction Operators (for absorption)
    //----------------------------------------------------------------------------------------------
    
    public static Sum operator -(Sum sum, ILength length)
    {
        sum.AppendLengthComponent(length);
        sum.AppendOperation(SUBTRACT);
        return sum;
    }
 
    public static Sum operator -(Sum sum, Single constant)
    {
        sum.AppendConstantComponent(constant);
        sum.AppendOperation(SUBTRACT);
        return sum;
    }
    
}



