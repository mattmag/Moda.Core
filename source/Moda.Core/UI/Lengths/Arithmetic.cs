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
    private readonly List<Func<Option<Single>>> components = new();
    private readonly List<Operation> operations = new();


    protected Arithmetic()
    {
        
    }

    protected Arithmetic(ILength lengthA, ILength lengthB, Operation operation)
    {
        this.AppendLengthComponent(lengthA);
        this.AppendLengthComponent(lengthB);
        this.AppendOperation(operation);
    }

    protected Arithmetic(ILength length, Single constant, Operation operation)
    {
        this.AppendLengthComponent(length);
        this.AppendConstantComponent(constant);
        this.AppendOperation(operation);
    }
    
    protected Arithmetic(Single constant, ILength length, Operation operation)
    {
        this.AppendConstantComponent(constant);
        this.AppendLengthComponent(length);
        this.AppendOperation(operation);
    }
    
    protected void AppendLengthComponent(ILength length)
    {
        AddLength(length);
        Func<Option<Single>> calc = length switch
            {
                IOptionalLength optionalLength => optionalLength.TryCalculate,
                _ => () => length.Calculate().Some(),
            };
        this.components.Add(calc);
    }

    protected void AppendConstantComponent(Single constant)
    {
        this.components.Add(() => constant.Some());
    }

    protected void AppendOperation(Operation operation)
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

    // TODO: unit test optional
    public Option<Single> TryCalculate()
    {
        if (!this.components.Any())
        {
            return 0f.Some();
        }
        
        if ( this.components.Count !=  this.operations.Count + 1)
        {
            throw new InvalidOperationException(
                $"Number of components `{this.components.Count}` does not match number of" +
                $" operations `{this.operations.Count}` + 1");
        }
        
        // TODO: ? do better ? idk
        Option<Single> running = this.components.First().Invoke();
        
        for (int i = 0; i < this.operations.Count && running.HasValue; i++)
        {
            Int32 index = i;
            Single running1 = running.ValueOrFailure();
            Option<Single> next = this.components[index + 1].Invoke();
            running = next.Map(a => this.operations[index](running1, a));
        }
        

        return running;
    }
    
    // protected void AppendOther(Arithmetic other, Operation joiningOperation)
    // {
    //     this.components.AddRange(other.components);
    //     this.operations.Add(joiningOperation);
    //     this.operations.AddRange(other.operations);
    // }
}
