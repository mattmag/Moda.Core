// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.Utility.Data;

public delegate void ValueChangedHandler<TSender, TValue>(TSender sender,
    ValueChangedArgs<TValue> args);

public class ValueChangedArgs<T>
{
    public ValueChangedArgs(T oldValue, T newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }


    public T OldValue { get; }
    public T NewValue { get;  }
}
