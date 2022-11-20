namespace Moda.Core.UI.Builders;

public class CellRecipe
{
    public CellRecipe(BoundariesRecipe boundaries, CompositionRecipe composition)
    {
        Boundaries = boundaries;
        Composition = composition;
    }


    public BoundariesRecipe Boundaries { get; }
    public CompositionRecipe Composition { get; }
}