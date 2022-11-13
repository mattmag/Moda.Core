namespace Moda.Core.UI.Builders;

public interface IInitializeVerticalAxis : ICellBuilder
{
    AnchoredVerticalBuilder AnchorAt(Vertical anchor);
}