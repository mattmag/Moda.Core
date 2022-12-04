// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.UI.Lengths;

namespace Moda.Core.UI;

public interface ILength : ICalculation
{
    public static Sum operator +(ILength length, ILength lengthB) =>
        Sum.Add(length, lengthB);
    
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

}

