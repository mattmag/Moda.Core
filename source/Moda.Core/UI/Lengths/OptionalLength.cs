// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Optional;

namespace Moda.Core.UI.Lengths;

public abstract class OptionalLength : Length
{
    public abstract Option<Single> TryCalculate();
}
