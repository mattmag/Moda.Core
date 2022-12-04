// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using Moda.Core.UI;
using Moq;

namespace Moda.Core.Tests.UI.Lengths;


public record OperationSet3(Single A, Single B, Single C, Single Result)
    : OperationSet2(A, B, Result);



public abstract class ArithmeticSecondaryOpParams<TArithmetic, TArg3>
    : ArithmeticBaseParams<TArithmetic, ILength, ILength>
{
    private readonly OperationSet3 updatedSet;


    protected ArithmeticSecondaryOpParams(OperationSet3 initialSet, OperationSet3 updatedSet)
        : base(initialSet, updatedSet)
    {
        this.updatedSet = updatedSet;
        this.Arg3 = CreateArg<TArg3>(initialSet.C);
    }


    public new void ChangeLengths()
    {
        base.ChangeLengths();
        if (Arg3 is ILength mockC)
        {
            Mock.Get(mockC).Setup(a => a.Calculate()).Returns(this.updatedSet.C);
        }
    }
    
    public abstract Func<TArithmetic, TArg3, TArithmetic> SecondOperation { get; }

    public TArg3 Arg3 { get; }
}
