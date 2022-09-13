// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Moda.Core.Utility.Data.Tests;

[TestFixture]
public class EqualityTests
{
    //  NullSafeEquals Tests
    //----------------------------------------------------------------------------------------------
    
    [Test]
    public void NullSafeEqualsShouldReturnFalseIfOneObjectIsNull()
    {
        SimpleClass? objectA = new();
        SimpleClass? objectB = null;
        Equality.NullSafeEquals(objectA, objectB).Should().BeFalse();
        Equality.NullSafeEquals(objectB, objectA).Should().BeFalse();
    }

    [Test]
    public void NullSafeEqualsShouldReturnTrueIfBothObjectsAreNull()
    {
        SimpleClass? objectA = null;
        SimpleClass? objectB = null;
        Equality.NullSafeEquals(objectA, objectB).Should().BeTrue();
        Equality.NullSafeEquals(objectB, objectA).Should().BeTrue();
    }

    [Test]
    public void NullSafeEqualsShouldReturnTrueIfBothObjectsAreTheSameInstance()
    {
        SimpleClass objectA = new();
        SimpleClass objectB = objectA;
        Equality.NullSafeEquals(objectA, objectB).Should().BeTrue();
        Equality.NullSafeEquals(objectB, objectA).Should().BeTrue();
    }

    [Test]
    public void NullSafeEqualsShouldReturnFalseIfEqualMethodFails()
    {
        SimpleClass objectA = new() { IntegerProperty = 1, StringProperty = "ABC" };
        SimpleClass objectB = new() { IntegerProperty = 2, StringProperty = "DEF" };
        Equality.NullSafeEquals(objectA, objectB).Should().BeFalse();
        Equality.NullSafeEquals(objectB, objectA).Should().BeFalse();
    }

    [Test]
    public void NullSafeEqualsShouldReturnTrueIfEqualsMethodSucceeds()
    {
        SimpleClass objectA = new() { IntegerProperty = 1, StringProperty = "ABC" };
        SimpleClass objectB = new() { IntegerProperty = 1, StringProperty = "ABC" };
        Equality.NullSafeEquals(objectA, objectB).Should().BeTrue();
        Equality.NullSafeEquals(objectB, objectA).Should().BeTrue();
    }

    [Test]
    public void NullSafeEqualsShouldAcceptStructs()
    {
        SimpleStruct objectA = new()
        {
            IntegerProperty = 1,
            StringProperty = "ABC",
        };
        SimpleStruct objectB = new()
        {
            IntegerProperty = 1,
            StringProperty = "ABC",
        };
        Equality.NullSafeEquals(objectA, objectB).Should().BeTrue();
        Equality.NullSafeEquals(objectB, objectA).Should().BeTrue();
    }


    //  CheckFromOverride Tests
    //------------------------------------------------------------------------------------------

    [Test]
    public void CheckFromOverrideShouldReturnFalseIfObjectsAreDifferentTypes()
    {
        EquatableClassX objectA = new();
        EquatableClassY objectB = new();
        Equality.CheckFromOverride(objectA, objectB).Should().BeFalse();
    }

    [Test]
    public void CheckFromOverrideShouldReturnFalseIfStructsAreDifferentTypes()
    {
        EquatableStructX objectA = new();
        EquatableStructY objectB = new();
        Equality.CheckFromOverride(objectA, objectB).Should().BeFalse();
    }

    [Test]
    public void CheckFromOverrideShouldReturnFalseIfOneObjectIsNull()
    {
        EquatableClassX? objectA = new();
        EquatableClassX? objectB = null;
        Assert.False(Equality.CheckFromOverride(objectA, objectB), "A,B");
        Assert.False(Equality.CheckFromOverride(objectB, objectA), "B,A");
    }

    [Test]
    public void CheckFromOverrideShouldReturnTrueIfBothObjectsAreNull()
    {
        EquatableClassX? objectA = null;
        EquatableClassX? objectB = null;
        Assert.True(Equality.CheckFromOverride(objectA, objectB), "A,B");
        Assert.True(Equality.CheckFromOverride(objectB, objectA), "B,A");
    }

    [Test]
    public void CheckFromOverrideShouldCallEquatableEqualsWithOtherClassAsTheParameter()
    {
        EquatableClassX objectA = new();
        EquatableClassX objectB = new();

        Equality.CheckFromOverride(objectA, objectB);

        ReferenceEquals(objectB, objectA.EqualsOtherParamater).Should().BeTrue();
        objectA.EqualsCallCount.Should().Be(1);
    }

