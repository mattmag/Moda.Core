// This file is part of the Moda.Core project.
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at
// https://mozilla.org/MPL/2.0/

namespace Moda.Core.UI.Lengths;

public abstract class Arithmetic : CompositeLength
{
    private List<Func<Single>> components = new();
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
        this.components.Add(length.Calculate);
    }

    protected void AddConstant(Single constant)
    {
        this.components.Add(() => constant);
    }

    protected void AddOperation(Operation operation)
    {
        this.operations.Add(operation);
    }
 

    public override Single Calculate()
    {
        if (!this.components.Any())
        {
            return 0;
        }
        
        if ( this.components.Count !=  this.operations.Count + 1)
        {
            throw new InvalidOperationException();
        }
        
        Single running = this.components.First().Invoke();
        for (int i = 0; i < this.operations.Count; i++)
        {
            running = this.operations[i](running, this.components[i+1].Invoke());
        }
        
        return running;
    }
}
