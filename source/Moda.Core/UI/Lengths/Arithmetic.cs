// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

using Optional;
using Optional.Unsafe;

namespace Moda.Core.UI.Lengths;



public abstract class Arithmetic : OptionalLength
{
    // private List<Func<Single>> components = new();
    private List<Func<Option<Single>>> tryComponents = new();
    private List<Operation> operations = new();


    public Arithmetic()
    {
        
    }

    protected Arithmetic(Length lengthA, Length lengthB, Operation operation)
    {
        this.AddLength(lengthA);
        this.AddLength(lengthB);
        this.AddOperation(operation);
    }

    protected Arithmetic(Length length, Single constant, Operation operation)
    {
        this.AddLength(length);
        this.AddConstant(constant);
        this.AddOperation(operation);
    }
    
    protected Arithmetic(Single constant, Length length, Operation operation)
    {
        this.AddConstant(constant);
        this.AddLength(length);
        this.AddOperation(operation);
    }
    
    protected new void AddLength(Length length)
    {
        base.AddLength(length);
        this.tryComponents.Add(() => length.Calculate().Some());
    }
    
    protected void AddLength(OptionalLength length)
    {
        base.AddLength(length);
        this.tryComponents.Add(length.TryCalculate);
    }

    protected void AddConstant(Single constant)
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


    public override Option<Single> TryCalculate()
    {
        // TODO: ? do better ? idk
        Option<Single> running = this.tryComponents.First().Invoke();
        while (running.HasValue)
        {
            for (int i = 0; i < this.tryComponents.Count; i++)
            {
                Option<Single> next = this.tryComponents[i + 1].Invoke();
                Int32 index = i;
                Option<Single> running1 = running;
                running = next.Map(a => this.operations[index](running1.ValueOrFailure(), a));
            }
        }

        return running;
    }
}
