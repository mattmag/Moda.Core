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
using Moq;
using NUnit.Framework;

namespace Moda.Core.Tests.UI.Lengths;



public partial class ArithmeticSecondaryOpFixture<TArithmetic, TArg>
    where TArithmetic : Arithmetic
    where TArg : notnull
{
    private readonly String nameOfParameters;

    public ArithmeticSecondaryOpFixture(String nameOfParameters)
    {
        this.nameOfParameters = nameOfParameters;
    }


    
    [Test]
    public void OperatorShouldAppendLengths()
    {
        ArithmeticSecondaryOpParams<TArithmetic, TArg> parameters = GetParameters();
        
        Length length1 = Mock.Of<Length>();
        Length length2 = Mock.Of<Length>();

        TArithmetic arithmetic = parameters.Factory(length1, length2);
        parameters.SecondOperation(arithmetic, parameters.Arg3);
        
        arithmetic.Lengths.Should().BeEquivalentTo(
            new Object[] { length1, length1, parameters.Arg3 }.OfType<Length>()
        );
    }

    [Test]
    public void CalculateShouldReturnExpectedResult()
    {
        ArithmeticSecondaryOpParams<TArithmetic, TArg> parameters = GetParameters();
        
        Length length1 = parameters.Arg1;
        Length length2 = parameters.Arg2;
        
        TArithmetic arithmetic = parameters.Factory(length1, length2);
        parameters.SecondOperation(arithmetic, parameters.Arg3);

        arithmetic.Calculate().Should().BeApproximately(parameters.InitialResult, 0.001f);
    }
    
    
    [Test]
    public void ReCalculateShouldReflectNewLengthValues()
    {
        ArithmeticSecondaryOpParams<TArithmetic, TArg> parameters = GetParameters();
        
        Length length1 = parameters.Arg1;
        Length length2 = parameters.Arg2;
        
        TArithmetic arithmetic = parameters.Factory(length1, length2);
        parameters.SecondOperation(arithmetic, parameters.Arg3);

        parameters.ChangeLengths();

        arithmetic.Calculate().Should().BeApproximately(parameters.UpdatedResult, 0.001f);
    }
    
    
    private ArithmeticSecondaryOpParams<TArithmetic, TArg> GetParameters()
    {
        Type type = this.GetType().Assembly
            .GetTypes()
            .Single(a => a.Name == this.nameOfParameters);
        return (ArithmeticSecondaryOpParams<TArithmetic, TArg>)(Activator.CreateInstance(type)
            ?? throw new NullReferenceException());
    }
    
}

