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


    private Sum(Length lengthA, Length lengthB, Operation operation)
        : base(lengthA, lengthB, operation)
    {
        
    }
    
    private Sum(Length length, Single constant, Operation operation)
        : base(length, constant, operation)
    {
        
    }
    
    private Sum(Single constant, Length length, Operation operation)
        : base(constant, length, operation)
    {
        
    }

    public static Sum Add(Length lengthA, Length lengthB) => new(lengthA, lengthB, ADD);
    public static Sum Add(Length lengthA, Single constant) => new(lengthA, constant, ADD);
    public static Sum Subtract(Length lengthA, Length lengthB) => new(lengthA, lengthB, SUBTRACT);
    public static Sum Subtract(Length length, Single constant) => new(length, constant, SUBTRACT);
    public static Sum Subtract(Single constant, Length length) => new(constant, length, SUBTRACT);
    
    
    public static Sum operator +(Sum sum, Length length)
    {
        sum.AddLength(length);
        sum.AddOperation(ADD);
        return sum;
    }
    
    public static Sum operator +(Sum sum, Single constant)
    {
        sum.AddConstant(constant);
        sum.AddOperation(ADD);
        return sum;
    }
    
    public static Sum operator -(Sum sum, Length length)
    {
        sum.AddLength(length);
        sum.AddOperation(SUBTRACT);
        return sum;
    }
    
    public static Sum operator -(Sum sum, Single constant)
    {
        sum.AddConstant(constant);
        sum.AddOperation(SUBTRACT);
        return sum;
    }
}



