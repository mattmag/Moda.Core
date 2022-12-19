// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using Moda.Core.UI;
using Moq;

namespace Moda.Core.Tests.UI.Lengths;



public record OperationSet2(Single A, Single B, Single Result);




public abstract class ArithmeticBaseParams<TArithmetic, TArg1, TArg2>
    where TArg1 : notnull
    where TArg2 : notnull
{
    private readonly OperationSet2 updatedSet;


    public ArithmeticBaseParams(OperationSet2 initialSet, OperationSet2 updatedSet)
    {
        this.updatedSet = updatedSet;
        this.Arg1 = CreateArg<TArg1>(initialSet.A);
        this.Arg2 = CreateArg<TArg2>(initialSet.B);
        this.InitialResult = initialSet.Result;
        this.UpdatedResult = updatedSet.Result;
    }
    
    public abstract Func<TArg1, TArg2, TArithmetic> Factory { get; }
    
    public TArg1 Arg1 { get; }
    public TArg2 Arg2 { get; }
    
    public Single InitialResult { get; }
    public Single UpdatedResult { get; }


    public void ChangeLengths()
    {
        if (Arg1 is ILength mockA)
        {
            Mock.Get(mockA).Setup(a => a.Calculate()).Returns(this.updatedSet.A);
        }
        if (Arg2 is ILength mockB)
        {
            Mock.Get(mockB).Setup(a => a.Calculate()).Returns(this.updatedSet.B);
        }
    }

    
    protected static T CreateArg<T>(Single val)
    {
        if (typeof(T) == typeof(Length))
        {
            Mock<Length> lengthA = new();
            lengthA.Setup(a => a.Calculate()).Returns(val);
            return (T)(object)lengthA.Object;
        }
        if (typeof(T) == typeof(ILength))
        {
            Mock<ILength> lengthA = new();
            lengthA.Setup(a => a.Calculate()).Returns(val);
            return (T)(object)lengthA.Object;
        }
        if (typeof(T) == typeof(Single))
        {
            return (T)(Object)val;
        }

        throw new ArgumentException("Type not supported");
    }
}
