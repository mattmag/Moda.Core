namespace Moda.Core.UI.Builders;

public class BoundariesRecipe
{
    public AxisRecipe XBoundary { get; } = new();
    public AxisRecipe YBoundary { get; } = new();
    
    public AxisRecipe GetAxisRecipe(Axis axis)
    {
        return axis switch
            {
                Axis.X => this.XBoundary,
                Axis.Y => this.YBoundary,
                _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
            };
    }
}