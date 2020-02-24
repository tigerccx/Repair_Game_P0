using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class Tester : MonoBehaviour
{
    public int i;
    Ref<int> refInt;

    // Start is called before the first frame update
    void Start()
    {
        //refInt = new Ref<int>(() => i, z => { i = z; });
        //refInt.Value = 10;
        //print(i);

        Test0 t0 = new Test0();
        object o0 = t0;
        object o1 = t0;
        print(o0 == o1);
        print(o0.Equals(o1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

class Test0
{
    private int I { get; set; }
    public int GetI()
    {
        return I;
    }
}
