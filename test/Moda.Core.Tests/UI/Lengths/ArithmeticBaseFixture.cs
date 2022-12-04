// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using System.Linq;
using FluentAssertions;
using Moda.Core.UI;
using Moda.Core.UI.Lengths;
using NUnit.Framework;

namespace Moda.Core.Tests.UI.Lengths;


public partial class ArithmeticBaseFixture<TArithmetic, TArg1, TArg2>
    where TArithmetic : Arithmetic
    where TArg1 : notnull
    where TArg2 : notnull
{
    private readonly String nameOfParameters;

    public ArithmeticBaseFixture(String nameOfParameters)
    {
        this.nameOfParameters = nameOfParameters;
    }


    
    [Test]
    public void FactoryShouldAppendLengths()
    {
        ArithmeticBaseParams<TArithmetic, TArg1, TArg2> parameters = GetParameters();
        TArg1 arg1 = parameters.Arg1;
        TArg2 arg2 = parameters.Arg2;

        TArithmetic arithmetic = parameters.Factory(arg1, arg2);
        
        arithmetic.Lengths.Should().BeEquivalentTo(
            new Object[] { arg1, arg2 }.OfType<Length>()
        );
    }

    [Test]
    public void CalculateShouldReturnExpectedResult()
    {
        ArithmeticBaseParams<TArithmetic, TArg1, TArg2> parameters = GetParameters();
        TArg1 arg1 = parameters.Arg1;
        TArg2 arg2 = parameters.Arg2;

        TArithmetic arithmetic = parameters.Factory(arg1, arg2);
        arithmetic.Calculate().Should().BeApproximately(parameters.InitialResult, 0.001f);
    }
    
    
    [Test]
    public void ReCalculateShouldReflectNewLengthValues()
    {
        ArithmeticBaseParams<TArithmetic, TArg1, TArg2> parameters = GetParameters();
        TArg1 arg1 = parameters.Arg1;
        TArg2 arg2 = parameters.Arg2;

        TArithmetic arithmetic = parameters.Factory(arg1, arg2);
        arithmetic.Calculate();

        parameters.ChangeLengths();

        arithmetic.Calculate().Should().BeApproximately(parameters.UpdatedResult, 0.001f);
    }
    
    
    private ArithmeticBaseParams<TArithmetic, TArg1, TArg2> GetParameters()
    {
        Type type = this.GetType().Assembly
            .GetTypes()
            .Single(a => a.Name == this.nameOfParameters);
        return (ArithmeticBaseParams<TArithmetic, TArg1, TArg2>)(Activator.CreateInstance(type)
            ?? throw new InvalidOperationException());
    }
    
}
