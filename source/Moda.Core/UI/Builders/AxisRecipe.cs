using Optional;

namespace Moda.Core.UI.Builders;

public class AxisRecipe
{
    private Option<ILength> _alpha;
    public Option<ILength> Alpha
    {
        get => this._alpha;
        set
        {
            if (this._alpha.HasValue)
            {
                throw new InvalidOperationException("A previous step has already set this value");
            }
            this._alpha = value;
        }
    }

    private Option<ILength> _beta;
    public Option<ILength> Beta
    {
        get => this._beta;
        set
        {
            if (this._beta.HasValue)
            {
                throw new InvalidOperationException("A previous step has already set this value");
            }
            this._beta = value;
        }
    }
}