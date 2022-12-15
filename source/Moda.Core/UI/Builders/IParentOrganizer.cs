// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.UI.Builders;

public interface IParentOrganizer
{
    BoundariesInitializer Append();
    BoundariesInitializer InsertAt(Int32 index);
    BoundariesInitializer InsertBefore(Cell peer);
    BoundariesInitializer InsertAfter(Cell peer);
}
