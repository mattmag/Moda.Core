// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.UI.Lengths;
using Moda.Core.Utility.Data;
using Optional;
using Optional.Unsafe;

namespace Moda.Core.UI;

public interface ILength : ICalculation
{
    public static Sum operator +(ILength lengthA, ILength lengthB) =>
        Sum.Add(lengthA, lengthB);
    
    public static Sum operator +(ILength length, Single constant) =>
        Sum.Add(length, constant);
    
    public static Sum operator +(Single constant, ILength length) =>
        Sum.Add(length, constant);
    
    public static Sum operator -(ILength lengthA, ILength lengthB) =>
        Sum.Subtract(lengthA, lengthB);
    
     public static Sum operator -(ILength length, Single constant) =>
        Sum.Subtract(length, constant);
    
    public static Sum operator -(Single constant, ILength length) =>
        Sum.Subtract(constant, length);
    
    public static Product operator *(ILength lengthA, ILength lengthB) =>
        Product.Multiply(lengthA, lengthB);
    
    public static Product operator *(ILength lengthA, Single constant) =>
        Product.Multiply(lengthA, constant);
    
    public static Product operator *(Single constant, ILength length) =>
        Product.Multiply(length, constant);
    
    public static Product operator /(ILength lengthA, ILength lengthB) =>
        Product.Divide(lengthA, lengthB);
    
     public static Product operator /(ILength length, Single constant) =>
        Product.Divide(length, constant);
    
    public static Product operator /(Single constant, ILength length) =>
        Product.Divide(constant, length);
    
}

public abstract class Length : ILength, ICalculation
{
    
    // explicitly relay to ILength operators as the compiler won't do it for us in some cases
    // because "classes that implement an interface do not inherit it's members"
    // other base classes may need to do the same, or cast to ILength as required during arithmetic
    
    public static Sum operator +(Length lengthA, ILength lengthB) => (ILength)lengthA + lengthB;
    
    public static Sum operator +(Length length, Single constant) => (ILength)length + constant;

    public static Sum operator +(Single constant, Length length) => constant + (ILength)length;

    
    public static Sum operator -(Length lengthA, ILength lengthB) => (ILength)lengthA - lengthB;
    
    public static Sum operator -(Length length, Single constant) => (ILength)length - constant;
    
    public static Sum operator -(Single constant, Length length) => constant - (ILength)length;


    public static Product operator *(Length lengthA, ILength lengthB) => (ILength)lengthA * lengthB;

    public static Product operator *(Length length, Single constant) => (ILength)length * constant;

    public static Product operator *(Single constant, Length length) => constant * (ILength)length;

    public static Product operator /(Length lengthA, ILength lengthB) => (ILength)lengthA / lengthB;

    public static Product operator /(Length length, Single constant) => (ILength)length / constant;

    public static Product operator /(Single constant, Length length) => constant / (ILength)length;

    

    public void Initialize(Cell owner, Axis axis)
    {
        this.IsInitialized = true;
        this._owner = owner.Some();
        this._axis = axis.Some();
        OnInitialize(owner, axis);
    }

    // TODO: unit test these
    // TODO: move to ILength

    public Boolean IsInitialized { get; private set; }
    
    protected Option<Axis> _axis { get; private set; }
    protected Axis Axis => this._axis.ValueOrFailure();
    
    protected Option<Cell> _owner { get; private set; }
    protected Cell Owner => this._owner.ValueOrFailure();
    
    
    public abstract Single Calculate();

    // public abstract Option<Single> TryCalculate();
    
    private HashSet<Coordinate> _prerequisites = new();
    public IEnumerable<Coordinate> Prerequisites => this._prerequisites;
    
    public event NotificationHandler<ICalculation>? ValueInvalidated;


    protected virtual void OnInitialize(Cell owner, Axis axis)
    {
        
    }
    
    protected void RaiseValueInvalidated()
    {
        this.ValueInvalidated?.Invoke(this);
    }


    protected void ModifyPrerequisites(IEnumerable<Coordinate> add, IEnumerable<Coordinate> remove)
    {
        HashSet<Coordinate> actuallyAdded = new();
        HashSet<Coordinate> actuallyRemoved = new();

        foreach (Coordinate coordinate in remove.Except(add))
        {
            if (this._prerequisites.Remove(coordinate))
            {
                actuallyRemoved.Add(coordinate);
            }
        }
        
        foreach (Coordinate coordinate in add)
        {
            if (this._prerequisites.Add(coordinate))
            {
                actuallyAdded.Add(coordinate);
            }
        }
        
        if (actuallyRemoved.Any() || actuallyAdded.Any())
        {
            RaisePrerequistesChanged(actuallyAdded, actuallyRemoved);
        }
    }
    
    public event CollectionChangedHandler<ICalculation, Coordinate>? PrerequisitesChanged;


    protected void RaisePrerequistesChanged(IEnumerable<Coordinate> added,
        IEnumerable<Coordinate> removed)
    {
        this.PrerequisitesChanged?.Invoke(this, new(added, removed));
    }
}

