// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.UI.Lengths;
using Moda.Core.Utility.Data;

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


    
    public abstract Single Calculate();

    public abstract IEnumerable<Coordinate> Prerequisites { get; }
    
    public event NotificationHandler<ICalculation>? ValueInvalidated;


    protected void RaiseValueInvalidated()
    {
        this.ValueInvalidated?.Invoke(this);
    }
    
    public event CollectionChangedHandler<ICalculation, Coordinate>? PrerequisitesChanged;


    protected void RaisePrerequistesChanged(IEnumerable<Coordinate> added,
        IEnumerable<Coordinate> removed)
    {
        this.PrerequisitesChanged?.Invoke(this, new(added, removed));
    }
}

