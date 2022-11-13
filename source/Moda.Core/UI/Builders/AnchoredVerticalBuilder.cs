namespace Moda.Core.UI.Builders;

public class AnchoredVerticalBuilder : AnchoredAxisBuilder
{
    public AnchoredVerticalBuilder(BoundariesRecipe boundariesBoundariesRecipe, Vertical anchor)
        : base(boundariesBoundariesRecipe, Axis.Y, ConvertAnchor(anchor))
    {
    }
    
    public ICellComposer WithHeight(Length width)
    {
        this.SetLength(width);
        return new CellComposer(this.BoundariesRecipe);
    }
    
    private static Neutral ConvertAnchor(Vertical anchor)
    {
        return anchor switch
            {
                Vertical.Top => Neutral.Alpha,
                Vertical.Middle => Neutral.Center,
                Vertical.Bottom => Neutral.Beta,
                _ => throw new ArgumentOutOfRangeException(nameof(anchor), anchor, null)
            };
    }
}