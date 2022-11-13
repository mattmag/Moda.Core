// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.UI.Builders;

public class CellBuilder : IInitializeCell, IInitializeHorizontalAxis, IInitializeVerticalAxis
{
    private BoundariesRecipe boundariesRecipe;


    public CellBuilder(BoundariesRecipe boundariesRecipe)
    {
        this.boundariesRecipe = boundariesRecipe;
    }
    
    public AnchoredHorizontalBuilder AnchorAt(Horizontal anchor) => new(this.boundariesRecipe, anchor);
    public AnchoredVerticalBuilder AnchorAt(Vertical anchor) => new(this.boundariesRecipe, anchor);

    public BoundariesRecipe BoundariesRecipe => this.boundariesRecipe;
}