using Moda.Core.Utility.Data;
using Optional;
using Optional.Unsafe;

namespace Moda.Core.UI.Builders;

public class CellComposer : ICellComposer, IReadyForConstruction
{
    public CellComposer(BoundariesRecipe boundariesRecipe)
    {
        this.BoundariesRecipe = boundariesRecipe;
        this.CompositionRecipe = new();
    }

    public BoundariesRecipe BoundariesRecipe { get; }
    public CompositionRecipe CompositionRecipe { get; }

    
    public IChildComposer WithComponents(Object[] components)
    {
        return WithComponents((IEnumerable<Object>)components);
    }


    public IChildComposer WithComponents(IEnumerable<Object> components)
    {
        this.CompositionRecipe.CellComponents = components;
        return this;
    }

    

    public IReadyForConstruction AppendTo(Cell cell)
    {
        this.CompositionRecipe.Parent = cell.Some();
        return this;
    }


    public IReadyForConstruction InsertAt(Cell parent, Int32 index)
    {
        this.CompositionRecipe.Parent = parent.Some();
        this.CompositionRecipe.InsertionIndex =
            Math.Max(0, Math.Min(parent.Children.Count, index)).Some();
        return this;
    }


    public IReadyForConstruction InsertBefore(Cell peer)
    {
        Cell parent = peer.Parent.ValueOrFailure();
        this.CompositionRecipe.Parent = parent.Some();
        this.CompositionRecipe.InsertionIndex = parent.Children.IndexOf(peer).OrFailure();
        return this;
    }


    public IReadyForConstruction InsertAfter(Cell peer)
    {
        Cell parent = peer.Parent.ValueOrFailure();
        this.CompositionRecipe.Parent = parent.Some();
        this.CompositionRecipe.InsertionIndex = parent.Children.IndexOf(peer).Map(i => i + 1)
                .OrFailure();
        return this;
    }


    public CellRecipe GetRecipe()
    {
        return new(this.BoundariesRecipe, this.CompositionRecipe);
    }
}