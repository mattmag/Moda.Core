// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.UI.Builders;

public class ComponentAssigner : IComponentAssignerOrReadyToBuild, IReadyToBuild
{
    private readonly CellBuilderState runningState;


    public ComponentAssigner(CellBuilderState runningState)
    {
        this.runningState = runningState;
    }
    
    public IReadyToBuild WithComponents(params Object[] components)
    {
        return WithComponents((IEnumerable<Object>)components);
    }


    public IReadyToBuild WithComponents(IEnumerable<Object> components)
    {
        this.runningState.Composition.Components.Set(components);
        return this;
    }


    public CellBuilderState GetCellRecipe()
    {
        return this.runningState;
    }
}
