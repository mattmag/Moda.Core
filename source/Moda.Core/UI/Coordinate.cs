// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using System.Diagnostics;
using Moda.Core.Utility.Data;
using Optional;

namespace Moda.Core.UI;

/// <summary>
///     A coordinate defines one end of a <see cref="Boundary"/>, or put another way, one corner of
///     a <see cref="Cell"/>. 
/// </summary>
[DebuggerDisplay("{DebugName}")]
public class Coordinate : IDependentValue
{
    //##############################################################################################
    //
    //  Constructors
    //
    //##############################################################################################
    public Coordinate(ICalculable calculation)
    {
        this._calculation = calculation;
        InitializeCalculation(this._calculation);
    }
    
    
    //##############################################################################################
    //
    //  Events
    //
    //##############################################################################################

    // TODO: can we just set RelativeValue to Option.None?
    // TODO: default EventHandler allows for null sender...do we want strongly typed again too?
    /// <summary>
    ///     Fired to indicate that the results of the previous calculation are no longer valid and
    ///     must be re-evaluated.
    /// </summary>
    public event EventHandler? ValueInvalidated;
    
    /// <summary>
    ///     Fired to indicate that the prerequisites of the <see cref="Calculation"/> (coordinates that
    ///     must be calculated before this one) have changed.
    /// </summary>
    public event EventHandler<CollectionChangedArgs<Coordinate>>? PrerequisitesChanged;
    
    
    
    //##############################################################################################
    //
    //  Properties
    //
    //##############################################################################################


    private ICalculable _calculation;
    /// <summary>
    ///     The calculation to use when calling <see cref="Calculate"/>.
    /// </summary>
    public ICalculable Calculation
    {
        get
        {
            return this._calculation;
        }
        set
        {
            if (this._calculation != value)
            {
                ICalculable old = this._calculation;
                old.ValueInvalidated -= OnCalculationOnValueInvalidated;
                old.PrerequisitesChanged -= OnCalculationOnPrerequisitesChanged;

                this._calculation = value;
                InitializeCalculation(this._calculation);
                
                this.CalculationChanged?.Invoke(this, new(old, value));
            }
        }
    }
    
    /// <summary>
    ///     Fired when the value of <see cref="Calculation"/> has changed.
    /// </summary>
    public event EventHandler<ValueChangedArgs<ICalculable>>? CalculationChanged;
    
    
    
    private Option<Single> _relativeValue;
    /// <summary>
    ///     The value of the coordinate relative to it's parent, or <see cref="Option.None{T}"/> if
    ///     the value has not yet been calculated.
    /// </summary>
    public Option<Single> RelativeValue
    {
        get => this._relativeValue;
        private set
        {
            if (this._relativeValue != value)
            {
                Option<Single> old = this._relativeValue;
                this._relativeValue = value;
                this.RelativeValueChanged?.Invoke(this, new(old, value));
                UpdateAbsolute();
            }
        }
    }
    /// <summary>
    ///     Fired when the value of <see cref="RelativeValue"/> has changed.
    /// </summary>
    public event EventHandler<ValueChangedArgs<Option<Single>>>? RelativeValueChanged;

    
    
    
    private Option<Single> _tare;
    /// <summary>
    ///     The <see cref="AbsoluteValue"/> of the parent's
    ///     <see cref="Boundary.AlphaCoordinate"/> (in the same axis) that is used to add to this
    ///     coordinate's <see cref="RelativeValue"/> to calculate it's <see cref="AbsoluteValue"/>.
    /// </summary>
    /// <remarks>
    ///     If the owning cell does not have a parent, or it's parent's boundary's absolute
    ///     coordinate is not yet known, this value will be <see cref="Option.None{T}"/>.
    /// </remarks>
    public Option<Single> Tare
    {
        get => this._tare;
        set
        {
            if (this._tare != value)
            {
                Option<Single> old = this._tare;
                this._tare = value;
                this.TareChanged?.Invoke(this, new(old, value));
                UpdateAbsolute();
            }
        }
    }
    
    /// <summary>
    ///     Fired when the value of <see cref="Tare"/> has changed.
    /// </summary>
    public event EventHandler<ValueChangedArgs<Option<Single>>>? TareChanged;

    
    
    
    private Option<Single> _absoluteValue;
    /// <summary>
    ///     The value of the coordinate in screen space, or <see cref="Option.None{T}"/> if
    ///     the value has not yet been calculated.
    /// </summary>
    /// <remarks>
    ///     Calculated as <see cref="Tare"/> + <see cref="RelativeValue"/>.
    /// </remarks>
    public Option<Single> AbsoluteValue
    {
        get => this._absoluteValue;
        private set
        {
            if (this._absoluteValue != value)
            {
                Option<Single> old = this._absoluteValue;
                this._absoluteValue = value;
                this.AbsoluteValueChanged?.Invoke(this, new(old, value));
            }
        }
    }
    /// <summary>
    ///     Fired when the value of <see cref="AbsoluteValue"/> has changed.
    /// </summary>
    public event EventHandler<ValueChangedArgs<Option<Single>>>? AbsoluteValueChanged;

    
    
    // TODO: this
    public IEnumerable<Coordinate> Prerequisites => this._calculation.Prerequisites
        ?? Enumerable.Empty<Coordinate>();
    
    public string DebugName { get; set; } = String.Empty;
    

    //##############################################################################################
    //
    //  Public Methods
    //
    //##############################################################################################
    
    /// <summary>
    ///     Evaluate the <see cref="Calculation"/> and assign it's results to
    ///     <see cref="AbsoluteValue"/>.
    /// </summary>
    /// <remarks>
    ///     If <see cref="Tare"/> has a value, <see cref="AbsoluteValue"/> will be updated as well.
    /// </remarks>
    public void Calculate()
    {
        this.RelativeValue = this._calculation.Calculate().Some();
    }


    //##############################################################################################
    //
    //  Private Methods
    //
    //##############################################################################################


    private void InitializeCalculation(ICalculable calculable)
    {
        calculable.ValueInvalidated += OnCalculationOnValueInvalidated;
        calculable.PrerequisitesChanged += OnCalculationOnPrerequisitesChanged;
    }

    private void UpdateAbsolute()
    {
        this.AbsoluteValue = this.Tare
            .FlatMap(tare => this.RelativeValue.Map(relative => tare + relative));
    }

    
    private void OnCalculationOnPrerequisitesChanged(Object? s, CollectionChangedArgs<Coordinate> e)
    {
        this.PrerequisitesChanged?.Invoke(this, e);
    }


    private void OnCalculationOnValueInvalidated(Object? s, EventArgs e)
    {
        this.ValueInvalidated?.Invoke(this, e);
    }

    
    
}