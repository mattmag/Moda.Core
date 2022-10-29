// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.Utility.Geometry;

/// <summary>
///     Pairs an <see cref="Intersection"/> result with a rectangle <see cref="Side"/>.
/// </summary>
/// <param name="Side">
///     The <see cref="Side"/> of the rectangle that the intersection occurred on.
/// </param>
/// <param name="Intersection">
///     Details on the intersection.
/// </param>
public record SideIntersection(Side Side, Intersection Intersection);