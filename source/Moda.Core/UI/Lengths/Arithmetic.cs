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


    // #warning opportunities for code reuse throughout
    // protected static T ReuseOrCreate<T>(ILength lengthA, ILength lengthB, Operation operation)
    //     where T : Arithmetic, new()
    // {
    //     if (lengthA is T reuse)
    //     {
    //         if (lengthB is T other)
    //         {
    //             reuse.Absorb(other, operation);
    //         }
    //         else
    //         {
    //             reuse.components.Add(lengthB.Calculate);
    //             reuse.operations.Add(operation);
    //             reuse.AddLength(lengthB);
    //         }
    //
    //         return reuse;
    //     }
    //
    //     if (lengthB is T arithmeticB)
    //     {
    //         arithmeticB.components.Add(lengthA.Calculate);
    //         arithmeticB.operations.Add(operation);
    //         arithmeticB.AddLength(lengthA);
    //         return arithmeticB;
    //     }
    //
    //     T arithmetic = new();
    //     arithmetic.components.Add(lengthA.Calculate);
    //     arithmetic.components.Add(lengthB.Calculate);
    //     arithmetic.operations.Add(operation);
    //     arithmetic.AddLength(lengthA);
    //     arithmetic.AddLength(lengthB);
    //     return arithmetic;
    // }
    //
    //
    // protected static T ReuseOrCreate<T>(ILength length, Single constant, Operation operation)
    //     where T : Arithmetic, new()
    // {
    //     if (length is T reuse)
    //     {
    //         reuse.components.Add(() => constant);
    //         reuse.operations.Add(operation);
    //         return reuse;
    //     }
    //     
    //     T arithmetic = new();
    //     arithmetic.components.Add(length.Calculate);
    //     arithmetic.components.Add(() => constant);
    //     arithmetic.operations.Add(operation);
    //     arithmetic.AddLength(length);
    //     return arithmetic;
    // }
    //
    //
    // protected static T ReuseOrCreate<T>(Single constant, ILength length, Operation operation)
    //     where T : Arithmetic, new()
    // {
    //     if (length is T reuse)
    //     {
    //         reuse.components.Insert(0, () => constant);
    //         reuse.operations.Insert(0, operation);
    //         return reuse;
    //     }
    //     
    //     T arithmetic = new();
    //     arithmetic.components.Insert(0, () => constant);
    //     arithmetic.components.Insert(1, length.Calculate);
    //     arithmetic.operations.Insert(0, operation);
    //     arithmetic.AddLength(length);
    //     return arithmetic;
    // }
    //
    //
    // protected void Absorb(Arithmetic other, Operation joiningOperation)
    // {
    //     foreach (ILength length in other.Lengths)
    //     {
    //         AddLength(length);
    //     }
    //     
    //     this.components.AddRange(other.components);
    //     this.operations.Add(joiningOperation);
    //     this.operations.AddRange(other.operations);
    // }
    
    protected new void AddLength(ILength length)
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