    [Test]
    public void CheckFromOverrideShouldCallEquatableEqualsWithOtherStructAsTheParameter()
    {
        Object otherFromEquatable = new Object();
        Int32 callCount = 0;
        void equatableHandler(Object other)
        {
            callCount++;
            otherFromEquatable = (EquatableStructX)other;
        }

        EquatableStructX objectA = new()
        {
            IntegerProperty = 1,
            StringProperty = "ABC",
            EquatableHandler = equatableHandler
        };
        EquatableStructX objectB = new()
        {
            IntegerProperty = 2,
            StringProperty = "DEF"
        };

        Equality.CheckFromOverride(objectA, objectB);

        // equals for this class just returns the EqualsReturnValue, so do a quick compare here to
        // ensure that we have the right instance
        otherFromEquatable.Should().BeOfType<EquatableStructX>();
        EquatableStructX otherResult = ((EquatableStructX)otherFromEquatable);
        otherResult.IntegerProperty.Should().Be(objectB.IntegerProperty);
        otherResult.StringProperty.Should().Be(objectB.StringProperty);
        callCount.Should().Be(1);
    }

    [Test]
    public void CheckFromOverrideShouldReturnResultsFromClassEquatable()
    {
        EquatableClassX objectA = new();
        EquatableClassX objectB = new();
        
        objectA.EqualsReturnValue = true;
        Equality.CheckFromOverride(objectA, objectB).Should().BeTrue();

        objectA.EqualsReturnValue = false;
        Equality.CheckFromOverride(objectA, objectB).Should().BeFalse();
    }

    [Test]
    public void CheckFromOverrideShouldReturnResultsFromStructEquatable()
    {
        EquatableStructX objectA = new();
        EquatableStructX objectB = new();

        objectA.EqualsReturnValue = true;
        Equality.CheckFromOverride(objectA, objectB).Should().BeTrue();

        objectA.EqualsReturnValue = false;
        Equality.CheckFromOverride(objectA, objectB).Should().BeFalse();
    }

    //  CheckFromOperator Tests
    //------------------------------------------------------------------------------------------
    
    [Test]
    public void CheckFromOperatorShouldReturnFalseIfOneObjectIsNull()
    {
        EquatableClassX objectA = new();
        EquatableClassX objectB = null;
        Equality.CheckFromOperator(objectA, objectB).Should().BeFalse();
        Equality.CheckFromOperator(objectB, objectA).Should().BeFalse();
    }

    [Test]
    public void CheckFromOperatorShouldReturnTrueIfBothObjectsAreNull()
    {
        EquatableClassX? objectA = null;
        EquatableClassX? objectB = null;
        Equality.CheckFromOperator(objectA, objectB).Should().BeTrue();
        Equality.CheckFromOperator(objectB, objectA).Should().BeTrue();
    }

    [Test]
    public void CheckFromOperatorShouldCallEquatableEqualsWithOtherClassAsTheParameter()
    {
        EquatableClassX objectA = new();
        EquatableClassX objectB = new();

        Equality.CheckFromOperator(objectA, objectB);

        ReferenceEquals(objectB, objectA.EqualsOtherParamater).Should().BeTrue();
        objectA.EqualsCallCount.Should().Be(1);
    }

    [Test]
    public void CheckFromOperatorShouldCallEquatableEqualsWithOtherStructAsTheParameter()
    {
        Object otherFromEquatable = new Object();
        Int32 callCount = 0;
        void equatableHandler(Object other)
        {
            callCount++;
            otherFromEquatable = (EquatableStructX)other;
        }

        EquatableStructX objectA = new()
        {
            IntegerProperty = 1,
            StringProperty = "ABC",
            EquatableHandler = equatableHandler
        };

        EquatableStructX objectB = new()
        {
            IntegerProperty = 2,
            StringProperty = "DEF"
        };

        Equality.CheckFromOperator(objectA, objectB);

        // equals for this class just returns the EqualsReturnValue, so do a quick compare here to
        // ensure that we have the right instance
        otherFromEquatable.Should().BeOfType<EquatableStructX>();
        EquatableStructX otherResult = ((EquatableStructX)otherFromEquatable);
        otherResult.IntegerProperty.Should().Be(objectB.IntegerProperty);
        otherResult.StringProperty.Should().Be(objectB.StringProperty);
        callCount.Should().Be(1);
    }

    [Test]
    public void CheckFromOperatorShouldReturnResultsFromClassEquatable()
    {
        EquatableClassX objectA = new();
        EquatableClassX objectB = new();

        objectA.EqualsReturnValue = true;
        Equality.CheckFromOperator(objectA, objectB).Should().BeTrue();

        objectA.EqualsReturnValue = false;
        Equality.CheckFromOperator(objectA, objectB).Should().BeFalse();
    }

    [Test]
    public void CheckFromOperatorShouldReturnResultsFromStructEquatable()
    {
        EquatableStructX objectA = new();
        EquatableStructX objectB = new();

        objectA.EqualsReturnValue = true;
        Equality.CheckFromOperator(objectA, objectB).Should().BeTrue();

        objectA.EqualsReturnValue = false;
        Equality.CheckFromOperator(objectA, objectB).Should().BeFalse();
    }


    //  CheckFromIEquatable Tests
    //------------------------------------------------------------------------------------------
    
