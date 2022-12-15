// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using Moda.Core.UI;
using Moda.Core.UI.Builders;
using Moda.Core.UI.Lengths;
using Moq;
using NUnit.Framework;

namespace Moda.Core.Tests.UI.Builders;

[TestFixture(typeof(AnchoredHorizontalBuilder), typeof(HAnchor),
    nameof(AnchoredHorizontalBuilder.WithWidth))]
[TestFixture(typeof(AnchoredVerticalBuilder), typeof(VAnchor),
    nameof(AnchoredVerticalBuilder.WithHeight))]
public class AnchoredAxisBuilderFixture<TBuilder, TAnchor>
    where TBuilder : AnchoredAxisBuilder
{
    private static Func<TAnchor, TBuilder> factory => anchor =>
        (TBuilder)(Activator.CreateInstance(typeof(TBuilder), new CellBuilder(), anchor)
            ?? throw new());

    private MethodInfo SetLengthMethod;
    private MethodInfo OffsetByMethod;

    
    public AnchoredAxisBuilderFixture(string nameOfSetLengthMethod)
    {
        this.SetLengthMethod = (typeof(TBuilder)
            .GetMethod(nameOfSetLengthMethod, new[] { typeof(Length) }) ?? throw new());
        this.OffsetByMethod = (typeof(TBuilder)
            .GetMethod("OffsetBy", new[] { typeof(Length) }) ?? throw new());
    }
        
        
    // Constructor Tests
    //----------------------------------------------------------------------------------------------
    
    [TestCaseSource(nameof(AnchorConversionData))]
    public NAnchor ConstructorShouldSetAndConvertAnchor(HAnchor anchor)
    {
        CellBuilder cellBuilder = new();
        AnchoredHorizontalBuilder axisBuilder = new(cellBuilder, anchor);
        return axisBuilder.Anchor;
    }
    
    public static IEnumerable<TestCaseData> AnchorConversionData()
    {
        if (typeof(TAnchor) == typeof(HAnchor))
        {
            return new[]
            {
                new TestCaseData(HAnchor.Left).Returns(NAnchor.Alpha),
                new TestCaseData(HAnchor.Center).Returns(NAnchor.Center),
                new TestCaseData(HAnchor.Right).Returns(NAnchor.Beta),
            };
        }

        if (typeof(TAnchor) == typeof(VAnchor))
        {
            return new[]
                {
                    new TestCaseData(VAnchor.Up).Returns(NAnchor.Alpha),
                    new TestCaseData(VAnchor.Middle).Returns(NAnchor.Center),
                    new TestCaseData(VAnchor.Down).Returns(NAnchor.Beta),
                };
        }

        throw new ArgumentException();
    }


    // With{Length}() Tests
    //----------------------------------------------------------------------------------------------
    
    [TestCaseSource(nameof(SetBoundaryData))]
    public void AnchorAtWithLengthShouldSetBoundaryCoordinates(TAnchor anchor,
        Single expectedAlpha, Single expectedBeta)
    {
        Cell parent = GetParent();
        
        TBuilder axisBuilder = factory(anchor);
        this.SetLengthMethod.Invoke(axisBuilder, new Object?[] { new Pixels(60) });

        Cell child = GetChild(axisBuilder.AxisRecipe.Alpha.Get(),
            axisBuilder.AxisRecipe.Beta.Get());
        parent.AppendChild(child);
        LayoutAllCoordinates(parent);

        child.GetBoundary(GetAxis()).AlphaCoordinate.Calculation.Calculate()
            .Should().Be(expectedAlpha);
        child.GetBoundary(GetAxis()).BetaCoordinate.Calculation.Calculate()
            .Should().Be(expectedBeta);
    }
    
    public static IEnumerable<TestCaseData> SetBoundaryData()
    {
        if (typeof(TAnchor) == typeof(HAnchor))
        {
            return new[]
            {
                new TestCaseData(HAnchor.Left, 0, 60),
                new TestCaseData(HAnchor.Center, 45, 105),
                new TestCaseData(HAnchor.Right, 90, 150),
            };
        }

        if (typeof(TAnchor) == typeof(VAnchor))
        {
            return new[]
                {
                    new TestCaseData(VAnchor.Up, 0, 60),
                    new TestCaseData(VAnchor.Middle, 45, 105),
                    new TestCaseData(VAnchor.Down, 90, 150),
                };
        }

        throw new ArgumentException();
    }
    


    // OffsetBy() Tests
    //----------------------------------------------------------------------------------------------
    
    [TestCaseSource(nameof(SetBoundaryDataWithOffset))]
    public void AnchorAtWithOffsetAndLengthShouldSetBoundaryCoordinates(TAnchor anchor,
        Single offset, Single expectedAlpha, Single expectedBeta)
    {
        Cell parent = GetParent();
        
        TBuilder axisBuilder = factory(anchor);
        this.OffsetByMethod.Invoke(axisBuilder, new Object?[] { Len(offset) });
        this.SetLengthMethod.Invoke(axisBuilder, new Object?[] { Len(60) });

        Cell child = GetChild(axisBuilder.AxisRecipe.Alpha.Get(),
            axisBuilder.AxisRecipe.Beta.Get());
        parent.AppendChild(child);
        LayoutAllCoordinates(parent);

        child.GetBoundary(GetAxis()).AlphaCoordinate.Calculation.Calculate()
            .Should().Be(expectedAlpha);
        child.GetBoundary(GetAxis()).BetaCoordinate.Calculation.Calculate()
            .Should().Be(expectedBeta);
    }
    
    public static IEnumerable<TestCaseData> SetBoundaryDataWithOffset()
    {
        if (typeof(TAnchor) == typeof(HAnchor))
        {
            return new[]
            {
                new TestCaseData(HAnchor.Left, 5, 5, 65),
                new TestCaseData(HAnchor.Center, 5, 50, 110),
                new TestCaseData(HAnchor.Right, 5, 95, 155),
            };
        }

        if (typeof(TAnchor) == typeof(VAnchor))
        {
            return new[]
                {
                    new TestCaseData(VAnchor.Up, 5, 5, 65),
                    new TestCaseData(VAnchor.Middle, 5, 50, 110),
                    new TestCaseData(VAnchor.Down, 5, 95, 155),
                };
        }

        throw new ArgumentException();
    }
    
    
    // Support
    //----------------------------------------------------------------------------------------------
    
    private Axis GetAxis() => typeof(TAnchor) == typeof(HAnchor) ? Axis.X : Axis.Y;

    private Cell GetParent() => GetAxis() switch
        {
            Axis.X => new(Mock.Of<IHoneyComb>(), Len(50), Len(200), Len(0), Len(0)),
            Axis.Y => new(Mock.Of<IHoneyComb>(), Len(0), Len(0), Len(50), Len(200)),
            _ => throw new ArgumentException()
        };


    private Cell GetChild(Length alpha, Length beta) => GetAxis() switch
        {
            Axis.X => new(Mock.Of<IHoneyComb>(), alpha, beta, Len(0), Len(0)),
            Axis.Y => new(Mock.Of<IHoneyComb>(), Len(0), Len(0), alpha, beta),
            _ => throw new ArgumentException()
        };
    
    private void LayoutAllCoordinates(Cell cell)
    {
        foreach (Coordinate coordinate in cell.GetCoordinates())
        {
            coordinate.Calculate();
        }
    }
    
    private Length Len(Single value)
    {
        Mock<Length> length = new();
        length.Setup(a => a.Calculate()).Returns(value);
        return length.Object;
    }
}
