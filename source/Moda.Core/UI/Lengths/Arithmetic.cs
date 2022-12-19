// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Optional;
using Optional.Unsafe;

namespace Moda.Core.UI.Lengths;



public abstract class Arithmetic : CompositeLength, IOptionalLength
{
    // private List<Func<Single>> components = new();
    private readonly List<Func<Option<Single>>> tryComponents = new();
    private readonly List<Operation> operations = new();


    protected Arithmetic()
    {
        
    }

    protected Arithmetic(ILength lengthA, ILength lengthB, Operation operation)
    {
        this.AddLengthComponent(lengthA);
        this.AddLengthComponent(lengthB);
        this.AddOperation(operation);
    }

    protected Arithmetic(ILength length, Single constant, Operation operation)
    {
        this.AddLengthComponent(length);
        this.AddConstantComponent(constant);
        this.AddOperation(operation);
    }
    
    protected Arithmetic(Single constant, ILength length, Operation operation)
    {
        this.AddConstantComponent(constant);
        this.AddLengthComponent(length);
        this.AddOperation(operation);
    }
    
    protected void AddLengthComponent(ILength length)
    {
        base.AddLength(length);
        this.tryComponents.Add(() => length.Calculate().Some());
    }

    protected void AddConstantComponent(Single constant)
    {
        this.tryComponents.Add(() => constant.Some());
    }

    protected void AddOperation(Operation operation)
    {
        this.operations.Add(operation);
    }
 

    public override Single Calculate()
    {
        // if (!this.components.Any())
        // {
        //     return 0;
        // }
        //
        // if ( this.components.Count !=  this.operations.Count + 1)
        // {
        //     throw new InvalidOperationException();
        // }
        //
        // Single running = this.components.First().Invoke();
        // for (int i = 0; i < this.operations.Count; i++)
        // {
        //     running = this.operations[i](running, this.components[i+1].Invoke());
        // }
        //
        // return running;
        return TryCalculate().ValueOrFailure();
    }

    // TODO: unit test 
    public Option<Single> TryCalculate()
    {
        if (!this.tryComponents.Any())
        {
            return 0f.Some();
        }
        
        if ( this.tryComponents.Count !=  this.operations.Count + 1)
        {
            throw new InvalidOperationException(
                $"Number of components `{this.tryComponents.Count}` does not match number of" +
                $" operations `{this.operations.Count}` + 1");
        }
        
        // TODO: ? do better ? idk
        Option<Single> running = this.tryComponents.First().Invoke();
        
        for (int i = 0; i < this.operations.Count && running.HasValue; i++)
        {
            Int32 index = i;
            Single running1 = running.ValueOrFailure();
            Option<Single> next = this.tryComponents[index + 1].Invoke();
            running = next.Map(a => this.operations[index](running1, a));
        }
        

        return running;
    }
}
