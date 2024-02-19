using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FuncPredicate : IPredicate
{
    readonly Func<bool> func;

    public FuncPredicate(Func<bool> func)
    {
        this.func = func;
    }

    public bool Evaluate()
    {
        return func.Invoke();
    }
}
