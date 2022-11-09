// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.Utility.Data;

namespace Moda.Core.UI;

public interface IDependentValue
{
    public IEnumerable<Coordinate> Prerequisites { get; }
    event EventHandler? ValueInvalidated;
    event EventHandler<CollectionChangedArgs<Coordinate>>? PrerequisitesChanged;
}
