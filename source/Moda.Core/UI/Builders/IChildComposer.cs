namespace Moda.Core.UI.Builders;

public interface IChildComposer
{
    IReadyForConstruction AppendTo(Cell cell);
    IReadyForConstruction InsertAt(Cell cell, int index);
    IReadyForConstruction InsertBefore(Cell peer);
    IReadyForConstruction InsertAfter(Cell peer);
}