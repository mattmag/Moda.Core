// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.Utility.Data;

namespace Moda.Core.UI;

public class DefaultRecipe : ICalculable
{
    public IEnumerable<Coordinate> Prerequisites => Enumerable.Empty<Coordinate>();
    
    public event EventHandler? ValueInvalidated;
    public event EventHandler<CollectionChangedArgs<Coordinate>>? PrerequisitesChanged;
    
    public Single Calculate()
    {
        return 0;
    }
}