    [Test]
    public void CheckFromIEquatableShouldReturnFalseIfOneObjectIsNull()
    {
        EquatableClassX objectA = new();
        EquatableClassX objectB = null;
        Equality.CheckFromIEquatable(objectA, objectB).Should().BeFalse();
        Equality.CheckFromIEquatable(objectB, objectA).Should().BeFalse();
    }

    [Test]
    public void CheckFromIEquatableShouldReturnTrueIfBothObjectsAreNull()
    {
        EquatableClassX objectA = null;
        EquatableClassX objectB = null;
        Equality.CheckFromIEquatable(objectA, objectB).Should().BeTrue();
        Equality.CheckFromIEquatable(objectB, objectA).Should().BeTrue();
    }

    [Test]
    public void CheckFromIEquatableReturnTrueIfIncludedPropertiesAreEqual()
    {
        EquatableClassX objectA = new()
        {
            IntegerProperty = 1,
            StringProperty = "ABC",
        };
        EquatableClassX objectB = new()
        {
            IntegerProperty = 1,
            StringProperty = "ABC",
        };

        Equality.CheckFromIEquatable(objectA, objectB,
                a => a.IntegerProperty,
                a => a.StringProperty)
            .Should().BeTrue();

        Equality.CheckFromIEquatable(objectA, objectB,
                a => a.IntegerProperty,
                a => a.StringProperty)
            .Should().BeTrue();
    }

    [Test]
    public void CheckFromIEquatableReturnFalseIfAnyIncludedPropertiesAreNotEqual()
    {
        EquatableClassX objectA = new()
        {
            IntegerProperty = 1,
            StringProperty = "ABC",
        };
        EquatableClassX objectB = new()
        {
            IntegerProperty = 2,
            StringProperty = "ABC",
        };

        Equality.CheckFromIEquatable(objectA, objectB,
                a => a.IntegerProperty, a => a.StringProperty)
            .Should().BeFalse();
        Equality.CheckFromIEquatable(objectA, objectB,
                a => a.IntegerProperty, a => a.StringProperty)
            .Should().BeFalse();
    }

    [Test]
    public void CheckFromIEquatableShouldIgnoreUnincludedProperties()
    {
        EquatableClassX objectA = new()
        {
            IntegerProperty = 1,
            StringProperty = "ABC",
        };
        EquatableClassX objectB = new()
        {
            IntegerProperty = 2,
            StringProperty = "ABC",
        };

        Equality.CheckFromIEquatable(objectA, objectB, a => a.StringProperty)
            .Should().BeTrue();
        Equality.CheckFromIEquatable(objectA, objectB, a => a.StringProperty)
            .Should().BeTrue();
    }


    //  Support
    //------------------------------------------------------------------------------------------

    public class EquatableClassX : IEquatable<EquatableClassX>
    {
        public Int32 IntegerProperty { get; set; }

        public String StringProperty { get; set; }

        public Boolean EqualsReturnValue { get; set; }

        public Int32 EqualsCallCount { get; set; }

        public Object EqualsOtherParamater { get; set; }

        public Boolean Equals(EquatableClassX other)
        {
            this.EqualsOtherParamater = other;
            this.EqualsCallCount++;

            return this.EqualsReturnValue;

        }
    }

    public class EquatableClassY : IEquatable<EquatableClassY>
    {
        public Int32 IntegerProperty { get; set; }

        public String StringProperty { get; set; }

        public Boolean EqualsReturnValue { get; set; }

        public Int32 EqualsCallCount { get; set; }

        public Object EqualsOtherParamater { get; set; }

        public Boolean Equals(EquatableClassY other)
        {
            this.EqualsOtherParamater = other;
            this.EqualsCallCount++;

            return this.EqualsReturnValue;
        }
    }

    

    private class SimpleClass
    {
        public Int32 IntegerProperty { get; set; }

        public String StringProperty { get; set; }

        public override Boolean Equals(Object other)
        {
            if (other is SimpleClass otherSimpleObject)
            {
                return this.IntegerProperty == otherSimpleObject.IntegerProperty &&
                    this.StringProperty == otherSimpleObject.StringProperty;
            }

            return false;
        }

        public override Int32 GetHashCode()
        {
            return base.GetHashCode();
        }
    }


    private struct SimpleStruct
    {
        public Int32 IntegerProperty { get; set; }

        public String StringProperty { get; set; }
    }

    public struct EquatableStructX : IEquatable<EquatableStructX>
    {
        public Int32 IntegerProperty { get; set; }

        public String StringProperty { get; set; }

        public Boolean EqualsReturnValue { get; set; }

        public Action<Object> EquatableHandler{ get; set; }

        public Boolean Equals(EquatableStructX other)
        {
            this.EquatableHandler?.Invoke(other);

            return this.EqualsReturnValue;
        }
    }

    public struct EquatableStructY : IEquatable<EquatableStructY>
    {
        public Boolean EqualsReturnValue { get; set; }

        public Action<Object> EquatableHandler { get; set; }

        public Boolean Equals(EquatableStructY other)
        {
            this.EquatableHandler(other);

            return this.EqualsReturnValue;
        }
    }
}
