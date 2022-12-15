// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.UI.Builders;

public delegate T CalculatePlacement<T>(Length length) where T : Length;

public class Placement<T> where T : Length
{
    public Placement(NCoordinate solves, CalculatePlacement<T> getLocation)
    {
        Solves = solves;
        GetLocation = getLocation;
    }
        
    public NCoordinate Solves { get; }
    
    public CalculatePlacement<T> GetLocation { get; }


    public void Calculate(Length length, out Length alpha, out Length beta)
    {
        Length location = GetLocation(length);
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


    public Placement<Length> ToBase()
    {
        return new(this.Solves, len => this.GetLocation(len));
    }
}