// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Moda.Core.Utility.Data;

namespace Moda.Core.UI;

public interface ICalculation
{
    Single Calculate();
    public IEnumerable<Coordinate> Prerequisites { get; }
    event NotificationHandler<ICalculation>? ValueInvalidated;
    event CollectionChangedHandler<ICalculation, Coordinate>? PrerequisitesChanged;
}
