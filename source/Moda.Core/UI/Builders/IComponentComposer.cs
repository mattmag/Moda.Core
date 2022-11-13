namespace Moda.Core.UI.Builders;

public interface IComponentComposer
{
    IChildComposer WithComponents(params object[] objects);
    IChildComposer WithComponents(IEnumerable<object> objects);
}