// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.UI;
using Moda.Core.Utility.Data;

namespace Moda.Core.Lengths;

public class Pixels : ILength
{
    public Pixels(Int32 value)
    {
        Value = value;
    }


     private Int32 _value;
    public Int32 Value
    {
        get => this._value;
        set
        {
            if (this._value != null)
            {
                this._value = value;
                this.ValueInvalidated?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public IEnumerable<Coordinate> Prerequisites { get; } = Enumerable.Empty<Coordinate>();
    
    public event EventHandler? ValueInvalidated;
    public event EventHandler<CollectionChangedArgs<Coordinate>>? PrerequisitesChanged;


    public Single Calculate()
    {
        return this.Value;
    }
}
