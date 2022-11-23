// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.Utility.Data;

/// <summary>
///     Used to control how iteration will proceed during processing of a graph.
/// </summary>
public enum GraphDirective
{
    /// <summary>
    ///     Continue to iterate the graph as normal.
    /// </summary>
    Continue = 1,
    
    /// <summary>
    ///     Stop iterating the current branch, but continue to iterate the rest of the graph.
    /// </summary>
    DepthStop = 2,
}
