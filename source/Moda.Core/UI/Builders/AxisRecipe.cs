using Optional;

namespace Moda.Core.UI.Builders;

public class AxisRecipe
{
    private Option<Length> _alpha;
    public Option<Length> Alpha
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

    private Option<Length> _beta;
    public Option<Length> Beta
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