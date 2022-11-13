using Moda.Core.Entity;
using Moda.Core.UI.Builders;
using Moda.Core.Utility.Geometry;
using Optional;
using Optional.Unsafe;

namespace Moda.Core.UI;

public class Hive
{
    private readonly IEntityManager entityManager;
    
    public Hive(IEntityManager entityManager)
    {
        this.entityManager = entityManager;
    }

    
    public Cell Root { get; }
    

    private Size2 _viewPortSize;
    public Size2 ViewPortSize
    {
        get => this._viewPortSize;
        set
        {
            this._viewPortSize = value;
        }
    }


    public Cell NewCell(Func<IInitializeCell, IReadyForConstruction> builder)
    {
        CellRecipe recipe = builder(new CellBuilder(new())).GetRecipe();
        
        Cell cell = new();
        cell.XBoundary.AlphaCoordinate.Recipe = recipe.Boundaries.XBoundary.Alpha.ValueOrFailure();
        cell.XBoundary.BetaCoordinate.Recipe = recipe.Boundaries.XBoundary.Beta.ValueOrFailure();
        cell.YBoundary.AlphaCoordinate.Recipe = recipe.Boundaries.YBoundary.Alpha.ValueOrFailure();
        cell.YBoundary.BetaCoordinate.Recipe = recipe.Boundaries.YBoundary.Beta.ValueOrFailure();

        recipe.Composition.Parent.MatchSome(parent =>
            recipe.Composition.InsertionIndex.Match(index =>
                    {
                        parent.InsertChild(cell, index);
                    },
                () =>
                    {
                        parent.AppendChild(cell);
                    }));
        
        UInt64 id = this.entityManager.AddEntity(recipe.Composition.CellComponents.Prepend(cell));
        cell.Entity = id.Some();

        return cell;
    }
}