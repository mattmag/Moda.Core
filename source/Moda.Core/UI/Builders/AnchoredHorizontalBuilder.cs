using Moda.Core.Lengths;
using Optional;

namespace Moda.Core.UI.Builders;

// TODO: interfaces and/or explicit implementation for abstraction during build?
public class AnchoredHorizontalBuilder : AnchoredAxisBuilder
{
    public AnchoredHorizontalBuilder(BoundariesRecipe boundariesBoundariesRecipe, Horizontal anchor)
        : base(boundariesBoundariesRecipe, Axis.X, ConvertAnchor(anchor))
    {
        //  TODO: placeholder
        this.MyAxisRecipe.Alpha = new Pixels(0).Some<Length>();
        this.MyAxisRecipe.Beta = new Pixels(0).Some<Length>();
    }


    public IInitializeVerticalAxis WithWidth(Length width)
    {
        this.SetLength(width);
        return new CellBuilder(this.BoundariesRecipe);
    }
    
    private static Neutral ConvertAnchor(Horizontal anchor)
    {
        return anchor switch
            {
                Horizontal.Left => Neutral.Alpha,
                Horizontal.Center => Neutral.Center,
                Horizontal.Right => Neutral.Beta,
                _ => throw new ArgumentOutOfRangeException(nameof(anchor), anchor, null)
            };
    }
}