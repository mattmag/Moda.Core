// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.UI.Lengths;



public delegate Single Operation(Single running, Single component);


public class Sum : Arithmetic
{
    private static readonly Operation ADD = (a, b) => a + b;
    private static readonly Operation SUBTRACT = (a, b) => a - b;
    

    private Sum(ILength lengthA, ILength lengthB, Operation operation)
    {
        this.AddLength(lengthA);
        this.AddLength(lengthB);
        this.AddOperation(operation);
    }

    private Sum(ILength length, Single constant, Operation operation)
    {
        this.AddLength(length);
        this.AddConstant(constant);
        this.AddOperation(operation);
    }
    
    private Sum(Single constant, ILength length, Operation operation)
    {
        this.AddConstant(constant);
        this.AddLength(length);
        this.AddOperation(operation);
    }
    

    public static Sum Add(ILength lengthA, ILength lengthB) => new(lengthA, lengthB, ADD);
    public static Sum Add(ILength lengthA, Single constant) => new(lengthA, constant, ADD);
    public static Sum Subtract(ILength lengthA, ILength lengthB) => new(lengthA, lengthB, SUBTRACT);
    public static Sum Subtract(ILength length, Single constant) => new(length, constant, SUBTRACT);
    public static Sum Subtract(Single constant, ILength length) => new(constant, length, SUBTRACT);
    
    
    public static Sum operator +(Sum sum, ILength length)
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
    
    public static Sum operator -(Sum sum, ILength length)
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



