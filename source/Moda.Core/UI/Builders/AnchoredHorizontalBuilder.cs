namespace Moda.Core.UI.Builders;

public class AnchoredHorizontalBuilder : AnchoredAxisBuilder
{
    public AnchoredHorizontalBuilder(BoundariesRecipe boundariesBoundariesRecipe, Horizontal anchor)
        : base(boundariesBoundariesRecipe, Axis.X, ConvertAnchor(anchor))
    {
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