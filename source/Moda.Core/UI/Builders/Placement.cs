// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.UI.Builders;

public delegate T CalculatePlacement<out T>(ILength length) where T : ILength;


public class Placement<T> where T : ILength
{
    public Placement(NCoordinate solves, CalculatePlacement<T> calculation)
    {
        Solves = solves;
        Calculation = calculation;
    }
        
    public NCoordinate Solves { get; }
    
    public CalculatePlacement<T> Calculation { get; }


    public void Calculate(ILength length, out ILength alpha, out ILength beta)
    {
        ILength location = Calculation(length);
        switch (this.Solves)
        {
            case NCoordinate.Alpha:
                alpha = location;
                beta = location + length;
                break;
            case NCoordinate.Beta:
                alpha = location - length;
                beta = location;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    public Placement<ILength> ToBase()
    {
        return new(this.Solves, len => this.Calculation(len));
    }
}