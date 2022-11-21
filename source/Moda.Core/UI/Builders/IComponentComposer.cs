namespace Moda.Core.UI.Builders;

public interface IComponentComposer
{
    IChildComposer WithComponents(params Object[] objects);
    IChildComposer WithComponents(IEnumerable<Object> objects);
}