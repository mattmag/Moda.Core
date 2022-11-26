// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.Utility.Data;

namespace Moda.Core.UI;

public interface ILength : ICalculation
{
    
     // event EventHandler? ValueInvalidated;
     // event EventHandler<CollectionChangedArgs<Coordinate>>? PrerequisitesChanged;
    
     // abstract IEnumerable<Coordinate> Prerequisites { get; }
     // abstract Single Calculate();
    
    
    // static Add operator +(ILength lengthA, ILength lengthB) =>
    //     new Add(lengthA, lengthB);
    
    // public static Subtract operator -(Length lengthA, Length lengthB) =>
    //     new Subtract(lengthA, lengthB);
    //
    // public static Subtract operator -(Length length) => // eh? can add and subtract be combined and we add a Sign to Length
    //     new Subtract(new Pixels(0), length);
    //
    // public static DivideByConstant operator /(Length lengthA, Single constant) =>
    //     new DivideByConstant(lengthA, constant);
    
    
    
    // protected void RaiseValueInvalidated()
    // {
    //     this.ValueInvalidated?.Invoke(this, EventArgs.Empty);
    // }
    //
    //
    //
    // protected void RaisePrerequisitesChanged(CollectionChangedArgs<Coordinate> args)
    // {
    //     this.PrerequisitesChanged?.Invoke(this, args);
    // }
}

