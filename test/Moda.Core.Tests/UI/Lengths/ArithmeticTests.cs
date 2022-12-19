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

namespace Moda.Core.Tests.UI.Lengths;



public class ArithmeticTests
{

    [Test]
    public void NoComponentsToCalculateShouldReturn0()
    {
        EmptyArithmetic empty = new();
        empty.Calculate().Should().Be(0);
    }
    
    
    [Test]
    public void InvalidOperationsCountShouldThrowException()
    {
        InvalidArithmetic1 invalid1 = new();
        invalid1.Invoking(a => a.Calculate()).Should().Throw<InvalidOperationException>();
        
        InvalidArithmetic2 invalid2 = new();
        invalid2.Invoking(a => a.Calculate()).Should().Throw<InvalidOperationException>();
    }


    private class EmptyArithmetic : Arithmetic
    {

    }
    
    private class InvalidArithmetic1 : Arithmetic
    {
        public InvalidArithmetic1()
        {
            this.AddConstantComponent(1);
            this.AddLengthComponent(Mock.Of<ILength>());
        }
    }
    
    private class InvalidArithmetic2 : Arithmetic
    {
        public InvalidArithmetic2()
        {
            this.AddConstantComponent(1);
            this.AddLengthComponent(Mock.Of<ILength>());
            this.AddOperation((a, b) => a + b);
            this.AddLengthComponent(Mock.Of<ILength>());
        }
    }

}
