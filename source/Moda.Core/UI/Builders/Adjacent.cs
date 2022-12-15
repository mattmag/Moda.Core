// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.UI.Builders;

public enum HAdjacent
{
    LeftOf,
    RightOf,
}

public enum VAdjacent
{
    Above,
    Below,
}


public enum NAdjacent
{
    BetaToOtherAlpha,
    AlphaToOtherBeta,
}


public static class AdjacentExtensions
{
    public static NAdjacent ToNeutral(this HAdjacent adjacency) => adjacency switch
        {
            HAdjacent.LeftOf => NAdjacent.BetaToOtherAlpha,
            HAdjacent.RightOf => NAdjacent.AlphaToOtherBeta,
            _ => throw new ArgumentOutOfRangeException(nameof(adjacency), adjacency, null)
        };
    
    public static NAdjacent ToNeutral(this VAdjacent adjacency) => adjacency switch
        {
            VAdjacent.Above => NAdjacent.BetaToOtherAlpha,
            VAdjacent.Below => NAdjacent.AlphaToOtherBeta,
            _ => throw new ArgumentOutOfRangeException(nameof(adjacency), adjacency, null)
        };
    
    public static VAdjacent ToVertical(this NAdjacent adjacency) => adjacency switch
        {
            NAdjacent.BetaToOtherAlpha => VAdjacent.Above,
            NAdjacent.AlphaToOtherBeta => VAdjacent.Below,
            _ => throw new ArgumentOutOfRangeException(nameof(adjacency), adjacency, null)
        };
    
    public static HAdjacent ToHorizontal(this NAdjacent adjacency) => adjacency switch
        {
            NAdjacent.BetaToOtherAlpha => HAdjacent.LeftOf,
            NAdjacent.AlphaToOtherBeta => HAdjacent.RightOf,
            _ => throw new ArgumentOutOfRangeException(nameof(adjacency), adjacency, null)
        };
}