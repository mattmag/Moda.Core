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

public abstract class Length : ICalculation
{
    public static Sum operator +(Length length, Length lengthB) =>
        Sum.Add(length, lengthB);
    
    public static Sum operator +(Length length, Single constant) =>
        Sum.Add(length, constant);
    
    public static Sum operator +(Single constant, Length length) =>
        Sum.Add(length, constant);
    
    public static Sum operator -(Length lengthA, Length lengthB) =>
        Sum.Subtract(lengthA, lengthB);
    
     public static Sum operator -(Length length, Single constant) =>
        Sum.Subtract(length, constant);
    
    public static Sum operator -(Single constant, Length length) =>
        Sum.Subtract(constant, length);
    
    public static Product operator *(Length length, Length lengthB) =>
        Product.Multiply(length, lengthB);
    
    public static Product operator *(Length length, Single constant) =>
        Product.Multiply(length, constant);
    
    public static Product operator *(Single constant, Length length) =>
        Product.Multiply(length, constant);
    
    public static Product operator /(Length lengthA, Length lengthB) =>
        Product.Divide(lengthA, lengthB);
    
     public static Product operator /(Length length, Single constant) =>
        Product.Divide(length, constant);
    
    public static Product operator /(Single constant, Length length) =>
        Product.Divide(constant, length);


    public void Initialize(Cell owner, Axis axis)
    {
        this.Owner = owner.Some();
        this.Axis = axis.Some();
        OnInitialize(owner, axis);
    }


    protected Option<Axis> Axis;
    
    protected Option<Cell> Owner;
    
    public abstract Single Calculate();

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

