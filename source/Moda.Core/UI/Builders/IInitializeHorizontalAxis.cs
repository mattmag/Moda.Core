namespace Moda.Core.UI.Builders;

public interface IInitializeHorizontalAxis : ICellBuilder
{
    AnchoredHorizontalBuilder AnchorAt(Horizontal anchor);
}