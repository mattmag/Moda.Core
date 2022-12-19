// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using Moda.Core.UI;
using Moda.Core.UI.Lengths;
using Moq;
using NUnit.Framework;

namespace Moda.Core.Tests.UI.Lengths;

public class CompositeLengthTests
{
    [Test]
    public void AddLengthShouldInitializeIfCompositeIsInitialized()
    {
        MockComposite composite = new();
        Cell owningCell = new Cell(Mock.Of<IHoneyComb>(), Mock.Of<ICalculation>(),
            Mock.Of<ICalculation>(), Mock.Of<ICalculation>(), Mock.Of<ICalculation>());
        composite.Initialize(owningCell, Axis.Y);
        ILength length = Mock.Of<ILength>();
        composite.AddLength(length);
        Mock.Get(length).Verify(a => a.Initialize(
            It.Is<Cell>(b => b == owningCell), It.Is<Axis>(b => b == Axis.Y)),
            Times .Once());
    }
    
    public class MockComposite : CompositeLength
    {
        public void AddLength(ILength length)
        {
            base.AddLength(length);    
        }
        
        public override Single Calculate()
        {
            throw new NotImplementedException();
        }
    }
}
