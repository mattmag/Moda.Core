// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.Utility.Data;

namespace Moda.Core.UI;

public abstract class Length : ICalculable
{
    // TODO: prereqs
    public IEnumerable<Coordinate> Prerequisites { get; }
    
    public event EventHandler? ValueInvalidated;
    public event EventHandler<CollectionChangedArgs<Coordinate>>? PrerequisitesChanged;
    public abstract Single Calculate();
    
    
    // public static Add operator +(Length lengthA, Length lengthB) =>
    //     new Add(lengthA, lengthB);
    //
    // public static Subtract operator -(Length lengthA, Length lengthB) =>
    //     new Subtract(lengthA, lengthB);
    //
    // public static Subtract operator -(Length length) => // eh? can add and subtract be combined and we add a Sign to Length
    //     new Subtract(new Pixels(0), length);
    //
    // public static DivideByConstant operator /(Length lengthA, Single constant) =>
    //     new DivideByConstant(lengthA, constant);
}
