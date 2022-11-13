using Optional;

namespace Moda.Core.UI.Builders;

public class CompositionRecipe
{
    public IEnumerable<Object> CellComponents { get; set; } = Enumerable.Empty<Object>();
    public Option<Cell> Parent { get; set; }
    public Option<Int32> InsertionIndex { get; set; }
}