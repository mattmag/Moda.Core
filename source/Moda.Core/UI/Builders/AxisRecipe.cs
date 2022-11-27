// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Optional;

namespace Moda.Core.UI.Builders;

public class AxisRecipe
{
    private Option<ILength> _alpha;
    public Option<ILength> Alpha
    {
        get => this._alpha;
        set
        {
            if (this._alpha.HasValue)
            {
                throw new InvalidOperationException("A previous step has already set this value");
            }
            this._alpha = value;
        }
    }

    private Option<ILength> _beta;
    public Option<ILength> Beta
    {
        get => this._beta;
        set
        {
            if (this._beta.HasValue)
            {
                throw new InvalidOperationException("A previous step has already set this value");
            }
            this._beta = value;
        }
    }
}