// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Optional;

namespace Moda.Core.UI.Builders;

public class AxisRecipe
{

    public Ingredient<Length> Alpha { get; } = new();
    
    public Ingredient<Length> Beta { get; } = new();
    
    // private Option<Length> _alpha;
    // public Option<Length> Alpha
    // {
    //     get => this._alpha;
    //     set
    //     {
    //         if (this._alpha.HasValue)
    //         {
    //             throw new InvalidOperationException("A previous step has already set this value");
    //         }
    //         this._alpha = value;
    //     }
    // }
    //
    // private Option<Length> _beta;
    // public Option<Length> Beta
    // {
    //     get => this._beta;
    //     set
    //     {
    //         if (this._beta.HasValue)
    //         {
    //             throw new InvalidOperationException("A previous step has already set this value");
    //         }
    //         this._beta = value;
    //     }
    // }
}