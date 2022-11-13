using Optional.Unsafe;

namespace Moda.Core.UI.Builders;

public abstract class AnchoredAxisBuilder : ICellBuilder
{
    
    public AnchoredAxisBuilder(BoundariesRecipe boundariesBoundariesRecipe, Axis axis, Neutral anchor)
    {
        this.BoundariesRecipe = boundariesBoundariesRecipe;
        this.Axis = axis;
        this.MyAxisRecipe = boundariesBoundariesRecipe.GetAxisRecipe(axis);
        this.Anchor = anchor;
        
        // TODO: set initial coordinates based on anchor
    }
    
    
    public Neutral Anchor { get; }
    public BoundariesRecipe BoundariesRecipe { get; }
    public Axis Axis { get; }
    public AxisRecipe MyAxisRecipe { get; }
    
    
    
    protected void SetLength(Length length)
    {
        switch (this.Anchor)
        {
            // TODO:
            case Neutral.Alpha: 
                // this.MyAxisRecipe.Beta = (this.MyAxisRecipe.Alpha.ValueOrFailure() + length).Some<Length>();
                break;
            case Neutral.Center:
                // this.MyAxisRecipe.Alpha = (new CenterOfParent() - (length / 2)).Some<Length>();
                // this.MyAxisRecipe.Beta = (new CenterOfParent() + (length / 2)).Some<Length>();
                break;
            case Neutral.Beta:
                // this.MyAxisRecipe.Alpha = (this.MyAxisRecipe.Beta.ValueOrFailure() - length).Some<Length>();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    // public CellBuilder FillRemaining()
    // {
    //     // TODO ...
    // }
}