using Moda.Core.Utility.Data;
using Optional;

namespace Moda.Core.UI.Builders;

public class CellComposer : ICellComposer, IReadyForConstruction
{
    public CellComposer(BoundariesRecipe boundariesRecipe)
    {
        this.BoundariesRecipe = boundariesRecipe;
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


    public IReadyForConstruction InsertAt(Cell cell, Int32 index)
    {
        this.CompositionRecipe.Parent = cell.Some();
        this.CompositionRecipe.InsertionIndex = Math.Max(0, Math.Min(cell.Children.Count, index)).Some();
        return this;
    }


    public IReadyForConstruction InsertBefore(Cell peer)
    {
        this.CompositionRecipe.Parent = peer.Parent;
        this.CompositionRecipe.InsertionIndex = peer.Children.IndexOf(peer);
        return this;
    }


    public IReadyForConstruction InsertAfter(Cell peer)
    {
        this.CompositionRecipe.Parent = peer.Parent;
        this.CompositionRecipe.InsertionIndex = peer.Children.IndexOf(peer).Map(i => i + 1);
        return this;
    }


    public CellRecipe GetRecipe()
    {
        throw new NotImplementedException();
    }
}