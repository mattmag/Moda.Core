// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Optional;
using Optional.Unsafe;

namespace Moda.Core.UI.Builders;

public class Ingredient<T>
{
    private readonly Option<T> defaultValue;
    private Option<T> value;
    
    
    public Ingredient()
    {
    }


    public Ingredient(T @default)
    {
        this.defaultValue = @default.Some();
    }
    
    
    public void Set(T val)
    {
        if (this.value.HasValue)
        {
            throw new InvalidOperationException("A previous step has already set this value}");
        }
        this.value = val.Some();
    }
    
    public T Get()
    {
        return this.value.Else(this.defaultValue).ValueOrFailure();
    }
}
