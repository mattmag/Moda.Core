// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.UI.Builders;

public enum HAnchor
{
    Left = 1,
    Center = 2,
    Right = 3,
}

public enum VAnchor
{
    Up = 1,
    Middle = 2,
    Down = 3,
}

public enum NAnchor
{
    Alpha = 1,
    Center = 2,
    Beta = 3,
}


public static class AnchorExtensions
{
    public static NAnchor ToNeutral(this HAnchor anchor) => anchor switch
        {
            HAnchor.Left => NAnchor.Alpha,
            HAnchor.Center => NAnchor.Center,
            HAnchor.Right => NAnchor.Beta,
            _ => throw new ArgumentOutOfRangeException(nameof(anchor), anchor, null)
        };
    
    public static NAnchor ToNeutral(this VAnchor anchor) => anchor switch
        {
            VAnchor.Up => NAnchor.Alpha,
            VAnchor.Middle => NAnchor.Center,
            VAnchor.Down => NAnchor.Beta,
            _ => throw new ArgumentOutOfRangeException(nameof(anchor), anchor, null)
        };
    
    public static HAnchor ToHorizontal(this NAnchor anchor) => anchor switch
        {
            NAnchor.Alpha => HAnchor.Left,
            NAnchor.Center => HAnchor.Center,
            NAnchor.Beta => HAnchor.Right,
            _ => throw new ArgumentOutOfRangeException(nameof(anchor), anchor, null)
        };
    
    public static VAnchor ToVertical(this NAnchor anchor) => anchor switch
        {
            NAnchor.Alpha => VAnchor.Up,
            NAnchor.Center => VAnchor.Middle,
            NAnchor.Beta => VAnchor.Down,
            _ => throw new ArgumentOutOfRangeException(nameof(anchor), anchor, null)
        };
}