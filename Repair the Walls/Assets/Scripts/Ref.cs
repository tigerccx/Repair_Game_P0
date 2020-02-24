using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed class Ref<T>
{
    private readonly Func<T> getter;
    private readonly Action<T> setter;

    //public Ref(T obj)
    //{
    //    this.getter = MakeGetter(obj);
    //    this.setter = MakeSetter(obj);
    //}

    public Ref(Func<T> getter, Action<T> setter)
    {
        this.getter = getter;
        this.setter = setter;
    }

    public T Value { get { return getter(); } set { setter(value); } }

    //static private Func<T> MakeGetter(ref T obj)
    //{
    //    return () => obj;
    //}

    //static private Action<T> MakeSetter(T obj)
    //{
    //    return x => { obj = x; };
    //}
}

