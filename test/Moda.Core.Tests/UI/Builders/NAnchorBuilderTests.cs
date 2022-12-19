// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using FluentAssertions;
using Moda.Core.UI;
using Moda.Core.UI.Builders;
using Moq;
using NUnit.Framework;

namespace Moda.Core.Tests.UI.Builders;

public class NAnchoredBuilderTests
{
    [Test]
    public void SetLengthShouldThrowExceptionWithUnknownAnchor()
    {
        MockAnchorBuilder mock = new(new(), Mock.Of<ISetLengthProcessor>(), (NAnchor)999, Axis.X);
        mock.Invoking(a => a.SetLength(Mock.Of<ILength>())).Should()
            .Throw<ArgumentOutOfRangeException>();
    }


    class MockAnchorBuilder : NAnchorBuilder
    {
        public MockAnchorBuilder(CellBuilderState runningState, ISetLengthProcessor lengthProcessor,
            NAnchor anchor, Axis axis)
            : base(runningState, lengthProcessor, anchor, axis)
        {
            
        }


        public void SetLength(ILength length)
        {
            base.SetLength(length);
        }
    }
}
