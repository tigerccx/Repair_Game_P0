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
        Type type = typeof(Test0);
        string memberName = "I";
        var a = type.GetProperty(memberName, BindingFlags.NonPublic | BindingFlags.Instance| BindingFlags.Public| BindingFlags.Static);
        print(a);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
