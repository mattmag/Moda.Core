namespace Moda.Core.UI.Builders;

public class CellRecipe
{
    public BoundariesRecipe Boundaries { get; } = new();
    public CompositionRecipe Composition { get; } = new();
}