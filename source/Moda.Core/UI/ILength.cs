// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.UI.Lengths;

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
